// --------------------------------------------------------------------------------------------------------------------
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
    using System.Threading.Tasks;

    using Kephas.Dynamic;

    using NUnit.Framework;

    [TestFixture]
    public class PythonLanguageServiceTest
    {
        [Test]
        public async Task ExecuteAsync_simple()
        {
            var langService = new PythonLanguageService();
            var script = new Script(PythonLanguageService.Language, "(1 + 2) * 3");
            var result = await langService.ExecuteAsync(script);

            Assert.AreEqual(9, result);
        }

        [Test]
        public async Task ExecuteAsync_function()
        {
            var langService = new PythonLanguageService();
            var script = new Script(
                PythonLanguageService.Language,
                "def Power(a):" + Environment.NewLine +
                "  return a * a" + Environment.NewLine +
                "Power(2) + 3");
            var result = await langService.ExecuteAsync(script);

            Assert.AreEqual(7, result);
        }

        [Test]
        public async Task ExecuteAsync_args()
        {
            var langService = new PythonLanguageService();
            var script = new Script(
                PythonLanguageService.Language,
                "def Power(a):" + Environment.NewLine +
                "  return a * a" + Environment.NewLine +
                "returnValue = Power(int(args.a))");
            var args = new Expando
                           {
                               ["a"] = 2,
                           };
            var result = await langService.ExecuteAsync(script, new ScriptGlobals { Args = args });

            Assert.AreEqual(4, result);
        }
    }
}