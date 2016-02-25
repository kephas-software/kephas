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
    using Kephas.Model.Factory;
    using Kephas.Model.Runtime;
    using Kephas.Model.Runtime.Construction;
    using Kephas.Model.Runtime.Factory;
    using Kephas.Model.Runtime.Factory.Composition;

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
            registrar.Arrange(r => r.GetRuntimeElementsAsync(CancellationToken.None)).Returns(Task.FromResult((IEnumerable<object>)new object[] { typeof(string).GetDynamicTypeInfo() }));

            var stringInfoMock = Mock.Create<INamedElementInfo>();

            var factory = Mock.Create<IRuntimeModelElementFactory>();
            factory.Arrange(f => f.TryCreateModelElement(Arg.IsAny<IModelConstructionContext>(), Arg.Is(typeof(string).GetDynamicTypeInfo()))).Returns(stringInfoMock);

            var provider = new DefaultRuntimeModelInfoProvider(factory, new[] { registrar });

            var elementInfos = (await provider.GetElementInfosAsync(TODO)).ToList();

            Assert.AreEqual(1, elementInfos.Count);
            Assert.AreSame(stringInfoMock, elementInfos[0]);
        }

        private IExportFactory<IRuntimeModelElementConstructor, RuntimeModelElementConstructorMetadata> CreateElementInfoFactory(IRuntimeModelElementConstructor constructor)
        {
            var metadata = new RuntimeModelElementConstructorMetadata(typeof(IClassifierInfo), typeof(TypeInfo));
            var exportFactory =
                new ExportFactoryAdapter<IRuntimeModelElementConstructor, RuntimeModelElementConstructorMetadata>(
                    () => Tuple.Create(constructor, (Action)null),
                    metadata);
            return exportFactory;
        }
    }
}