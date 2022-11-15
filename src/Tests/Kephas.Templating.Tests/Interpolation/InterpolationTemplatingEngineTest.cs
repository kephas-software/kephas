// --------------------------------------------------------------------------------------------------------------------
// <copyright file="InterpolationTemplatingEngineTest.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Templating.Tests.Interpolation;

using Kephas.Injection;
using Kephas.Templating.Interpolation;
using NSubstitute;
using NUnit.Framework;

[TestFixture]
public class InterpolationTemplatingEngineTest
{
    [TestCaseSource(nameof(GetInterpolationCases))]
    public async Task ProcessAsync_success(string template, object? model, string result)
    {
        var engine = new InterpolationTemplatingEngine();
        using var sbWriter = new StringWriter();
        var opResult = await engine.ProcessAsync(new StringTemplate(template), model, sbWriter, CreateProcessingContext());

        Assert.AreEqual(result, sbWriter.GetStringBuilder().ToString());
    }

    [Test]
    public async Task ProcessAsync_null_model()
    {
        var engine = new InterpolationTemplatingEngine();
        using var sbWriter = new StringWriter();
        var opResult = await engine.ProcessAsync<object?>(new StringTemplate("hello {boy}!"), null, sbWriter, CreateProcessingContext());

        Assert.AreEqual("hello !", sbWriter.GetStringBuilder().ToString());
    }

    public static object[] GetInterpolationCases()
    {
        return new object[] {
            new object[] { "hello {there}, boy?", new { there = "John Doe" }, "hello John Doe, boy?" },
            new object[] { "{hello} there", new { hello = "hi" }, "hi there" },
            new object[] { "hello {there}", new { there = 12 }, "hello 12" },
        };
    }

    private static ITemplateProcessingContext CreateProcessingContext()
    {
        return Substitute.For<ITemplateProcessingContext>();
    }
}