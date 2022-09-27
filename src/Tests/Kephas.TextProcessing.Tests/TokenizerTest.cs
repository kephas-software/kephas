// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TokenizerTest.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the tokenizer test class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.TextProcessing.Tests
{
    using System.Linq;

    using NUnit.Framework;

    [TestFixture]
    public class TokenizerTest : TextProcessingTestBase
    {
        [Test]
        public void Injection()
        {
            var container = this.BuildServiceProvider();
            var tokenizer = container.Resolve<ITokenizer>();

            Assert.IsInstanceOf<Tokenizer>(tokenizer);
        }

        [TestCase("hello there", new[] { "hello" })]
        [TestCase("Please, come home quickly!", new[] { "Please", "come", "home", "quickly" })]
        [TestCase("The \"stereotype\" is 'key'!", new[] { "stereotype", "key" })]
        public void Tokenize_simple(string text, string[] expectedTokens)
        {
            var container = this.BuildServiceProvider();
            var tokenizer = container.Resolve<ITokenizer>();

            var tokens = tokenizer.Tokenize(text).ToArray();

            CollectionAssert.AreEqual(expectedTokens, tokens);
        }
    }
}
