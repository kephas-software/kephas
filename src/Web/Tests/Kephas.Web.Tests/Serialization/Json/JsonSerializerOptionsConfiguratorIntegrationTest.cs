// --------------------------------------------------------------------------------------------------------------------
// <copyright file="JsonSerializerOptionsConfiguratorIntegrationTest.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System.Text.Json;
using System.Text.Json.Serialization;
using Kephas.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Xunit;

namespace Kephas.Web.Tests.Serialization.Json;

public class JsonSerializerOptionsConfiguratorIntegrationTest : IntegrationTestBase<JsonSerializerOptionsConfiguratorIntegrationTest>
{
    [Fact]
    public void JsonSerializerOptions_FilledByConfigurator()
    {
        var options = ServiceProvider.GetRequiredService<IOptions<JsonSerializerOptions>>().Value;
        Assert.Equal(JsonNumberHandling.AllowReadingFromString, options.NumberHandling);
        Assert.Equal(JsonIgnoreCondition.WhenWritingNull, options.DefaultIgnoreCondition);
        Assert.True(options.AllowTrailingCommas);
    }
}