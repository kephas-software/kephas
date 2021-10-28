// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CSharpLanguageService.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the C# language service class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Scripting.CSharp
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;

    using Kephas.Collections;
    using Kephas.Dynamic;
    using Kephas.IO;
    using Kephas.Scripting.AttributedModel;
    using Kephas.Services;
    using Kephas.Threading.Tasks;

    using Microsoft.CodeAnalysis.CSharp.Scripting;
    using Microsoft.CodeAnalysis.Scripting;

    /// <summary>
    /// A C# language service.
    /// </summary>
    [Language(Language, LanguageAlt)]
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
            if (script.SourceCode == null)
            {
                throw new ArgumentException($"{nameof(script.SourceCode)} must nut be null.", nameof(script));
            }

            args ??= new Expando();
            scriptGlobals ??= new ScriptGlobals { Args = args };

            var (globalsScript, assemblies) = this.GetGlobalsScript(scriptGlobals);

            var source = script.SourceCode is string codeText
                ? codeText
                : script.SourceCode is Stream codeStream
                    ? codeStream.ReadAllString()
                    : throw new SourceCodeNotSupportedException(script, typeof(string), typeof(Stream));

            var state = await CSharpScript.RunAsync(
                globalsScript + source,
                options: this.GetScriptOptions(assemblies),
                globals: new Globals { __g = scriptGlobals },
                cancellationToken: cancellationToken)
                .PreserveThreadContext();
            return state.ReturnValue;
        }

        private ScriptOptions GetScriptOptions(IEnumerable<Assembly> assemblies)
        {
            return ScriptOptions.Default.WithReferences(assemblies.ToArray());
        }

        private (string globalsScript, IEnumerable<Assembly> assemblies) GetGlobalsScript(IScriptGlobals scriptGlobals)
        {
            // TODO workaround for CSharp not supporting dynamic
            var sb = new StringBuilder("var globals = new {").AppendLine();
            var assemblies = new HashSet<Assembly>();
            foreach (var kv in scriptGlobals.ToDictionary())
            {
                var typeName = this.GetValueType(kv.Value, assemblies);
                sb.AppendLine($"{kv.Key} = ({typeName})__g[\"{kv.Key}\"],");
            }

            sb.AppendLine("};");

            assemblies.ForEach(a => sb.Insert(0, $"#r \"{a.Location}\"" + Environment.NewLine));

            return (sb.ToString(), assemblies);
        }

        private string GetValueType(object value, HashSet<Assembly> assemblies)
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
                return valueType.FullName;
            }

            var typeDef = valueType.GetGenericTypeDefinition();
            var args = string.Join(",", valueType.GetGenericArguments()
                .Select(t => this.GetTypeName(t, assemblies)));
            var genericMarkerIndex = typeDef.Name.IndexOf('`');

            return $"{typeDef.Namespace}.{typeDef.Name.Substring(0, genericMarkerIndex)}<{args}>";
        }

        public class Globals
        {
            public IDynamic __g { get; set; }
        }
    }
}