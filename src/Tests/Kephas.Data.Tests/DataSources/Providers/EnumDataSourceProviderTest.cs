// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EnumDataSourceProviderTest.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the enum data source provider test class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Data.Tests.DataSources.Providers
{
    using System.Linq;
    using System.Threading.Tasks;

    using Kephas.Data.DataSources;
    using Kephas.Data.DataSources.Providers;
    using Kephas.Reflection;
    using Kephas.Runtime;
    using NSubstitute;
    using NUnit.Framework;

    [TestFixture]
    public class EnumDataSourceProviderTest
    {
        private IRuntimeTypeRegistry typeRegistry;

        public EnumDataSourceProviderTest()
        {
            this.typeRegistry = new RuntimeTypeRegistry();
        }

        [Test]
        public void CanHandle_non_nullable()
        {
            var typeInfo = typeof(TestWithEnum).AsRuntimeTypeInfo(this.typeRegistry);
            var provider = new EnumDataSourceProvider();
            var context = new DataSourceContext(Substitute.For<IDataContext>(), typeInfo, typeInfo);
            var canHandle = provider.CanHandle(
                                 typeInfo.Properties[nameof(TestWithEnum.Prop1)],
                                 context);
            Assert.IsTrue(canHandle);
        }

        [Test]
        public void CanHandle_nullable()
        {
            var typeInfo = typeof(TestWithEnum).AsRuntimeTypeInfo(this.typeRegistry);
            var provider = new EnumDataSourceProvider();
            var context = new DataSourceContext(Substitute.For<IDataContext>(), typeInfo, typeInfo);
            var canHandle = provider.CanHandle(
                typeInfo.Properties[nameof(TestWithEnum.NullableProp1)],
                context);
            Assert.IsTrue(canHandle);
        }

        [Test]
        public void CanHandle_non_enum()
        {
            var typeInfo = typeof(TestWithEnum).AsRuntimeTypeInfo(this.typeRegistry);
            var provider = new EnumDataSourceProvider();
            var context = new DataSourceContext(Substitute.For<IDataContext>(), typeInfo, typeInfo);
            var canHandle = provider.CanHandle(
                typeInfo.Properties[nameof(TestWithEnum.NonEnum)],
                context);
            Assert.IsFalse(canHandle);
        }

        [Test]
        public async Task GetDataSourceAsync_non_nullable()
        {
            var typeInfo = typeof(TestWithEnum).AsRuntimeTypeInfo(this.typeRegistry);
            var provider = new EnumDataSourceProvider();
            var context = new DataSourceContext(Substitute.For<IDataContext>(), typeInfo, typeInfo);
            var dataSource = await provider.GetDataSourceAsync(
                                 typeInfo.Properties[nameof(TestWithEnum.Prop1)],
                                 context);
            var listSource = dataSource.Cast<IDataSourceItem>().ToList();
            Assert.AreEqual(2, listSource.Count);
            Assert.AreEqual(TestEnum.Value1, listSource[0].Id);
            Assert.AreEqual(nameof(TestEnum.Value1), listSource[0].DisplayText);
            Assert.AreEqual(TestEnum.Value2, listSource[1].Id);
            Assert.AreEqual(nameof(TestEnum.Value2), listSource[1].DisplayText);
        }

        [Test]
        public async Task GetDataSourceAsync_nullable()
        {
            var typeInfo = typeof(TestWithEnum).AsRuntimeTypeInfo(this.typeRegistry);
            var provider = new EnumDataSourceProvider();
            var context = new DataSourceContext(Substitute.For<IDataContext>(), typeInfo, typeInfo);
            var dataSource = await provider.GetDataSourceAsync(
                                 typeInfo.Properties[nameof(TestWithEnum.NullableProp1)],
                                 context);
            var listSource = dataSource.Cast<IDataSourceItem>().ToList();
            Assert.AreEqual(2, listSource.Count);
            Assert.AreEqual(TestEnum.Value1, listSource[0].Id);
            Assert.AreEqual(nameof(TestEnum.Value1), listSource[0].DisplayText);
            Assert.AreEqual(TestEnum.Value2, listSource[1].Id);
            Assert.AreEqual(nameof(TestEnum.Value2), listSource[1].DisplayText);
        }

        [Test]
        public async Task GetDataSourceAsync_non_enum()
        {
            var typeInfo = typeof(TestWithEnum).AsRuntimeTypeInfo(this.typeRegistry);
            var provider = new EnumDataSourceProvider();
            var context = new DataSourceContext(Substitute.For<IDataContext>(), typeInfo, typeInfo);
            var dataSource = await provider.GetDataSourceAsync(
                                 typeInfo.Properties[nameof(TestWithEnum.NonEnum)],
                                 context);
            Assert.IsNull(dataSource);
        }

        public class TestWithEnum
        {
            public TestEnum Prop1 { get; set; }
            public TestEnum? NullableProp1 { get; set; }
            public string NonEnum { get; set; }
        }

        public enum TestEnum
        {
            Value1,
            Value2
        }
    }
}
