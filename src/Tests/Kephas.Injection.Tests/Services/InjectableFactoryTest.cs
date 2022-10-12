// --------------------------------------------------------------------------------------------------------------------
// <copyright file="InjectableFactoryTest.cs" company="Kephas Software SRL">
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
public class InjectableFactoryTest
{
    [Test]
    public void Create_Context()
    {
        var (ambientServices, injector) = this.GetServices();
        IInjectableFactory factory = new InjectableFactory(injector, ambientServices, Substitute.For<ILogManager>());
        var context = factory.Create<Context>();

        Assert.AreSame(injector, context.ServiceProvider);
    }

    [Test]
    public void Create_TestContext()
    {
        var (ambientServices, injector) = this.GetServices();
        IInjectableFactory factory = new InjectableFactory(injector, ambientServices, Substitute.For<ILogManager>());
        var context = factory.Create<TestContext>();

        Assert.AreSame(injector, context.ServiceProvider);
    }

    [Test]
    public void Create_TestContext_with_params()
    {
        var testService = Substitute.For<ITestService>();

        var (ambientServices, injector) = this.GetServices(new AppServiceInfo(typeof(ITestService), testService));
        injector.Resolve(typeof(ITestService)).Returns(testService);
        IInjectableFactory factory = new InjectableFactory(injector, ambientServices, Substitute.For<ILogManager>());
        var context = factory.Create<TestContext>(typeof(string));

        Assert.AreSame(injector, context.ServiceProvider);
        Assert.AreSame(testService, context.TestService);
        Assert.AreSame(typeof(string), context.MediaType);
    }

    private (IAmbientServices ambientServices, IServiceProvider injector) GetServices(params IAppServiceInfo[] appServiceInfos)
    {
        var ambientServices = Substitute.For<IAmbientServices>();
        var injector = Substitute.For<IServiceProvider>();

        ambientServices.GetEnumerator().Returns(appServiceInfos.GetEnumerator());

        injector.Resolve(typeof(IAmbientServices)).Returns(ambientServices);
        injector.Resolve<IAmbientServices>().Returns(ambientServices);
        injector.Resolve(typeof(IServiceProvider)).Returns(injector);
        injector.Resolve<IServiceProvider>().Returns(injector);

        return (ambientServices, injector);
    }

    public class TestContext : Context
    {
        public TestContext(IServiceProvider serviceProvider)
            : base(serviceProvider)
        {
        }

        public TestContext(IServiceProvider serviceProvider, ITestService testService, Type? mediaType = null)
            : base(serviceProvider)
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