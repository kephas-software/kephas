// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MefDefaultScriptProcessorTest.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the default scripting engine test class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Scripting.Tests.Composition.Mef
{
    using System.Threading.Tasks;

    using NUnit.Framework;

    [TestFixture]
    public class MefDefaultScriptProcessorTest : MefScriptingTestBase
    {
        [Test]
        public void DefaultMessageProcessor_Composition_success()
        {
            var container = CreateContainer();
            var scriptingEngine = container.GetExport<IScriptProcessor>();
            Assert.IsInstanceOf<DefaultScriptProcessor>(scriptingEngine);

            var typedScriptingService = (DefaultScriptProcessor)scriptingEngine;
            Assert.IsNotNull(typedScriptingService.Logger);
        }

        [Test]
        public async Task ExecuteAsync_composition_success()
        {
            var container = CreateContainer(parts: new[] { typeof(TestLanguageService) });
            var scriptingEngine = container.GetExport<IScriptProcessor>();

            var script = new Script("test", "dummy");
            var result = await scriptingEngine.ExecuteAsync(script);

            Assert.AreEqual("executed dummy", result);
        }
    }
}