// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RuntimeModelInfoProviderTest.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Tests for <see cref="RuntimeModelInfoProvider" />.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Model.Tests.Runtime
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using System.Reflection;
    using System.Threading;
    using System.Threading.Tasks;

    using Kephas.Composition;
    using Kephas.Composition.Mef;
    using Kephas.Model.Elements.Construction;
    using Kephas.Model.Runtime;
    using Kephas.Model.Runtime.Factory;

    using NUnit.Framework;

    using Telerik.JustMock;
    using Telerik.JustMock.Helpers;

    /// <summary>
    /// Tests for <see cref="RuntimeModelInfoProvider"/>.
    /// </summary>
    [TestFixture]
    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented", Justification = "Reviewed. Suppression is OK here.")]
    public class RuntimeModelInfoProviderTest
    {
        [Test]
        public async Task GetElementInfosAsync()
        {
            var registrar = Mock.Create<IRuntimeModelRegistry>();
            registrar.Arrange(r => r.GetRuntimeElementsAsync(CancellationToken.None)).Returns(Task.FromResult((IEnumerable<object>)new object[] { typeof(string) }));

            var stringInfoMock = Mock.Create<INamedElementInfo>();

            var factory = Mock.Create<IRuntimeElementInfoFactory>();
            factory.Arrange(f => f.TryGetElementInfo(Arg.Is(typeof(string).GetTypeInfo()))).Returns(stringInfoMock);

            var exportFactory = this.CreateElementInfoFactory(factory);

            var provider = new RuntimeModelInfoProvider(new[] { registrar }, new[] { exportFactory });

            var elementInfos = (await provider.GetElementInfosAsync()).ToList();

            Assert.AreEqual(1, elementInfos.Count);
            Assert.AreSame(stringInfoMock, elementInfos[0]);
        }

        private IExportFactory<IRuntimeElementInfoFactory, RuntimeElementInfoFactoryMetadata> CreateElementInfoFactory(IRuntimeElementInfoFactory factory)
        {
            var metadata = new RuntimeElementInfoFactoryMetadata(typeof(IClassifierInfo), typeof(TypeInfo));
            var exportFactory =
                new ExportFactoryAdapter<IRuntimeElementInfoFactory, RuntimeElementInfoFactoryMetadata>(
                    () => Tuple.Create(factory, (Action)null),
                    metadata);
            return exportFactory;
        }
    }
}