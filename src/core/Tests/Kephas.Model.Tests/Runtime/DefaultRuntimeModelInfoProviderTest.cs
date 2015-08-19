// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DefaultRuntimeModelInfoProviderTest.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Tests for <see cref="DefaultRuntimeModelInfoProvider" />.
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
    /// Tests for <see cref="DefaultRuntimeModelInfoProvider"/>.
    /// </summary>
    [TestFixture]
    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented", Justification = "Reviewed. Suppression is OK here.")]
    public class DefaultRuntimeModelInfoProviderTest
    {
        [Test]
        public async Task GetElementInfosAsync()
        {
            var registrar = Mock.Create<IRuntimeModelRegistry>();
            registrar.Arrange(r => r.GetRuntimeElementsAsync(CancellationToken.None)).Returns(Task.FromResult((IEnumerable<object>)new object[] { typeof(string) }));

            var stringInfoMock = Mock.Create<INamedElementInfo>();

            var factory = Mock.Create<IRuntimeElementInfoFactoryDispatcher>();
            factory.Arrange(f => f.TryGetModelElementInfo(Arg.Is(typeof(string).GetTypeInfo()))).Returns(stringInfoMock);

            var provider = new DefaultRuntimeModelInfoProvider(factory, new[] { registrar });

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