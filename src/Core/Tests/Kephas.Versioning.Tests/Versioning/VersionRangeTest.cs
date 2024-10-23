// --------------------------------------------------------------------------------------------------------------------
// <copyright file="VersionRangeTest.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Tests.Versioning;

using Kephas.Versioning;
using Xunit;

public class VersionRangeTest
{
    [Fact]
    public void Parse_missing_max()
    {
        var range = VersionRange.Parse("2.*:");

        Assert.Null(range.MaxVersion);
        Assert.NotNull(range.MinVersion);
        Assert.Equal(range.MinVersion, SemanticVersion.Parse("2.0.0"));
    }

    [Fact]
    public void Parse_missing_min()
    {
        var range = VersionRange.Parse(":2.*");

        Assert.Null(range.MinVersion);
        Assert.NotNull(range.MaxVersion);
        Assert.Equal(range.MaxVersion, SemanticVersion.Parse($"2.{short.MaxValue}.{short.MaxValue}"));
    }

    [Fact]
    public void Parse_star_min()
    {
        var range = VersionRange.Parse("*:2.*");

        Assert.Null(range.MinVersion);
        Assert.NotNull(range.MaxVersion);
        Assert.Equal(range.MaxVersion, SemanticVersion.Parse($"2.{short.MaxValue}.{short.MaxValue}"));
    }
}