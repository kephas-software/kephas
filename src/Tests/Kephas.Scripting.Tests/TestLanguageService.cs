// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TestLanguageService.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the test language service class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Scripting.Tests
{
    using System.Threading;
    using System.Threading.Tasks;

    using Kephas.Dynamic;
    using Kephas.Scripting.AttributedModel;
    using Kephas.Services;

    [Language("test")]
    public class TestLanguageService : ILanguageService
    {
        public Task<object?> ExecuteAsync(IScript script,
            IScriptGlobals? scriptGlobals = null,
            IDynamic? args = null,
            IContext? executionContext = null,
            CancellationToken cancellationToken = default)
        {
            return Task.FromResult<object>("executed " + script.SourceCode);
        }
    }
}
