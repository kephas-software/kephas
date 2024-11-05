// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SemanticVersionTest.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the semantic version test class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Tests.Versioning;

using System.Linq;
using Kephas.Versioning;
using Xunit;

public class SemanticVersionTest
{
    [Fact]
    public void Parse_complete()
    {
        var version = SemanticVersion.Parse("2.1.4-dev.12+1234");

        Assert.Equal(2, version.Major);
        Assert.Equal(1, version.Minor);
        Assert.Equal(4, version.Patch);
        Assert.Equal("dev", version.ReleaseLabels.First());
        Assert.Equal("12", version.ReleaseLabels.Skip(1).First());
        Assert.Equal("1234", version.Metadata);
    }

    [Fact]
    public void Parse_hotfix_version_success()
    {
        var version = SemanticVersion.Parse("2.1.4.0");

        Assert.Equal(2, version.Major);
        Assert.Equal(1, version.Minor);
        Assert.Equal(4, version.Patch);
        Assert.Null(version.Hotfix);
        Assert.Equal("2.1.4", version.ToString());
    }

    [Fact]
    public void TryParse_hotfix_version_success()
    {
        var parsed = SemanticVersion.TryParse("2.1.4.0", out var version);

        Assert.True(parsed);
        Assert.NotNull(version);
        Assert.Equal(2, version.Major);
        Assert.Equal(1, version.Minor);
        Assert.Equal(4, version.Patch);
        Assert.Null(version.Hotfix);
        Assert.Equal("2.1.4", version.ToString());
    }

    [Fact]
    public void Parse_hotfix_version_non_null()
    {
        var version = SemanticVersion.Parse("2.1.4.5");

        Assert.Equal(2, version.Major);
        Assert.Equal(1, version.Minor);
        Assert.Equal(4, version.Patch);
        Assert.Equal(5, version.Hotfix);
        Assert.Equal("2.1.4.5", version.ToString());
    }

    [Fact]
    public void TryParse_hotfix_version_non_null()
    {
        var parsed = SemanticVersion.TryParse("2.1.4.5", out var version);

        Assert.True(parsed);
        Assert.NotNull(version);
        Assert.Equal(2, version.Major);
        Assert.Equal(1, version.Minor);
        Assert.Equal(4, version.Patch);
        Assert.Equal(5, version.Hotfix);
        Assert.Equal("2.1.4.5", version.ToString());
    }

    [Fact]
    public void CompareTo_release_greater()
    {
        var version = SemanticVersion.Parse("2.1.4-dev.122");
        var version2 = SemanticVersion.Parse("2.1.4-dev.21");

        Assert.True(version.CompareTo(version2) > 0);
    }

    [Fact]
    public void GreaterThan_release()
    {
        var version = SemanticVersion.Parse("2.1.4-dev.122");
        var version2 = SemanticVersion.Parse("2.1.4-dev.21");

        Assert.True(version > version2);
    }

    [Fact]
    public void GreaterThan_hotfix()
    {
        var version = SemanticVersion.Parse("2.1.4.5");
        var version2 = SemanticVersion.Parse("2.1.4");

        Assert.True(version > version2);
    }

    [Fact]
    public void LessThan_release()
    {
        var version = SemanticVersion.Parse("2.1.4-dev.122");
        var version2 = SemanticVersion.Parse("2.1.4-dev.21");

        Assert.True(version2 < version);
    }

    [Fact]
    public void LessThan_hotfix()
    {
        var version = SemanticVersion.Parse("2.1.4.5-dev.122");
        var version2 = SemanticVersion.Parse("2.1.4-dev.222");

        Assert.True(version2 < version);
    }

    [Fact]
    public void Equals_hotfix()
    {
        var version = SemanticVersion.Parse("2.1.4");
        var version2 = SemanticVersion.Parse("2.1.4.0");

        Assert.True(version2 == version);
    }
}