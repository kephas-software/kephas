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
            var (ambientServices, compositionContext) = this.GetServices();
            var factory = new ContextFactory(compositionContext, ambientServices, Substitute.For<ILogManager>());
            var context = factory.CreateContext<Context>();

            Assert.AreSame(ambientServices, context.AmbientServices);
            Assert.AreSame(compositionContext, context.Injector);
        }

        [Test]
        public void CreateContext_EncryptionContext()
        {
            var (ambientServices, compositionContext) = this.GetServices();
            var factory = new ContextFactory(compositionContext, ambientServices, Substitute.For<ILogManager>());
            var context = factory.CreateContext<EncryptionContext>();

            Assert.AreSame(ambientServices, context.AmbientServices);
            Assert.AreSame(compositionContext, context.Injector);
        }

        [Test]
        public void CreateContext_SerializationContext()
        {
            var serializationService = Substitute.For<ISerializationService>();

            var (ambientServices, compositionContext) = this.GetServices(new AppServiceInfo(typeof(ISerializationService), serializationService));
            compositionContext.Resolve(typeof(ISerializationService), Arg.Any<string>()).Returns(serializationService);
            var factory = new ContextFactory(compositionContext, ambientServices, Substitute.For<ILogManager>());
            var context = factory.CreateContext<SerializationContext>(typeof(string));

            Assert.AreSame(ambientServices, context.AmbientServices);
            Assert.AreSame(compositionContext, context.Injector);
            Assert.AreSame(serializationService, context.SerializationService);
            Assert.AreSame(typeof(string), context.MediaType);
        }

        private (IAmbientServices ambientServices, IInjector compositionContext) GetServices(params IAppServiceInfo[] appServiceInfos)
        {
            var ambientServices = Substitute.For<IAmbientServices>();
            var compositionContext = Substitute.For<IInjector>();

            var infos = appServiceInfos.Select(i => (contractType: i.ContractType, appServiceInfo: i));

            ambientServices.Injector.Returns(compositionContext);
            ambientServices.GetService(typeof(IInjector)).Returns(compositionContext);
            ambientServices["__AppServiceInfosKey"].Returns(infos);

            compositionContext.Resolve(typeof(IAmbientServices), Arg.Any<string>()).Returns(ambientServices);
            compositionContext.Resolve<IAmbientServices>(Arg.Any<string>()).Returns(ambientServices);
            compositionContext.Resolve(typeof(IInjector), Arg.Any<string>()).Returns(compositionContext);
            compositionContext.Resolve<IInjector>(Arg.Any<string>()).Returns(compositionContext);

            return (ambientServices, compositionContext);
        }
    }
}
