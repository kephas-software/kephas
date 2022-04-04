// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ContextFactoryTest.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the context factory test class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Tests.Services;

using System.Linq;

using Kephas.Injection;
using Kephas.Logging;
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
        var context = factory.CreateContext<TestContext>();

        Assert.AreSame(ambientServices, context.AmbientServices);
        Assert.AreSame(injector, context.Injector);
    }

    [Test]
    public void CreateContext_SerializationContext()
    {
        var testService = Substitute.For<ITestService>();

        var (ambientServices, injector) = this.GetServices(new AppServiceInfo(typeof(ITestService), testService));
        injector.Resolve(typeof(ITestService)).Returns(testService);
        var factory = new ContextFactory(injector, ambientServices, Substitute.For<ILogManager>());
        var context = factory.CreateContext<TestContext>(typeof(string));

        Assert.AreSame(ambientServices, context.AmbientServices);
        Assert.AreSame(injector, context.Injector);
        Assert.AreSame(testService, context.TestService);
        Assert.AreSame(typeof(string), context.MediaType);
    }

    private (IAmbientServices ambientServices, IInjector injector) GetServices(params IAppServiceInfo[] appServiceInfos)
    {
        var ambientServices = Substitute.For<IAmbientServices>();
        var injector = Substitute.For<IInjector>();

        var infos = appServiceInfos.Select(i => new ContractDeclaration(i.ContractType, i));

        ambientServices.Injector.Returns(injector);
        ambientServices.GetService(typeof(IInjector)).Returns(injector);
        ambientServices[InjectionAmbientServicesExtensions.AppServiceInfosKey].Returns(infos);

        injector.Resolve(typeof(IAmbientServices)).Returns(ambientServices);
        injector.Resolve<IAmbientServices>().Returns(ambientServices);
        injector.Resolve(typeof(IInjector)).Returns(injector);
        injector.Resolve<IInjector>().Returns(injector);

        return (ambientServices, injector);
    }

    public class TestContext : Context
    {
        public TestContext(IInjector injector)
            : base(injector)
        {
        }

        public TestContext(IInjector injector, ITestService testService, Type? mediaType = null)
            : base(injector)
        {
            testService = testService ?? throw new ArgumentNullException(nameof(testService));

            this.TestService = testService;
            this.MediaType = mediaType;
        }

        public ITestService TestService { get; }

        public Type? MediaType { get; set; }
    }

    public interface ITestService { }
}