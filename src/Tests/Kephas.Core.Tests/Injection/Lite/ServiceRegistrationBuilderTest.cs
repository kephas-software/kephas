// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ServiceRegistrationBuilderTest.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the service registration builder test class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Core.Tests.Injection.Lite
{
    using System;

    using Kephas.Injection.Lite.Builder;
    using Kephas.Services;
    using Kephas.Services.Reflection;
    using NSubstitute;
    using NUnit.Framework;

    [TestFixture]
    public class ServiceRegistrationBuilderTest
    {
        [Test]
        public void Build_scoped_makes_singleton()
        {
            var ambientServices = Substitute.For<IAppServiceRegistry>();
            var builder = new ServiceRegistrationBuilder(ambientServices, typeof(string), typeof(string));
            builder.Scoped();

            var svcInfo = builder.Build();
            Assert.AreSame(typeof(string), svcInfo.ContractType);
            Assert.AreSame(typeof(string), svcInfo.InstanceType);
            Assert.IsTrue(svcInfo.IsSingleton());
        }

        [Test]
        public void Build_generic_service_generic_type()
        {
            var ambientServices = Substitute.For<IAppServiceRegistry>();
            var builder = new ServiceRegistrationBuilder(ambientServices, typeof(IGenericSvc<>), typeof(UnknownGenericSvc<>));

            var svcInfo = builder.Build();
            Assert.AreSame(typeof(IGenericSvc<>), svcInfo.ContractType);
            Assert.AreSame(typeof(UnknownGenericSvc<>), svcInfo.InstanceType);
        }

        [Test]
        public void Build_generic_service_non_generic_implementation_type()
        {
            var ambientServices = Substitute.For<IAppServiceRegistry>();
            var builder = new ServiceRegistrationBuilder(ambientServices, typeof(IGenericSvc<>), typeof(UnknownIntSvc));

            Assert.Throws<ArgumentException>(() => builder.Build());
        }

        [Test]
        public void Build_generic_service_non_generic_implementation_type_non_generic_contract()
        {
            var ambientServices = Substitute.For<IAppServiceRegistry>();
            var builder = new ServiceRegistrationBuilder(ambientServices, typeof(IGenericSvc<>), typeof(UnknownIntSvc));
            builder.As(typeof(ISvc));

            var svcInfo = builder.Build();

            Assert.AreSame(typeof(ISvc), svcInfo.ContractType);
            Assert.AreSame(typeof(UnknownIntSvc), svcInfo.InstanceType);
        }

        [Test]
        public void WithType_generic_service_mismatch_implementation_type()
        {
            var ambientServices = Substitute.For<IAppServiceRegistry>();

            Assert.Throws<ArgumentException>(() => new ServiceRegistrationBuilder(ambientServices, typeof(IGenericSvc<>), typeof(string)));
        }

        private interface IUnknown { }

        private interface ISvc { }

        private interface IGenericSvc<T> : ISvc { }

        private interface IGenericNonDerivedSvc<T> { }

        private class GenericSvc<T> : IGenericSvc<T> { }

        // leave the order of the interfaces like this, there are some use cases that test for generics
        private class UnknownGenericSvc<T> : IUnknown, IGenericSvc<T> { }

        private class UnknownIntSvc : IUnknown, IGenericSvc<int> { }
    }
}
