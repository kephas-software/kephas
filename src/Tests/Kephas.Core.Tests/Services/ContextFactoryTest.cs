// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ContextFactoryTest.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the context factory test class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using Kephas.Injection;

namespace Kephas.Core.Tests.Services
{
    using System.Linq;
    using Kephas.Cryptography;
    using Kephas.Logging;
    using Kephas.Serialization;
    using Kephas.Services;
    using Kephas.Services.Reflection;
    using NSubstitute;
    using NUnit.Framework;

    [TestFixture]
    public class ContextFactoryTest
    {
        [Test]
        public void CreateContext_Context()
        {
            var (ambientServices, injector) = this.GetServices();
            var factory = new ContextFactory(injector, ambientServices, Substitute.For<ILogManager>());
            var context = factory.CreateContext<Context>();

            Assert.AreSame(ambientServices, context.AmbientServices);
            Assert.AreSame(injector, context.Injector);
        }

        [Test]
        public void CreateContext_EncryptionContext()
        {
            var (ambientServices, injector) = this.GetServices();
            var factory = new ContextFactory(injector, ambientServices, Substitute.For<ILogManager>());
            var context = factory.CreateContext<EncryptionContext>();

            Assert.AreSame(ambientServices, context.AmbientServices);
            Assert.AreSame(injector, context.Injector);
        }

        [Test]
        public void CreateContext_SerializationContext()
        {
            var serializationService = Substitute.For<ISerializationService>();

            var (ambientServices, injector) = this.GetServices(new AppServiceInfo(typeof(ISerializationService), serializationService));
            injector.Resolve(typeof(ISerializationService)).Returns(serializationService);
            var factory = new ContextFactory(injector, ambientServices, Substitute.For<ILogManager>());
            var context = factory.CreateContext<SerializationContext>(typeof(string));

            Assert.AreSame(ambientServices, context.AmbientServices);
            Assert.AreSame(injector, context.Injector);
            Assert.AreSame(serializationService, context.SerializationService);
            Assert.AreSame(typeof(string), context.MediaType);
        }

        private (IAmbientServices ambientServices, IInjector injector) GetServices(params IAppServiceInfo[] appServiceInfos)
        {
            var ambientServices = Substitute.For<IAmbientServices>();
            var injector = Substitute.For<IInjector>();

            var infos = appServiceInfos.Select(i => (contractType: i.ContractType, appServiceInfo: i));

            ambientServices.Injector.Returns(injector);
            ambientServices.GetService(typeof(IInjector)).Returns(injector);
            ambientServices["__AppServiceInfosKey"].Returns(infos);

            injector.Resolve(typeof(IAmbientServices)).Returns(ambientServices);
            injector.Resolve<IAmbientServices>().Returns(ambientServices);
            injector.Resolve(typeof(IInjector)).Returns(injector);
            injector.Resolve<IInjector>().Returns(injector);

            return (ambientServices, injector);
        }
    }
}
