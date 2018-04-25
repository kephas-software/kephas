// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DefaultScriptingServiceTest.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the default scripting service test class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Scripting.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;
    using System.Threading;
    using System.Threading.Tasks;

    using Kephas.Composition;
    using Kephas.Composition.Mef.Hosting;
    using Kephas.Dynamic;
    using Kephas.Services;
    using Kephas.Testing.Composition.Mef;

    using NUnit.Framework;

    [TestFixture]
    public class DefaultScriptingServiceTest : CompositionTestBase
    {
        public override ICompositionContext CreateContainer(IEnumerable<Assembly> assemblies = null, IEnumerable<Type> parts = null, Action<MefCompositionContainerBuilder> config = null)
        {
            var assemblyList = new List<Assembly>(assemblies ?? new Assembly[0]);
            assemblyList.Add(typeof(DefaultScriptingService).Assembly); /* Kephas.Scripting */
            return base.CreateContainer(assemblyList, parts, config);
        }

        [Test]
        public void DefaultMessageProcessor_Composition_success()
        {
            var container = this.CreateContainer();
            var scriptingService = container.GetExport<IScriptingService>();
            Assert.IsInstanceOf<DefaultScriptingService>(scriptingService);

            var typedScriptingService = (DefaultScriptingService)scriptingService;
            Assert.IsNotNull(typedScriptingService.Logger);
        }

        [Test]
        public async Task ExecuteAsync_composition_success()
        {
            var container = this.CreateContainer(parts: new[] { typeof(TestScriptInterpreter) });
            var scriptingService = container.GetExport<IScriptingService>();

            var script = new Script("test", "dummy");
            var result = await scriptingService.ExecuteAsync(script);

            Assert.AreEqual("dummy", result);
        }

        [Language("test")]
        public class TestScriptInterpreter : IScriptInterpreter
        {
            /// <summary>
            /// Executes the expression asynchronously.
            /// </summary>
            /// <param name="script">The script to be interpreted/executed.</param>
            /// <param name="args">The arguments (optional).</param>
            /// <param name="executionContext">The execution context (optional).</param>
            /// <param name="cancellationToken">The cancellation token (optional).</param>
            /// <returns>
            /// A promise of the execution result.
            /// </returns>
            public Task<object> ExecuteAsync(
                IScript script,
                IExpando args = null,
                IContext executionContext = null,
                CancellationToken cancellationToken = default)
            {
                return Task.FromResult<object>(script.SourceCode);
            }
        }
    }
}