// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CoreSimpleTypesModelInfoProviderTest.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Tests for <see cref="CoreSimpleTypesModelInfoProvider" />
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Model.Tests.Runtime
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using System.Threading.Tasks;

    using Kephas.Composition;
    using Kephas.Model.Runtime;
    using Kephas.Model.Runtime.Construction;
    using Kephas.Model.Runtime.Factory;

    using NUnit.Framework;

    /// <summary>
    /// Tests for <see cref="CoreSimpleTypesModelInfoProvider"/>
    /// </summary>
    [TestFixture]
    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented", Justification = "Reviewed. Suppression is OK here.")]
    public class CoreSimpleTypesModelInfoProviderTest
    {
        [Test]
        public async Task GetElementInfosAsync()
        {
            var provider = new CoreSimpleTypesModelInfoProvider(new List<IExportFactory<IRuntimeElementInfoFactory, RuntimeElementInfoFactoryMetadata>>());
            var elementInfos = (await provider.GetElementInfosAsync()).Cast<RuntimeValueTypeInfo>().ToList();

            Assert.IsTrue(elementInfos.Any(info => info.RuntimeElement == typeof(bool) && info.IsPrimitive));

            Assert.IsTrue(elementInfos.Any(info => info.RuntimeElement == typeof(byte) && info.IsPrimitive));
            Assert.IsTrue(elementInfos.Any(info => info.RuntimeElement == typeof(sbyte) && info.IsPrimitive));
            Assert.IsTrue(elementInfos.Any(info => info.RuntimeElement == typeof(short) && info.IsPrimitive));
            Assert.IsTrue(elementInfos.Any(info => info.RuntimeElement == typeof(ushort) && info.IsPrimitive));
            Assert.IsTrue(elementInfos.Any(info => info.RuntimeElement == typeof(int) && info.IsPrimitive));
            Assert.IsTrue(elementInfos.Any(info => info.RuntimeElement == typeof(uint) && info.IsPrimitive));
            Assert.IsTrue(elementInfos.Any(info => info.RuntimeElement == typeof(long) && info.IsPrimitive));
            Assert.IsTrue(elementInfos.Any(info => info.RuntimeElement == typeof(ulong) && info.IsPrimitive));

            Assert.IsTrue(elementInfos.Any(info => info.RuntimeElement == typeof(decimal) && info.IsPrimitive));
            Assert.IsTrue(elementInfos.Any(info => info.RuntimeElement == typeof(double) && info.IsPrimitive));
            Assert.IsTrue(elementInfos.Any(info => info.RuntimeElement == typeof(float) && info.IsPrimitive));

            Assert.IsTrue(elementInfos.Any(info => info.RuntimeElement == typeof(DateTime) && info.IsPrimitive));
            Assert.IsTrue(elementInfos.Any(info => info.RuntimeElement == typeof(DateTimeOffset) && info.IsPrimitive));
            Assert.IsTrue(elementInfos.Any(info => info.RuntimeElement == typeof(TimeSpan) && info.IsPrimitive));

            Assert.IsTrue(elementInfos.Any(info => info.RuntimeElement == typeof(Uri) && info.IsPrimitive));
            Assert.IsTrue(elementInfos.Any(info => info.RuntimeElement == typeof(Guid) && info.IsPrimitive));

            Assert.IsTrue(elementInfos.Any(info => info.RuntimeElement == typeof(string) && info.IsPrimitive));
        }
    }
}