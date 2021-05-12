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
    using System;
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

        [Test]
        [TestCase("1{0}4{1}7{2}", new[] { "23", "56", "89" }, "123456789")]
        [TestCase("1{0}2{1}3{2}4", new[] { " x ", " y ", " z " }, "1 x 2 y 3 z 4")]
        [TestCase("123456789", new[] { " x ", " y ", " z " }, "123456789")]
        public void FormatWith_success(string format, string[] args, string expected)
        {
            var formatted = StringExtensions.FormatWith(format, args);

            Assert.AreEqual(expected.Length, formatted.Length);
            var i = 0;
            foreach (var expectedValue in expected)
            {
                Assert.AreEqual(expectedValue, formatted[i++]);
            }
        }

        [Test]
        public void FormatWith_format_null()
        {
            Assert.Throws<ArgumentNullException>(() => StringExtensions.FormatWith(null, new string[0]));
        }

        [Test]
        [TestCase(",", new[] { "123", "456", "789" }, "123,456,789")]
        [TestCase("; ", new[] { "123", "456", "789" }, "123; 456; 789")]
        [TestCase("x", new[] { "123456789" }, "123456789")]
        [TestCase("xx", new[] { "123 ", " 456 ", " 789" }, "123 xx 456 xx 789")]

        public void JoinWith_success(string separator, string[] args, string expected)
        {
            var joined = StringExtensions.JoinWith(args, separator);

            Assert.AreEqual(expected.Length, joined.Length);
            var i = 0;
            foreach (var expectedValue in expected)
            {
                Assert.AreEqual(expectedValue, joined[i++]);
            }

            //i = 0;
            //var badValues = from v in expected
            //                where v != joined[i++]
            //                select v + "ok";
            //badValues = expected.Where(v => v != joined[i++]).Select(v => v + "ok");
        }

        [Test]
        public void JoinWith_join_null()
        {
            Assert.Throws<ArgumentNullException>(() => StringExtensions.JoinWith(null, "123"));
        }
    }
}