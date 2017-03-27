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
    using Kephas.Logging;

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
        public void RegisterService_instance_cannot_set_null()
        {
            var ambientServices = new AmbientServices();
            Assert.That(() => ambientServices.RegisterService(typeof(ICompositionContext), (object)null), Throws.InstanceOf<Exception>());
        }

        [Test]
        public void RegisterService_factory_cannot_set_null()
        {
            var ambientServices = new AmbientServices();
            Assert.That(() => ambientServices.RegisterService(typeof(ICompositionContext), (Func<object>)null), Throws.InstanceOf<Exception>());
        }

        [Test]
        public void RegisterService_service_instance()
        {
            var ambientServices = new AmbientServices();
            var logManager = Substitute.For<ILogManager>();
            ambientServices.RegisterService(typeof(ILogManager), logManager);
            Assert.AreSame(logManager, ambientServices.GetService(typeof(ILogManager)));
        }

        [Test]
        public void RegisterService_service_factory()
        {
            var ambientServices = new AmbientServices();
            var logManager = Substitute.For<ILogManager>();
            ambientServices.RegisterService(typeof(ILogManager), () => logManager);
            Assert.AreSame(logManager, ambientServices.GetService(typeof(ILogManager)));
        }

        [Test]
        public void RegisterService_service_factory_non_shared()
        {
            var ambientServices = new AmbientServices();
            ambientServices.RegisterService(typeof(ILogManager), () => Substitute.For<ILogManager>());
            var logManager1 = ambientServices.GetService(typeof(ILogManager));
            var logManager2 = ambientServices.GetService(typeof(ILogManager));
            Assert.AreNotSame(logManager1, logManager2);
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