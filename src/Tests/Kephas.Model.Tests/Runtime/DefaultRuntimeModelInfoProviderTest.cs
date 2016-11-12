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
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;

    using Kephas.Model.Construction;
    using Kephas.Model.Runtime;
    using Kephas.Model.Runtime.Construction;

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
            registrar.Arrange(r => r.GetRuntimeElementsAsync(CancellationToken.None)).Returns(Task.FromResult((IEnumerable<object>)new object[] { typeof(string).GetRuntimeTypeInfo() }));

            var stringInfoMock = Mock.Create<INamedElement>();

            var factory = Mock.Create<IRuntimeModelElementFactory>();
            factory.Arrange(f => f.TryCreateModelElement(Arg.IsAny<IModelConstructionContext>(), Arg.Is(typeof(string).GetRuntimeTypeInfo()))).Returns(stringInfoMock);

            var provider = new DefaultRuntimeModelInfoProvider(factory, new[] { registrar });

            var elementInfos = (await provider.GetElementInfosAsync(Mock.Create<IModelConstructionContext>())).ToList();

            Assert.AreEqual(1, elementInfos.Count);
            Assert.AreSame(stringInfoMock, elementInfos[0]);
        }
    }
}