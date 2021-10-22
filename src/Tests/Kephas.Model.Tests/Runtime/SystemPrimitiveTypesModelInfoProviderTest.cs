// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SystemPrimitiveTypesModelInfoProviderTest.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Tests for <see cref="SystemPrimitiveTypesModelInfoProvider" />
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Model.Tests.Runtime
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using System.Threading.Tasks;

    using Kephas.Data;
    using Kephas.Model.Runtime;
    using Kephas.Model.Runtime.Construction;
    using Kephas.Model.Tests.Runtime.Construction;
    using Kephas.Reflection;

    using NSubstitute;

    using NUnit.Framework;

    /// <summary>
    /// Tests for <see cref="SystemPrimitiveTypesModelInfoProvider"/>
    /// </summary>
    [TestFixture]
    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented", Justification = "Reviewed. Suppression is OK here.")]
    public class SystemPrimitiveTypesModelInfoProviderTest : ConstructorTestBase
    {
        [Test]
        public async Task GetElementInfosAsync()
        {
            var provider = new SystemPrimitiveTypesModelInfoProvider(Substitute.For<IRuntimeModelElementFactory>());
            var context = this.GetConstructionContext();
            var typeRegistry = context.RuntimeTypeRegistry;
            var elementInfos = (await provider.GetElementInfosAsync(context)).Cast<IValueType>().ToList();

            Assert.IsTrue(elementInfos.Any(info => info.Parts.First() == typeRegistry.GetTypeInfo(typeof(bool)) && info.IsPrimitive));

            Assert.IsTrue(elementInfos.Any(info => info.Parts.First() == typeRegistry.GetTypeInfo(typeof(byte)) && info.IsPrimitive));
            Assert.IsTrue(elementInfos.Any(info => info.Parts.First() == typeRegistry.GetTypeInfo(typeof(sbyte)) && info.IsPrimitive));
            Assert.IsTrue(elementInfos.Any(info => info.Parts.First() == typeRegistry.GetTypeInfo(typeof(short)) && info.IsPrimitive));
            Assert.IsTrue(elementInfos.Any(info => info.Parts.First() == typeRegistry.GetTypeInfo(typeof(ushort)) && info.IsPrimitive));
            Assert.IsTrue(elementInfos.Any(info => info.Parts.First() == typeRegistry.GetTypeInfo(typeof(int)) && info.IsPrimitive));
            Assert.IsTrue(elementInfos.Any(info => info.Parts.First() == typeRegistry.GetTypeInfo(typeof(uint)) && info.IsPrimitive));
            Assert.IsTrue(elementInfos.Any(info => info.Parts.First() == typeRegistry.GetTypeInfo(typeof(long)) && info.IsPrimitive));
            Assert.IsTrue(elementInfos.Any(info => info.Parts.First() == typeRegistry.GetTypeInfo(typeof(ulong)) && info.IsPrimitive));

            Assert.IsTrue(elementInfos.Any(info => info.Parts.First() == typeRegistry.GetTypeInfo(typeof(decimal)) && info.IsPrimitive));
            Assert.IsTrue(elementInfos.Any(info => info.Parts.First() == typeRegistry.GetTypeInfo(typeof(double)) && info.IsPrimitive));
            Assert.IsTrue(elementInfos.Any(info => info.Parts.First() == typeRegistry.GetTypeInfo(typeof(float)) && info.IsPrimitive));

            Assert.IsTrue(elementInfos.Any(info => info.Parts.First() == typeRegistry.GetTypeInfo(typeof(DateTime)) && info.IsPrimitive));
            Assert.IsTrue(elementInfos.Any(info => info.Parts.First() == typeRegistry.GetTypeInfo(typeof(DateTimeOffset)) && info.IsPrimitive));
            Assert.IsTrue(elementInfos.Any(info => info.Parts.First() == typeRegistry.GetTypeInfo(typeof(TimeSpan)) && info.IsPrimitive));

            Assert.IsTrue(elementInfos.Any(info => info.Parts.First() == typeRegistry.GetTypeInfo(typeof(Uri)) && info.IsPrimitive));
            Assert.IsTrue(elementInfos.Any(info => info.Parts.First() == typeRegistry.GetTypeInfo(typeof(Guid)) && info.IsPrimitive));

            Assert.IsTrue(elementInfos.Any(info => info.Parts.First() == typeRegistry.GetTypeInfo(typeof(string)) && info.IsPrimitive));
        }
    }
}