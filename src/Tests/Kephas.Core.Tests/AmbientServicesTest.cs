// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AmbientServicesTest.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Test class for <see cref="AmbientServices" />.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Core.Tests
{
    using System;
    using System.Diagnostics.CodeAnalysis;

    using Kephas;
    using Kephas.Composition;

    using NSubstitute;

    using NUnit.Framework;

    /// <summary>
    /// Test class for <see cref="AmbientServices"/>.
    /// </summary>
    [TestFixture]
    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented", Justification = "Reviewed. Suppression is OK here.")]
    public class AmbientServicesTest
    {
        [Test]
        public void CompositionContainer_cannot_set_null()
        {
            var ambientServices = new AmbientServices();
            Assert.That(() => ambientServices.RegisterService<ICompositionContext>(null), Throws.InstanceOf<Exception>());
        }

        [Test]
        public void CompositionContainer_works_fine_when_explicitely_set()
        {
            var ambientServices = new AmbientServices();
            var compositionContextMock = Substitute.For<ICompositionContext>();
            compositionContextMock.TryGetExport<ICompositionContext>(null).Returns((ICompositionContext)null);
            ambientServices.RegisterService(compositionContextMock);
            var noService = ambientServices.CompositionContainer.TryGetExport<ICompositionContext>();
            Assert.IsNull(noService);
        }
    }
}