// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RazorTemplatingTestBase.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Templating.Razor.Tests;

using System.Reflection;
using Kephas.Testing.Injection;

public abstract class RazorTemplatingTestBase  : TestBase
{
    protected override IEnumerable<Assembly> GetAssemblies()
    {
        return new List<Assembly>(base.GetAssemblies())
        {
            typeof(ITemplatingEngine).Assembly, // Kephas.Templating
            typeof(RazorTemplatingEngine).Assembly, // Kephas.Templating.Razor
        };
    }
}