// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AppServiceMetadataTest.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Test class for <see cref="AppServiceMetadata" />.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Core.Tests.Services.Composition
{
    using System;
    using System.Collections.Generic;

    using Kephas.Composition.Metadata;
    using Kephas.Services.Composition;

    using NUnit.Framework;

    /// <summary>
    /// Test class for <see cref="AppServiceMetadata"/>.
    /// </summary>
    [TestFixture]
    public class AppServiceMetadataTest
    {
        [Test]
        public void GetMetadataValue_Override()
        {
            var metadata = new Dictionary<string, object> { { "Override", true } };
            var appServiceMetadata = new AppServiceMetadata(metadata);
            Assert.IsTrue(appServiceMetadata.IsOverride);
        }

        [Test]
        public void GetMetadataValue_Bool()
        {
            var metadata = new Dictionary<string, object> { { "Bool", false } };
            var appServiceMetadata = new TestAppServiceMetadata(metadata);
            var boolValue = appServiceMetadata.GetMetadataValue<BoolAttribute, bool>(metadata, true);
            Assert.IsFalse(boolValue);
        }

        [Test]
        public void GetMetadataValue_Bool_exception()
        {
            var metadata = new Dictionary<string, object> { { "Bool", "bad value" } };
            var appServiceMetadata = new TestAppServiceMetadata(metadata);
            Assert.Throws<InvalidCastException>(
                () => appServiceMetadata.GetMetadataValue<BoolAttribute, bool>(metadata, true));
        }

        [Test]
        public void GetMetadataValue_Bool_null_with_default()
        {
            var metadata = new Dictionary<string, object> { { "Bool", null } };
            var appServiceMetadata = new TestAppServiceMetadata(metadata);
            var boolValue = appServiceMetadata.GetMetadataValue<BoolAttribute, bool>(metadata, true);
            Assert.IsTrue(boolValue);
        }

        [Test]
        public void GetMetadataValue_Bool_null_with_implicit_default()
        {
            var metadata = new Dictionary<string, object> { { "Bool", null } };
            var appServiceMetadata = new TestAppServiceMetadata(metadata);
            var boolValue = appServiceMetadata.GetMetadataValue<BoolAttribute, bool>(metadata);
            Assert.IsFalse(boolValue);
        }

        [Test]
        public void GetMetadataValue_Bool_missing_with_default()
        {
            var metadata = new Dictionary<string, object>();
            var appServiceMetadata = new TestAppServiceMetadata(metadata);
            var boolValue = appServiceMetadata.GetMetadataValue<BoolAttribute, bool>(metadata, true);
            Assert.IsTrue(boolValue);
        }

        [Test]
        public void GetMetadataValue_Bool_missing_with_implicit_default()
        {
            var metadata = new Dictionary<string, object>();
            var appServiceMetadata = new TestAppServiceMetadata(metadata);
            var boolValue = appServiceMetadata.GetMetadataValue<BoolAttribute, bool>(metadata);
            Assert.IsFalse(boolValue);
        }

        private class BoolAttribute : Attribute, IMetadataValue<bool>
        {
            object IMetadataValue.Value => this.Value;

            public bool Value { get; set; }
        }

        private class TestAppServiceMetadata : AppServiceMetadata
        {
            public TestAppServiceMetadata(IDictionary<string, object?>? metadata) : base(metadata)
            {
            }

            /// <summary>
            /// Gets the metadata value for the specific attribute.
            /// </summary>
            /// <param name="metadata">The metadata.</param>
            /// <param name="defaultValue">The default value.</param>
            /// <typeparam name="TAttribute">The attribute type.</typeparam>
            /// <typeparam name="TValue">The value type.</typeparam>
            /// <returns>The metadata value if found, otherwise the default value.</returns>
            public new TValue GetMetadataValue<TAttribute, TValue>(
                IDictionary<string, object> metadata,
                TValue defaultValue = default(TValue)) where TAttribute : IMetadataValue<TValue>
            {
                return base.GetMetadataValue<TAttribute, TValue>(metadata, defaultValue);
            }
        }
    }
}
