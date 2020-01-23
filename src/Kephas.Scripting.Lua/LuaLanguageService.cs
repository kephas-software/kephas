// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LuaLanguageService.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the lua language service class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Scripting.Lua
{
    using System;
    using System.IO;
    using System.Threading;
    using System.Threading.Tasks;

    using Kephas.Dynamic;
    using Kephas.IO;
    using Kephas.Reflection;
    using Kephas.Scripting.AttributedModel;
    using Kephas.Services;
    using Kephas.Threading.Tasks;
    using Neo.IronLua;

    /// <summary>
    /// A LUA language service.
    /// </summary>
    [Language(Language)]
    public class LuaLanguageService : ILanguageService
#if NETSTANDARD2_1
#else
        , ISyncLanguageService
#endif
    {
        /// <summary>
        /// The language identifier.
        /// </summary>
        public const string Language = "LUA";

        private Lua engine;

        /// <summary>
        /// Initializes a new instance of the <see cref="LuaLanguageService"/> class.
        /// </summary>
        public LuaLanguageService()
        {
            this.engine = new Neo.IronLua.Lua();
        }

        /// <summary>
        /// Executes the script.
        /// </summary>
        /// <param name="script">The script to be interpreted/executed.</param>
        /// <param name="scriptGlobals">Optional. The script globals.</param>
        /// <param name="args">Optional. The arguments.</param>
        /// <param name="executionContext">Optional. The execution context.</param>
        /// <returns>
        /// A promise of the execution result.
        /// </returns>
        public object Execute(IScript script, IScriptGlobals scriptGlobals = null, IExpando args = null, IContext executionContext = null)
        {
            args = args ?? new Expando();
            scriptGlobals = scriptGlobals ?? new ScriptGlobals { Args = args };

            var (scope, source) = this.PrepareScope(script, scriptGlobals);

            var chunk = this.engine.CompileChunk(source, "dynamicCode", new LuaCompileOptions());
            var result = chunk.Run(scope);
            var returnValue = this.GetReturnValue(result, scope);
            return returnValue;
        }

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
        public async Task<object> ExecuteAsync(IScript script, IScriptGlobals scriptGlobals = null, IExpando args = null, IContext executionContext = null, CancellationToken cancellationToken = default)
        {
            args = args ?? new Expando();
            scriptGlobals = scriptGlobals ?? new ScriptGlobals { Args = args };

            var (scope, source) = this.PrepareScope(script, scriptGlobals);

            var result = await ((Func<object[]>)(() =>
            {
                var chunk = this.engine.CompileChunk(source, "dynamicCode", new LuaCompileOptions());
                return chunk.Run(scope);
            })).AsAsync(cancellationToken).PreserveThreadContext();
            var returnValue = this.GetReturnValue(result, scope);
            return returnValue;
        }

        private (LuaGlobal scope, string source) PrepareScope(IScript script, IScriptGlobals scriptGlobals)
        {
            var scope = this.engine.CreateEnvironment();
            foreach (var kv in scriptGlobals.ToDictionary(k => k.ToCamelCase(), v => v))
            {
                scope[kv.Key] = kv.Value;
            }

            var source = script.SourceCode is string codeText
                ? codeText
                : script.SourceCode is Stream codeStream
                    ? codeStream.ReadAllString()
                    : throw new SourceCodeNotSupportedException(script, typeof(string), typeof(Stream));

            return (scope, source);
        }

        private object GetReturnValue(LuaResult result, LuaGlobal scope)
        {
            // TODO
            return result.Values.Length == 0 ? null : result.Values[0];
        }
    }
}
