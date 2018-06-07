// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CSharpScriptInterpreterTest.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the C# script interpreter test class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Scripting.CSharp.Tests
{
    using System;
    using System.Threading.Tasks;

    using Kephas.Dynamic;

    using NUnit.Framework;

    [TestFixture]
    public class CSharpScriptInterpreterTest
    {
        [Test]
        public async Task ExecuteAsync_simple()
        {
            var interpreter = new CSharpScriptInterpreter();
            var script = new Script(CSharpScriptInterpreter.Language, "(1 + 2) * 3");
            var result = await interpreter.ExecuteAsync(script);

            Assert.AreEqual(9, result);
        }

        [Test]
        public async Task ExecuteAsync_function()
        {
            var interpreter = new CSharpScriptInterpreter();
            var script = new Script(
                CSharpScriptInterpreter.Language,
                "int Power(int a) => a * a;" + 
                "return Power(2);");
            var result = await interpreter.ExecuteAsync(script);

            Assert.AreEqual(4, result);
        }

        [Test]
        public async Task ExecuteAsync_args()
        {
            var interpreter = new CSharpScriptInterpreter();
            var script = new Script(
                CSharpScriptInterpreter.Language,
                "int Power(int a) => a * a;" + 
                "return Power((int)args[\"a\"]);");
            var args = new Expando
                           {
                               ["a"] = 2,
                           };
            var result = await interpreter.ExecuteAsync(script, args);

            Assert.AreEqual(4, result);
        }

        [Test]
        [Ignore("Check how to add support for dynamic")]
        public async Task ExecuteAsync_args_dynamic()
        {
            var interpreter = new CSharpScriptInterpreter();
            var script = new Script(
                CSharpScriptInterpreter.Language,
                "#r \"System.Dynamic\"" + Environment.NewLine +
                "#r \"Microsoft.CSharp\"" + Environment.NewLine +
                "int Power(int a) => a * a;" + 
                "dynamic dargs = args;" + 
                "return Power(dargs.a);");
            var args = new Expando
                           {
                               ["a"] = 2,
                           };
            var result = await interpreter.ExecuteAsync(script, args);

            Assert.AreEqual(4, result);
        }
    }
}