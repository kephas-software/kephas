// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RazorTemplatingEngineIntegrationTest.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Templating.Razor.Tests;

using System.Reflection;
using Kephas.Operations;
using Kephas.Services;
using NUnit.Framework;

[TestFixture]
public class RazorTemplatingEngineIntegrationTest : RazorTemplatingTestBase
{
    [TestCase("simple.cshtml", "Hello!", "<div>Hello!</div>")]
    [TestCase("functions.cshtml", "Hello!", "<div>@Hello!@</div>\r\n")]
    public async Task ProcessAsync(string templatePath, string model, string expected)
    {
        var injector = this.CreateInjector();
        var engine = injector.Resolve<ITemplatingEngine>("Razor");
        var contextFactory = injector.Resolve<IContextFactory>();
        using var context = contextFactory.CreateContext<TemplateProcessingContext>();
        var result = await engine.ProcessAsync(this.GetTemplate(templatePath), model, context);

        Assert.IsFalse(result.HasErrors(), "No rendering errors expected!");
        Assert.AreEqual(expected, result.Value);
    }

    private ITemplate GetTemplate(string templatePath)
    {
        return new StreamTemplate(Assembly.GetExecutingAssembly().GetManifestResourceStream($"Kephas.Templating.Razor.Tests.Templates.{templatePath}"));
    }
}