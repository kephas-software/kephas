﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AutofacDefaultScriptProcessorTest.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the default scripting engine test class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Scripting.Tests.Autofac
{
    using System.Threading.Tasks;

    using Kephas.Testing;
    using NUnit.Framework;

    [TestFixture]
    public class AutofacDefaultScriptProcessorTest : TestBase
    {
        [Test]
        public void DefaultMessageProcessor_Injection_success()
        {
            var container = this.CreateServicesBuilder()
                .BuildWithAutofac();
            var scriptingEngine = container.Resolve<IScriptProcessor>();
            Assert.IsInstanceOf<DefaultScriptProcessor>(scriptingEngine);

            var typedScriptingService = (DefaultScriptProcessor)scriptingEngine;
            Assert.IsNotNull(typedScriptingService.Logger);
        }

        [Test]
        public async Task ExecuteAsync_Injection_success()
        {
            var container = this.CreateServicesBuilder()
                .WithParts(typeof(TestLanguageService))
                .BuildWithAutofac();
            var scriptingEngine = container.Resolve<IScriptProcessor>();

            var script = new StringScript("dummy", "test");
            var result = await scriptingEngine.ExecuteAsync(script);

            Assert.AreEqual("executed dummy", result);
        }
    }
}