// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SystemCompositionDefaultScriptProcessorTest.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the default scripting engine test class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Scripting.Tests.Injection.SystemComposition
{
    using System.Threading.Tasks;
    using NUnit.Framework;

    [TestFixture]
    public class SystemCompositionDefaultScriptProcessorTest : SystemCompositionScriptingTestBase
    {
        [Test]
        public void DefaultScriptProcessor_Injection_success()
        {
            var container = CreateInjector();
            var scriptingEngine = container.Resolve<IScriptProcessor>();
            Assert.IsInstanceOf<DefaultScriptProcessor>(scriptingEngine);

            var typedScriptingService = (DefaultScriptProcessor)scriptingEngine;
            Assert.IsNotNull(typedScriptingService.Logger);
        }

        [Test]
        public async Task ExecuteAsync_Injection_success()
        {
            var container = CreateInjector(parts: new[] { typeof(TestLanguageService) });
            var scriptingEngine = container.Resolve<IScriptProcessor>();

            var script = new Script("test", "dummy");
            var result = await scriptingEngine.ExecuteAsync(script);

            Assert.AreEqual("executed dummy", result);
        }
    }
}