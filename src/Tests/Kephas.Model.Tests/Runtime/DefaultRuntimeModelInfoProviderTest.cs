﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DefaultRuntimeModelInfoProviderTest.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Tests for <see cref="DefaultRuntimeModelInfoProvider" />.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System;
using Kephas.Services.Composition;

namespace Kephas.Model.Tests.Runtime
{
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;

    using Kephas.Composition.ExportFactories;
    using Kephas.Model.Construction;
    using Kephas.Model.Runtime;
    using Kephas.Model.Runtime.Construction;
    using Kephas.Runtime;
    using NSubstitute;
    using NUnit.Framework;

    /// <summary>
    /// Tests for <see cref="DefaultRuntimeModelInfoProvider"/>.
    /// </summary>
    [TestFixture]
    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented", Justification = "Reviewed. Suppression is OK here.")]
    public class DefaultRuntimeModelInfoProviderTest
    {
        private RuntimeTypeRegistry typeRegistry;

        public DefaultRuntimeModelInfoProviderTest()
        {
            this.typeRegistry = new RuntimeTypeRegistry();
        }

        [Test]
        public async Task GetElementInfosAsync()
        {
            var registrar = Substitute.For<IRuntimeModelRegistry>();
            registrar.GetRuntimeElementsAsync(CancellationToken.None).Returns(Task.FromResult((IEnumerable<object>)new object[] { typeof(string).GetRuntimeTypeInfo() }));

            var registrarFactory = new ExportFactory<IRuntimeModelRegistry, AppServiceMetadata>(() => registrar, new AppServiceMetadata());

            var stringInfoMock = Substitute.For<INamedElement>();

            var factory = Substitute.For<IRuntimeModelElementFactory>();
            factory.TryCreateModelElement(Arg.Any<IModelConstructionContext>(), Arg.Is(typeof(string).GetRuntimeTypeInfo())).Returns(stringInfoMock);

            var provider = new DefaultRuntimeModelInfoProvider(factory, new[] { registrarFactory }, this.typeRegistry);

            var elementInfos = (await provider.GetElementInfosAsync(Substitute.For<IModelConstructionContext>())).ToList();

            Assert.AreEqual(1, elementInfos.Count);
            Assert.AreSame(stringInfoMock, elementInfos[0]);
        }
    }
}