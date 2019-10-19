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
    using Kephas.Diagnostics.Contracts;
    using Kephas.Dynamic;
    using Kephas.Reflection;
    using Kephas.Scripting.AttributedModel;
    using Kephas.Services;
    using Kephas.Threading.Tasks;

    using Microsoft.CodeAnalysis.CSharp.Scripting;

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
        public async Task<object> ExecuteAsync(
            IScript script,
            IScriptGlobals scriptGlobals = null,
            IExpando args = null,
            IContext executionContext = null,
            CancellationToken cancellationToken = default)
        {
            Requires.NotNull(script, nameof(script));
            Requires.NotNull(script.SourceCode, nameof(script.SourceCode));

            args = args ?? new Expando();
            scriptGlobals = scriptGlobals ?? new ScriptGlobals { Args = args };

            var globalsScript = this.GetGlobalsScript(scriptGlobals);

            if (script.SourceCode is string codeText)
            {
            }
            else if (script.SourceCode is Stream codeStream)
            {
                using (var reader = new StreamReader(codeStream))
                {
                    codeText = reader.ReadToEnd();
                }
            }
            else
            {
                // TODO localization
                throw new ScriptingException($"Source code type {script.GetType()} not supported. Please provide either a {typeof(string)} or a {typeof(Stream)}.");
            }

            var state = await CSharpScript.RunAsync(globalsScript + codeText, globals: new Globals { __g = scriptGlobals }, cancellationToken: cancellationToken)
                .PreserveThreadContext();
            return state.ReturnValue;
        }

        private string GetGlobalsScript(IScriptGlobals scriptGlobals)
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

            assemblies.ForEach(a => sb.Insert(0, $"#r \"{a.GetName().Name}\"" + Environment.NewLine));

            return sb.ToString();
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
            public IExpando __g { get; set; }
        }
    }
}