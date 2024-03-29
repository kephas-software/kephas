﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PythonLanguageServiceTest.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the C# language service test class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Scripting.Python.Tests
{
    using System;
    using System.Collections;
    using System.Reflection;
    using System.Threading.Tasks;

    using Kephas.Configuration;
    using Kephas.Dynamic;
    using Kephas.IO;
    using Kephas.Reflection;
    using Kephas.Services;
    using NSubstitute;
    using NUnit.Framework;

    [TestFixture]
    public class PythonLanguageServiceTest : PythonScriptingTestBase
    {
        [Test]
        public void Injection()
        {
            var injector = this.CreateServicesBuilder().BuildWithDependencyInjection();
            var engine = injector.Resolve<ILanguageService>(PythonLanguageService.Language);

            Assert.IsInstanceOf<PythonLanguageService>(engine);
        }

        [Test]
        public async Task ExecuteAsync_simple()
        {
            var langService = new PythonLanguageService();
            var script = new PythonStringScript("(1 + 2) * 3");
            var result = await langService.ExecuteAsync(script);

            Assert.AreEqual(9, result);
        }

        [Test]
        public void Execute_simple()
        {
            var langService = new PythonLanguageService();
            var script = new PythonStringScript("(1 + 2) * 3");
            var result = langService.Execute(script);

            Assert.AreEqual(9, result);
        }

        [Test]
        public async Task ExecuteAsync_DateTime()
        {
            var langService = new PythonLanguageService();
            var script = new PythonStringScript("from System import DateTime\nDateTime(2000, 12, 25)");
            var result = await langService.ExecuteAsync(script);

            Assert.AreEqual(new DateTime(2000, 12, 25), result);
        }

        [Test]
        public void Execute_DateTime()
        {
            var langService = new PythonLanguageService();
            var script = new PythonStringScript("from System import DateTime\nDateTime(2000, 12, 25)");
            var result = langService.Execute(script);

            Assert.AreEqual(new DateTime(2000, 12, 25), result);
        }

        [Test]
        public async Task ExecuteAsync_function()
        {
            var langService = new PythonLanguageService();
            var script = new PythonStringScript(
                "def Power(a):" + Environment.NewLine +
                "  return a * a" + Environment.NewLine +
                "Power(2) + 3");
            var result = await langService.ExecuteAsync(script);

            Assert.AreEqual(7, result);
        }

        [Test]
        public void Execute_function()
        {
            var langService = new PythonLanguageService();
            var script = new PythonStringScript(
                "def Power(a):" + Environment.NewLine +
                "  return a * a" + Environment.NewLine +
                "Power(2) + 3");
            var result = langService.Execute(script);

            Assert.AreEqual(7, result);
        }

        [Test]
        public async Task ExecuteAsync_lambda()
        {
            var langService = new PythonLanguageService();
            var script = new PythonStringScript(
                "power(2) + 3");
            var globals = new ScriptGlobals
            {
                ["Power"] = (Func<int, int>)(s => s * s),
            };
            var result = await langService.ExecuteAsync(script, globals);

            Assert.AreEqual(7, result);
        }

        [Test]
        public void Execute_lambda()
        {
            var langService = new PythonLanguageService();
            var script = new PythonStringScript(
                "power(2) + 3");
            var globals = new ScriptGlobals
            {
                ["Power"] = (Func<int, int>)(s => s * s),
            };
            var result = langService.Execute(script, globals);

            Assert.AreEqual(7, result);
        }

        [Test]
        public async Task ExecuteAsync_args()
        {
            var langService = new PythonLanguageService();
            var script = new PythonStringScript(
                "def Power(a):" + Environment.NewLine +
                "  return a * a" + Environment.NewLine +
                "returnValue = Power(int(args.a))");
            var args = new Expando
                           {
                               ["a"] = 2,
                           };
            var result = await langService.ExecuteAsync(script, new ScriptGlobals(args, false));

            Assert.AreEqual(4, result);
        }

        [Test]
        public void Execute_args()
        {
            var langService = new PythonLanguageService();
            var script = new PythonStringScript(
                "def Power(a):" + Environment.NewLine +
                "  return a * a" + Environment.NewLine +
                "returnValue = Power(int(args.a))");
            var args = new Expando
            {
                ["a"] = 2,
            };
            var result = langService.Execute(script, new ScriptGlobals(args, false));

            Assert.AreEqual(4, result);
        }

        [TestCase(6, new[] { 1, 1, 2, 3, 5 })]
        [TestCase(15, new[] { 1, 1, 2, 3, 5, 8, 13 })]
        public void Execute_load_globals(int n, int[] expected)
        {
            var locationsManager = new FolderLocationsManager(Assembly.GetExecutingAssembly().GetLocationDirectory());
            var configuration = Substitute.For<IConfiguration<PythonSettings>>();
            configuration.GetSettings(Arg.Any<IContext>())
                .Returns(new PythonSettings { SearchPaths = new[] { "GlobalScripts" }, PreloadGlobalModules = true });
            var langService = new PythonLanguageService(configuration, locationsManager: locationsManager);
            var script = new PythonStringScript("fibo.fib(n)");
            var args = new { n };
            var result = langService.Execute(script, new ScriptGlobals(args.ToDynamic())) as IEnumerable;

            CollectionAssert.AreEqual(expected, result?.OfType<int>().ToArray());
        }
    }
}