// --------------------------------------------------------------------------------------------------------------------
// <copyright file="StringExtensionsTest.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using FluentAssertions;
using Xunit;

namespace Kephas.Tests;

public class StringExtensionsTest
{
    [Theory]
    [InlineData("123456'123456'", new[] { "123456'123456'" })]
    [InlineData("123 456 '123 456'", new[] { "123", "456", "'123 456'" })]
    [InlineData("123 456 '123 '456'", new[] { "123", "456", "'123 '456'" })]
    public void SplitWith_function(string str, string[] expected)
    {
        var inQuotes = false;
        var splits = str.SplitWith(
            c =>
            {
                if (c == '\'')
                {
                    inQuotes = !inQuotes;
                }

                return !inQuotes && c == ' ';
            }).ToArray();

        splits.Should().BeEquivalentTo(expected);
    }

    [Theory]
    [InlineData("123456'123456'", new[] { "123456'123456'" })]
    [InlineData("123 456 '123 456'", new[] { "123", "456", "'123 456'" })]
    [InlineData("123 456 '123 '456'", new[] { "123", "456", "'123 '456'" })]
    [InlineData(" 123   456   '123   456'  ", new[] { "123", "456", "'123   456'" })] // remove empty args
    public void SplitWith_quotes(string str, string[] expected)
    {
        var splits = str.SplitWith([' '], ['\'']).ToArray();

        splits.Should().BeEquivalentTo(expected);
    }

    [Theory]
    [InlineData("123, \"456,34\"; '123 456'", new[] { ',', ';', ' ' }, new[] { '\'', '"' }, new[] { "123", "\"456,34\"", "'123 456'" })]
    [InlineData("123, \"456,34'\"; '123\" 456'", new[] { ',', ';', ' ' }, new[] { '\'', '"' }, new[] { "123", "\"456,34'\"", "'123\" 456'" })]
    public void SplitWith_multiple_chars(string str, char[] separator, char[] quote, string[] expected)
    {
        var splits = str.SplitWith(separator, quote).ToArray();

        splits.Should().BeEquivalentTo(expected);
    }

    [Theory]
    [InlineData("1{0}4{1}7{2}", new[] { "23", "56", "89" }, "123456789")]
    [InlineData("1{0}2{1}3{2}4", new[] { " x ", " y ", " z " }, "1 x 2 y 3 z 4")]
    [InlineData("123456789", new[] { " x ", " y ", " z " }, "123456789")]
    public void FormatWith_success(string format, object[] args, string expected)
    {
        var formatted = format.FormatWith(args);

        formatted.Should().Be(expected);
    }

    [Fact]
    public void FormatWith_format_null()
    {
        Assert.Throws<ArgumentNullException>(() => StringExtensions.FormatWith(null!, []));
    }

    [Theory]
    [InlineData(",", new[] { "123", "456", "789" }, "123,456,789")]
    [InlineData("; ", new[] { "123", "456", "789" }, "123; 456; 789")]
    [InlineData("x", new[] { "123456789" }, "123456789")]
    [InlineData("xx", new[] { "123 ", " 456 ", " 789" }, "123 xx 456 xx 789")]

    public void JoinWith_success(string separator, string[] args, string expected)
    {
        var joined = args.JoinWith(separator);

        joined.Should().Be(expected);
    }

    [Fact]
    public void JoinWith_join_null()
    {
        Assert.Throws<ArgumentNullException>(() => StringExtensions.JoinWith(null, "123"));
    }

    [Theory]
    [InlineData("123 456 '123 456'", new[] { "123", "456", "'123 456'" })]
    [InlineData("123 456 '123 '456'", new[] { "123", "456", "'123 '456'" })]
    [InlineData(" 123   456   '123   456'  ", new[] { "123", "456", "'123   456'" })]
    [InlineData("", new string[0])]
    [InlineData("   ", new string[0])]
    public void SplitWith_quotes_edge_cases(string str, string[] expected)
    {
        var splits = str.SplitWith(new[] { ' ' }, new[] { '\'' }).ToArray();

        splits.Should().BeEquivalentTo(expected);
    }

    [Theory]
    [InlineData("1{0}4{1}7{2}", new[] { "23", "56" }, "1{0}4{1}7{2}")]
    [InlineData("1{0}2{1}3{2}4", new string[0], "1{0}2{1}3{2}4")]
    public void FormatWith_edge_cases(string format, object?[] args, string expected)
    {
        var formatted = format.FormatWith(args);
        formatted.Should().Be(expected);
    }

    [Theory]
    [InlineData(",", new string[0], "")]
    [InlineData("; ", new string[0], "")]
    [InlineData("x", new string[0], "")]
    [InlineData("xx", new string[0], "")]
    public void JoinWith_edge_cases(string separator, string[] args, string expected)
    {
        var joined = args.JoinWith(separator);

        joined.Should().Be(expected);
    }
    
    [Theory]
    [InlineData("helloWorld", "HelloWorld")]
    [InlineData("HelloWorld", "HelloWorld")]
    [InlineData("hello_world", "HelloWorld")]
    [InlineData("hello-world", "HelloWorld")]
    [InlineData("HELLO_WORLD", "HELLOWORLD")]
    [InlineData("", "")]
    [InlineData(null, null)]
    public void ToPascalCase_ShouldConvertToPascalCase(string? input, string? expected)
    {
        var result = input.ToPascalCase();
        Assert.Equal(expected, result);
    }

    [Theory]
    [InlineData("HelloWorld", "helloWorld")]
    [InlineData("helloWorld", "helloWorld")]
    [InlineData("Hello_World", "hello_World")]
    [InlineData("Hello-World", "hello-World")]
    [InlineData("HELLO_WORLD", "HELLO_WORLD")]
    [InlineData("", "")]
    [InlineData(null, null)]
    public void ToCamelCase_ShouldConvertToCamelCase(string? input, string? expected)
    {
        var result = input.ToCamelCase();
        Assert.Equal(expected, result);
    }
}