// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LuaLanguageServiceTest.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the C# language service test class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Scripting.Lua.Tests
{
    using System;
    using System.Threading.Tasks;

    using Kephas.Dynamic;

    using NUnit.Framework;

    [TestFixture]
    public class LuaLanguageServiceTest
    {
        [Test]
        public async Task ExecuteAsync_simple()
        {
            var langService = new LuaLanguageService();
            var script = new Script(LuaLanguageService.Language, "return (1 + 2) * 3");
            var result = await langService.ExecuteAsync(script);

            Assert.AreEqual(9, result);
        }

        [Test]
        public void Execute_simple()
        {
            var langService = new LuaLanguageService();
            var script = new Script(LuaLanguageService.Language, "return (1 + 2) * 3");
            var result = langService.Execute(script);

            Assert.AreEqual(9, result);
        }

        [Test]
        public async Task ExecuteAsync_function()
        {
            var langService = new LuaLanguageService();
            var script = new Script(
                LuaLanguageService.Language,
                "function Power(a)" + Environment.NewLine +
                "  return a * a" + Environment.NewLine +
                "end" + Environment.NewLine +
                "return Power(2) + 3");
            var result = await langService.ExecuteAsync(script);

            Assert.AreEqual(7, result);
        }

        [Test]
        public void Execute_function()
        {
            var langService = new LuaLanguageService();
            var script = new Script(
                LuaLanguageService.Language,
                "function Power(a)" + Environment.NewLine +
                "  return a * a" + Environment.NewLine +
                "end" + Environment.NewLine +
                "return Power(2) + 3");
            var result = langService.Execute(script);

            Assert.AreEqual(7, result);
        }

        [Test]
        public async Task ExecuteAsync_lambda()
        {
            var langService = new LuaLanguageService();
            var script = new Script(
                LuaLanguageService.Language,
                "return power(2) + 3");
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
            var langService = new LuaLanguageService();
            var script = new Script(
                LuaLanguageService.Language,
                "return power(2) + 3");
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
            var langService = new LuaLanguageService();
            var script = new Script(
                LuaLanguageService.Language,
                "function Power(a)" + Environment.NewLine +
                "  return a * a" + Environment.NewLine +
                "end" + Environment.NewLine +
                "return Power(args.a)");
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
            var langService = new LuaLanguageService();
            var script = new Script(
                LuaLanguageService.Language,
                "function Power(a)" + Environment.NewLine +
                "  return a * a" + Environment.NewLine +
                "end" + Environment.NewLine +
                "return Power(args.a)");
            var args = new Expando
            {
                ["a"] = 2,
            };
            var result = langService.Execute(script, new ScriptGlobals { Args = args });

            Assert.AreEqual(4, result);
        }
    }
}