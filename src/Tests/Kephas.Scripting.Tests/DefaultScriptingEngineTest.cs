// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DefaultScriptingServiceTest.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the default scripting engine test class.
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
    using Kephas.Scripting.AttributedModel;
    using Kephas.Services;
    using Kephas.Testing.Composition;

    using NUnit.Framework;

    [TestFixture]
    public class DefaultScriptingEngineTest : MefCompositionTestBase
    {
        public override ICompositionContext CreateContainer(
            IAmbientServices ambientServices = null,
            IEnumerable<Assembly> assemblies = null,
            IEnumerable<Type> parts = null,
            Action<MefCompositionContainerBuilder> config = null)
        {
            var assemblyList = new List<Assembly>(assemblies ?? new Assembly[0]);
            assemblyList.Add(typeof(DefaultScriptingEngine).Assembly); /* Kephas.Scripting */
            return base.CreateContainer(ambientServices, assemblyList, parts, config);
        }

        [Test]
        public void DefaultMessageProcessor_Composition_success()
        {
            var container = this.CreateContainer();
            var scriptingEngine = container.GetExport<IScriptingEngine>();
            Assert.IsInstanceOf<DefaultScriptingEngine>(scriptingEngine);

            var typedScriptingService = (DefaultScriptingEngine)scriptingEngine;
            Assert.IsNotNull(typedScriptingService.Logger);
        }

        [Test]
        public async Task ExecuteAsync_composition_success()
        {
            var container = this.CreateContainer(parts: new[] { typeof(TestLanguageService) });
            var scriptingEngine = container.GetExport<IScriptingEngine>();

            var script = new Script("test", "dummy");
            var result = await scriptingEngine.ExecuteAsync(script);

            Assert.AreEqual("dummy", result);
        }

        [Language("test")]
        public class TestLanguageService : ILanguageService
        {
            public Task<object> ExecuteAsync(
                IScript script,
                IScriptGlobals scriptGlobals = null,
                IExpando args = null,
                IContext executionContext = null,
                CancellationToken cancellationToken = default)
            {
                return Task.FromResult<object>(script.SourceCode);
            }
        }
    }
}