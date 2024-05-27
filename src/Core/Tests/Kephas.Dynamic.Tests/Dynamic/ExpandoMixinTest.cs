// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ExpandoMixinTest.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Tests.Dynamic
{
    using System.Collections.Generic;
    using Kephas.Dynamic;
    using Xunit;

        public class ExpandoMixinTest
    {
        [Fact]
        public void Indexer_from_dictionary()
        {
            var expando = new Expandable();
            expando.ToDynamic()["Hi"] = "there";
            Assert.Equal("there", expando.ToDynamic()["Hi"]);
        }

        [Fact]
        public void Indexer_from_member()
        {
            var expando = new Expandable();
            expando.ToDynamic()["Name"] = "John Doe";
            Assert.Equal("John Doe", expando.ToDynamic()["Name"]);
            Assert.Equal("John Doe", expando.Name);
        }

        [Fact]
        public void HasDynamicMember()
        {
            var expando = new Expandable();
            expando.ToDynamic()["Hi"] = "there";
            Assert.True(expando.ToDynamic().HasKey("Hi"));
            Assert.False(expando.ToDynamic().HasKey("there"));
        }

        [Fact]
        public void ToDictionary()
        {
            var expando = new Expandable();
            expando.ToDynamic()["Hi"] = "there";

            var dict = expando.ToDynamic().ToDictionary();
            Assert.True(dict.ContainsKey("Hi"));
            Assert.False(dict.ContainsKey("there"));
        }

        public class Expandable : IExpandoMixin
        {
            private IDictionary<string, object?> inner = new Dictionary<string, object?>();

            IDictionary<string, object?> IExpandoMixin.InnerDictionary => this.inner;

            public string? Name { get; set; }
        }
    }
}
