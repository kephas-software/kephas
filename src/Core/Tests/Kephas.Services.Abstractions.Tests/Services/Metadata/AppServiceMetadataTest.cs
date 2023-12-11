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
using NUnit.Framework;

namespace Kephas.Tests.Services.Metadata
{
    /// <summary>
    /// Test class for <see cref="AppServiceMetadata"/>.
    /// </summary>
    [TestFixture]
    public class AppServiceMetadataTest
    {
        [Test]
        public void Constructor_IsOverride()
        {
            var metadata = new Dictionary<string, object> { { "IsOverride", true } };
            var appServiceMetadata = new AppServiceMetadata(metadata);
            Assert.IsTrue(appServiceMetadata.IsOverride);
        }

        [Test]
        public void Constructor_OverridePriority()
        {
            var metadata = new Dictionary<string, object> { { "OverridePriority", Priority.High } };
            var appServiceMetadata = new AppServiceMetadata(metadata);
            Assert.AreEqual(Priority.High, appServiceMetadata.OverridePriority);
        }

        [Test]
        public void Constructor_ProcessingPriority()
        {
            var metadata = new Dictionary<string, object> { { "ProcessingPriority", Priority.Low } };
            var appServiceMetadata = new AppServiceMetadata(metadata);
            Assert.AreEqual(Priority.Low, appServiceMetadata.ProcessingPriority);
        }

        [Test]
        public void Constructor_ServiceName()
        {
            var metadata = new Dictionary<string, object> { { "ServiceName", "John Doe" } };
            var appServiceMetadata = new AppServiceMetadata(metadata);
            Assert.AreEqual("John Doe", appServiceMetadata.ServiceName);
        }
    }
}
