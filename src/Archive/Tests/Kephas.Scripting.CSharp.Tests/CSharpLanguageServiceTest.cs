// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CSharpLanguageServiceTest.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the C# language service test class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Scripting.CSharp.Tests
{
    using System;
    using System.Reflection;
    using System.Threading.Tasks;

    using Kephas.Dynamic;
    using Kephas.Scripting;
    using NUnit.Framework;

    [TestFixture]
    public class CSharpLanguageServiceTest
    {
        [Test]
        public async Task ExecuteAsync_simple()
        {
            var langService = new CSharpLanguageService();
            var script = new CSharpStringScript("(1 + 2) * 3");
            var result = await langService.ExecuteAsync(script);

            Assert.AreEqual(9, result);
        }

        [Test]
        public void Execute_simple()
        {
            var langService = new CSharpLanguageService();
            var script = new CSharpStringScript("(1 + 2) * 3");
            var result = ((ILanguageService)langService).Execute(script);

            Assert.AreEqual(9, result);
        }

        [Test]
        public void Execute_simple_csharp10()
        {
            var langService = new CSharpLanguageService();
            var script = new CSharpStringScript("name[..4] + age.ToString()");
            var result = ((ILanguageService)langService).Execute(script, args: new { name = "Johnny", age = 42 }.ToDynamic());

            Assert.AreEqual("John42", result);
        }

        [Test]
        public async Task ExecuteAsync_function()
        {
            var langService = new CSharpLanguageService();
            var script = new CSharpStringScript("int Power(int a) => a * a; return Power(2);");
            var result = await langService.ExecuteAsync(script);

            Assert.AreEqual(4, result);
        }

        [Test]
        public void Execute_function()
        {
            var langService = new CSharpLanguageService();
            var script = new CSharpStringScript("int Power(int a) => a * a; return Power(2);");
            var result = ((ILanguageService)langService).Execute(script);

            Assert.AreEqual(4, result);
        }

        [Test]
        public async Task ExecuteAsync_lambda()
        {
            var langService = new CSharpLanguageService();
            var script = new CSharpStringScript(
                "return Power(2);");
            var globals = new ScriptGlobals
            {
                ["Power"] = (Func<int, int>)(s => s * s),
            };
            var result = await langService.ExecuteAsync(script, globals);

            Assert.AreEqual(4, result);
        }

        [Test]
        public void Execute_lambda()
        {
            var langService = new CSharpLanguageService();
            var script = new CSharpStringScript(
                "return Power(2);");
            var globals = new ScriptGlobals
            {
                ["Power"] = (Func<int, int>)(s => s * s),
            };
            var result = ((ILanguageService)langService).Execute(script, globals);

            Assert.AreEqual(4, result);
        }

        [Test]
        public async Task ExecuteAsync_args()
        {
            var langService = new CSharpLanguageService();
            var script = new CSharpStringScript(
                "int Power(int a) => a * a;" +
                "return Power((int)Args[\"a\"]);");
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
            var langService = new CSharpLanguageService();
            var script = new CSharpStringScript(
                "int Power(int a) => a * a;" +
                "return Power((int)Args[\"a\"]);");
            var args = new Expando
            {
                ["a"] = 2,
            };
            var result = ((ILanguageService)langService).Execute(script, new ScriptGlobals(args, false));

            Assert.AreEqual(4, result);
        }

        [Test, Ignore("dynamic keyword not supported.")]
        public async Task ExecuteAsync_args_dynamic()
        {
            var langService = new CSharpLanguageService();
            var script = this.GetStreamScript("Power.csx");
            var args = new Expando { ["a"] = 2 };
            var result = await langService.ExecuteAsync(script, new ScriptGlobals(args, false));

            Assert.AreEqual(4, result);
        }

#if NET6_0_OR_GREATER
        [TestCase("PointDistance.csx", 2, 3, -1, 5)]
        public async Task ExecuteAsync_stream(string scriptFile, int x1, int y1, int x2, int y2)
        {
            var langService = new CSharpLanguageService();
            var args = new { x1, y1, x2, y2 };
            var result = await langService.ExecuteAsync(this.GetStreamScript(scriptFile), args: args.ToDynamic());

            var expected = (x1 - x2) * (x1 - x2) + (y1 - y2) * (y1 - y2);
            Assert.AreEqual(expected, result);
        }
#endif

        private IScript GetStreamScript(string fileName)
        {
            var assembly = Assembly.GetExecutingAssembly();
            return new CSharpStreamScript(assembly.GetManifestResourceStream($"Kephas.Scripting.CSharp.Tests.Scripts.{fileName}")!);
        }
    }
}