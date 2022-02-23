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
    using System.Threading;
    using System.Threading.Tasks;

    using Kephas.Dynamic;
    using Kephas.Scripting.AttributedModel;
    using Kephas.Services;
    using Kephas.Threading.Tasks;
    using Neo.IronLua;

    /// <summary>
    /// A LUA language service.
    /// </summary>
    [Language(Language)]
    public class LuaLanguageService : ILanguageService
    {
        /// <summary>
        /// The language identifier.
        /// </summary>
        public const string Language = "LUA";

        private readonly Lua engine;

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
        public object? Execute(
            IScript script,
            IScriptGlobals? scriptGlobals = null,
            IDynamic? args = null,
            IContext? executionContext = null)
        {
            args ??= new Expando();
            scriptGlobals ??= new ScriptGlobals(args);

            var scope = this.CreateGlobalScope(scriptGlobals);
            var source = script.GetSourceCode();

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
        public async Task<object?> ExecuteAsync(
            IScript script,
            IScriptGlobals? scriptGlobals = null,
            IDynamic? args = null,
            IContext? executionContext = null,
            CancellationToken cancellationToken = default)
        {
            args ??= new Expando();
            scriptGlobals ??= new ScriptGlobals(args);

            var scope = this.CreateGlobalScope(scriptGlobals);
            var source = await script.GetSourceCodeAsync(cancellationToken).PreserveThreadContext();

            await Task.Yield();

            var chunk = this.engine.CompileChunk(source, "dynamicCode", new LuaCompileOptions());
            var result = chunk.Run(scope);
            var returnValue = this.GetReturnValue(result, scope);
            return returnValue;
        }

        private LuaGlobal CreateGlobalScope(IScriptGlobals scriptGlobals)
        {
            var scope = this.engine.CreateEnvironment();
            foreach (var (key, value) in scriptGlobals.ToDictionary(k => k.ToCamelCase(), v => v))
            {
                scope[key] = value;
            }

            return scope;
        }

        private object? GetReturnValue(LuaResult result, LuaGlobal scope)
        {
            // TODO
            return result.Values.Length == 0 ? null : result.Values[0];
        }
    }
}
