// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ServiceFactoryTest.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the context factory test class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using Kephas.DependencyInjection.Reflection;
using Microsoft.Extensions.Logging;

namespace Kephas.Tests.Services;

using System.Linq;
using Kephas.Services;
using NSubstitute;
using Xunit;

public class ServiceFactoryTest
{
    [Fact]
    public void Create_Context()
    {
        var (appServices, injector) = GetServices();
        IServiceFactory factory = new ServiceFactory(injector, appServices, Substitute.For<ILoggerFactory>());
        var context = factory.Create<Context>();

        Assert.Same(injector, context.ServiceProvider);
    }

    [Fact]
    public void Create_TestContext()
    {
        var (appServices, injector) = GetServices();
        IServiceFactory factory = new ServiceFactory(injector, appServices, Substitute.For<ILoggerFactory>());
        var context = factory.Create<TestContext>();

        Assert.Same(injector, context.ServiceProvider);
    }

    [Fact]
    public void Create_TestContext_with_params()
    {
        var testService = Substitute.For<ITestService>();

        var (appServices, injector) = GetServices(new ServiceInfo(typeof(ITestService), testService));
        injector.GetService(typeof(ITestService)).Returns(testService);
        IServiceFactory factory = new ServiceFactory(injector, appServices, Substitute.For<ILoggerFactory>());
        var context = factory.Create<TestContext>(typeof(string));

        Assert.Same(injector, context.ServiceProvider);
        Assert.Same(testService, context.TestService);
        Assert.Same(typeof(string), context.MediaType);
    }

    private (IAppServiceCollection appServices, IServiceProvider injector) GetServices(params IServiceInfo[] appServiceInfos)
    {
        var appServices = Substitute.For<IAppServiceCollection>();
        var injector = Substitute.For<IServiceProvider>();

        appServices.GetEnumerator().Returns(appServiceInfos.GetEnumerator());

        injector.GetService(typeof(IAppServiceCollection)).Returns(appServices);
        injector.GetService(typeof(IServiceProvider)).Returns(injector);

        return (appServices, injector);
    }

    public class Context
    {
        public Context(IServiceProvider serviceProvider)
        {
            ServiceProvider = serviceProvider;
        }

        public IServiceProvider ServiceProvider { get; }
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

            TestService = testService;
            MediaType = mediaType;
        }

        public ITestService TestService { get; }

        public Type? MediaType { get; set; }
    }

    public interface ITestService { }
}
