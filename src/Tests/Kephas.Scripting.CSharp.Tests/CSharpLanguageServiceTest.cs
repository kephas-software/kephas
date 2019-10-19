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
    using System.Threading.Tasks;

    using Kephas.Dynamic;

    using NUnit.Framework;

    [TestFixture]
    public class CSharpLanguageServiceTest
    {
        [Test]
        public async Task ExecuteAsync_simple()
        {
            var langService = new CSharpLanguageService();
            var script = new Script(CSharpLanguageService.Language, "(1 + 2) * 3");
            var result = await langService.ExecuteAsync(script);

            Assert.AreEqual(9, result);
        }

        [Test]
        public void Execute_simple()
        {
            var langService = new CSharpLanguageService();
            var script = new Script(CSharpLanguageService.Language, "(1 + 2) * 3");
            var result = langService.Execute(script);

            Assert.AreEqual(9, result);
        }

        [Test]
        public async Task ExecuteAsync_function()
        {
            var langService = new CSharpLanguageService();
            var script = new Script(
                CSharpLanguageService.Language,
                "int Power(int a) => a * a;" +
                "return Power(2);");
            var result = await langService.ExecuteAsync(script);

            Assert.AreEqual(4, result);
        }

        [Test]
        public void Execute_function()
        {
            var langService = new CSharpLanguageService();
            var script = new Script(
                CSharpLanguageService.Language,
                "int Power(int a) => a * a;" +
                "return Power(2);");
            var result = langService.Execute(script);

            Assert.AreEqual(4, result);
        }

        [Test]
        public async Task ExecuteAsync_lambda()
        {
            var langService = new CSharpLanguageService();
            var script = new Script(
                CSharpLanguageService.Language,
                "return globals.Power(2);");
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
            var script = new Script(
                CSharpLanguageService.Language,
                "return globals.Power(2);");
            var globals = new ScriptGlobals
            {
                ["Power"] = (Func<int, int>)(s => s * s),
            };
            var result = langService.Execute(script, globals);

            Assert.AreEqual(4, result);
        }

        [Test]
        public async Task ExecuteAsync_args()
        {
            var langService = new CSharpLanguageService();
            var script = new Script(
                CSharpLanguageService.Language,
                "int Power(int a) => a * a;" +
                "return Power((int)globals.Args[\"a\"]);");
            var args = new Expando
            {
                ["a"] = 2,
            };
            var result = await langService.ExecuteAsync(script, new ScriptGlobals { Args = args });

            Assert.AreEqual(4, result);
        }

        [Test]
        public void Execute_args()
        {
            var langService = new CSharpLanguageService();
            var script = new Script(
                CSharpLanguageService.Language,
                "int Power(int a) => a * a;" +
                "return Power((int)globals.Args[\"a\"]);");
            var args = new Expando
            {
                ["a"] = 2,
            };
            var result = langService.Execute(script, new ScriptGlobals { Args = args });

            Assert.AreEqual(4, result);
        }

        [Test]
        [Ignore("Check how to add support for dynamic")]
        public async Task ExecuteAsync_args_dynamic()
        {
            var langService = new CSharpLanguageService();
            var script = new Script(
                CSharpLanguageService.Language,
                "#r \"System.Dynamic\"" + Environment.NewLine +
                "#r \"Microsoft.CSharp\"" + Environment.NewLine +
                "int Power(int a) => a * a;" +
                "dynamic dargs = globals.Args;" +
                "return Power(dargs.a);");
            var args = new Expando
            {
                ["a"] = 2,
            };
            var result = await langService.ExecuteAsync(script, new ScriptGlobals { Args = args });

            Assert.AreEqual(4, result);
        }
    }
}