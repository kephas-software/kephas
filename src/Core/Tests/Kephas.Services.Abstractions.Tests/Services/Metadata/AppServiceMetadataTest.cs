// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AppServiceMetadataTest.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Test class for <see cref="AppServiceMetadata" />.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using Kephas.Services;
using Kephas.Services.Metadata;
using Xunit;

namespace Kephas.Tests.Services.Metadata
{
    /// <summary>
    /// Test class for <see cref="AppServiceMetadata"/>.
    /// </summary>
        public class AppServiceMetadataTest
    {
        [Fact]
        public void Constructor_IsOverride()
        {
            var metadata = new Dictionary<string, object> { { "IsOverride", true } };
            var appServiceMetadata = new AppServiceMetadata(metadata);
            Assert.True(appServiceMetadata.IsOverride);
        }

        [Fact]
        public void Constructor_Priority()
        {
            var metadata = new Dictionary<string, object> { { "Priority", Priority.Low } };
            var appServiceMetadata = new AppServiceMetadata(metadata);
            Assert.Equal(Priority.Low, appServiceMetadata.Priority);
        }

        [Fact]
        public void Constructor_ServiceName()
        {
            var metadata = new Dictionary<string, object> { { "ServiceName", "John Doe" } };
            var appServiceMetadata = new AppServiceMetadata(metadata);
            Assert.Equal("John Doe", appServiceMetadata.ServiceName);
        }
    }
}
