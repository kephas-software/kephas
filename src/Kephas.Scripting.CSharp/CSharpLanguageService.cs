// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CSharpLanguageService.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the C# language service class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Scripting
{
    using System;
    using System.Collections.Generic;
    using System.Dynamic;
    using System.Linq;
    using System.Reflection;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;

    using Kephas.Collections;
    using Kephas.Dynamic;
    using Kephas.Scripting.AttributedModel;
    using Kephas.Services;
    using Kephas.Threading.Tasks;
    using Microsoft.CodeAnalysis.CSharp.Scripting;
    using Microsoft.CodeAnalysis.Scripting;

    /// <summary>
    /// A C# language service.
    /// </summary>
    [Language(Language, LanguageAlt, LanguageShort, LanguageScript)]
    [ServiceName(Language)]
    public class CSharpLanguageService : ILanguageService
    {
        /// <summary>
        /// The language identifier.
        /// </summary>
        public const string Language = "C#";

        /// <summary>
        /// The alternate language identifier.
        /// </summary>
        public const string LanguageAlt = "csharp";

        /// <summary>
        /// The short language identifier.
        /// </summary>
        public const string LanguageShort = "cs";

        /// <summary>
        /// The script language identifier.
        /// </summary>
        public const string LanguageScript = "csx";

        /// <summary>
        /// Executes the script asynchronously.
        /// </summary>
        /// <param name="script">The script to be interpreted/executed.</param>
        /// <param name="scriptGlobals">Optional. The script globals.</param>
        /// <param name="args">Optional. The arguments.</param>
        /// <param name="executionContext">Optional. The execution context.</param>
        /// <param name="cancellationToken">Optional. The cancellation token.</param>
        /// <returns>
        /// A promise of the execution result.
        /// </returns>
        public async Task<object?> ExecuteAsync(
            IScript script,
            IScriptGlobals? scriptGlobals = null,
            IDynamic? args = null,
            IContext? executionContext = null,
            CancellationToken cancellationToken = default)
        {
            script = script ?? throw new ArgumentNullException(nameof(script));
            args ??= new Expando();
            scriptGlobals ??= new ScriptGlobals(args);

            var (globalsSection, refSection, assemblies) = this.GetGlobalsScript(scriptGlobals);

            var source = await script.GetSourceCodeAsync(cancellationToken).PreserveThreadContext();

            var state = await CSharpScript.RunAsync(
                MergeSections(globalsSection, refSection, source),
                options: this.GetScriptOptions(assemblies),
                globals: new Globals { __g = scriptGlobals },
                cancellationToken: cancellationToken)
                .PreserveThreadContext();
            return state.ReturnValue;
        }

        private static string MergeSections(string globalsSection, string refSection, string source)
        {
            if (string.IsNullOrEmpty(globalsSection))
            {
                return refSection + source;
            }

            // insert the globals section after all preprocessor directives
            using var lineReader = new StringReader(source);
            var sb = new StringBuilder(refSection);
            var line = lineReader.ReadLine();
            while (line is not null && (string.IsNullOrWhiteSpace(line) || line.StartsWith("#")))
            {
                sb.AppendLine(line);
                line = lineReader.ReadLine();
            }
            
            sb.Append(globalsSection);

            if (line is not null)
            {
                sb.AppendLine(line);
                sb.Append(lineReader.ReadToEnd());
            }
            
            return sb.ToString();
        }

        private ScriptOptions GetScriptOptions(IEnumerable<Assembly> assemblies)
        {
            return ScriptOptions.Default.WithReferences(assemblies.ToArray());
        }

        private (string globalsScriptSection, string refScriptSection, IEnumerable<Assembly> assemblies) GetGlobalsScript(IScriptGlobals scriptGlobals)
        {
            var sb = new StringBuilder();
            var assemblies = new HashSet<Assembly> { typeof(IDynamicMetaObjectProvider).Assembly };
            foreach (var (key, value) in scriptGlobals.ToDictionary())
            {
                var typeName = this.GetValueType(value, assemblies);
                sb.AppendLine($"var {key} = ({typeName})__g[\"{key}\"];");
            }

            var sbRef = new StringBuilder();
            assemblies.ForEach(a => sbRef.AppendLine($"#r \"{a.Location}\""));

            return (sb.ToString(), sbRef.ToString(), assemblies);
        }

        private string GetValueType(object? value, HashSet<Assembly> assemblies)
        {
            if (value == null)
            {
                return "object";
            }

            var valueType = value.GetType();
            return this.GetTypeName(valueType, assemblies);
        }

        private string GetTypeName(Type valueType, HashSet<Assembly> assemblies)
        {
            assemblies.Add(valueType.Assembly);

            if (!valueType.IsGenericType)
            {
                return valueType.FullName!;
            }

            var typeDef = valueType.GetGenericTypeDefinition();
            var args = string.Join(",", valueType.GetGenericArguments()
                .Select(t => this.GetTypeName(t, assemblies)));
            var genericMarkerIndex = typeDef.Name.IndexOf('`');

            return $"{typeDef.Namespace}.{typeDef.Name[..genericMarkerIndex]}<{args}>";
        }

        public class Globals
        {
            public IDynamic __g { get; set; }
        }
    }
}