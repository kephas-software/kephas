// --------------------------------------------------------------------------------------------------------------------
// <copyright file="StringExtensionsTest.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the string extensions test class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Core.Tests.Text
{
    using System.Linq;
    using Kephas;

    using NUnit.Framework;

    [TestFixture]
    public class StringExtensionsTest
    {
        [Test]
        [TestCase("123456'123456'", new[] { "123456'123456'" })]
        [TestCase("123 456 '123 456'", new[] { "123", "456", "'123 456'" })]
        [TestCase("123 456 '123 '456'", new[] { "123", "456", "'123 '456'" })]
        public void Split_with_function(string str, string[] expected)
        {
            var inQuotes = false;
            var splits = str.Split(
                c =>
                    {
                        if (c == '\'')
                        {
                            inQuotes = !inQuotes;
                        }

                        return !inQuotes && c == ' ';
                    }).ToArray();

            Assert.AreEqual(expected.Length, splits.Length);
            for (var i = 0; i < expected.Length; i++)
            {
                Assert.AreEqual(expected[i], splits[i]);
            }
        }

        [Test]
        [TestCase("123456'123456'", new[] { "123456'123456'" })]
        [TestCase("123 456 '123 456'", new[] { "123", "456", "'123 456'" })]
        [TestCase("123 456 '123 '456'", new[] { "123", "456", "'123 '456'" })]
        [TestCase(" 123   456   '123   456'  ", new[] { "123", "456", "'123   456'" })] // remove empty args
        public void Split_with_quotes(string str, string[] expected)
        {
            var splits = str.Split(new[] { ' ' }, new[] { '\'' }).ToArray();

            Assert.AreEqual(expected.Length, splits.Length);
            for (var i = 0; i < expected.Length; i++)
            {
                Assert.AreEqual(expected[i], splits[i]);
            }
        }

        [Test]
        [TestCase("123, \"456,34\"; '123 456'", new[] { ',', ';', ' ' }, new[] { '\'', '"' }, new[] { "123", "\"456,34\"", "'123 456'" })]
        [TestCase("123, \"456,34'\"; '123\" 456'", new[] { ',', ';', ' ' }, new[] { '\'', '"' }, new[] { "123", "\"456,34'\"", "'123\" 456'" })]
        public void Split_with_multiple_chars(string str, char[] separator, char[] quote, string[] expected)
        {
            var splits = str.Split(separator, quote).ToArray();

            Assert.AreEqual(expected.Length, splits.Length);
            for (var i = 0; i < expected.Length; i++)
            {
                Assert.AreEqual(expected[i], splits[i]);
            }
        }
    }
}