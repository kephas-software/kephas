// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EnsureRequiredFeatureMessageProcessingBehaviorTest.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Messaging.Tests.Application.Behaviors;

using Kephas.Application;
using Kephas.Application.Reflection;
using Kephas.Injection;
using Kephas.Messaging.Application.Behaviors;
using NSubstitute;
using NUnit.Framework;

[TestFixture]
public class EnsureRequiredFeatureMessageProcessingBehaviorTest
{
    [Test]
    public async Task BeforeProcessAsync_free_message()
    {
        var appRuntime = new StaticAppRuntime();
        var behavior = new EnsureRequiredFeatureMessageProcessingBehavior(appRuntime);
        var message = new FreeMessage();
        // this should not crash, no required permissions
        await behavior.BeforeProcessAsync(
            message,
            new MessagingContext(Substitute.For<IServiceProvider>(), Substitute.For<IMessageProcessor>()),
            default);
    }

    [Test]
    public async Task BeforeProcessAsync_requiresfeature_message_fail()
    {
        var appRuntime = new StaticAppRuntime();
        var behavior = new EnsureRequiredFeatureMessageProcessingBehavior(appRuntime);
        var message = new NonFreeMessage();
        Assert.ThrowsAsync<MessagingException>(() => behavior.BeforeProcessAsync(
            message,
            new MessagingContext(Substitute.For<IServiceProvider>(), Substitute.For<IMessageProcessor>()),
            default), "Should be called with test!");
    }

    [Test]
    public async Task BeforeProcessAsync_requiresfeature_message_success()
    {
        var appRuntime = new StaticAppRuntime();
        var behavior = new EnsureRequiredFeatureMessageProcessingBehavior(appRuntime);
        appRuntime.SetFeatures(new IFeatureInfo[] { new FeatureInfo("test", "0.0") });
        var message = new NonFreeMessage();
        await behavior.BeforeProcessAsync(
            message,
            new MessagingContext(Substitute.For<IServiceProvider>(), Substitute.For<IMessageProcessor>()),
            default);
    }

    [Test]
    public async Task BeforeProcessAsync_requiresfeature_object_message_success()
    {
        var appRuntime = new StaticAppRuntime();
        var behavior = new EnsureRequiredFeatureMessageProcessingBehavior(appRuntime);
        appRuntime.SetFeatures(new IFeatureInfo[] { new FeatureInfo("test", "0.0") });
        var message = new NonFree();
        await behavior.BeforeProcessAsync(
            message.ToMessage(),
            new MessagingContext(Substitute.For<IServiceProvider>(), Substitute.For<IMessageProcessor>()),
            default);
    }

    public class FreeMessage : IMessage { }

    [RequiresFeature("test")]
    public class NonFreeMessage : IMessage { }

    [RequiresFeature("test")]
    public class NonFree { }
}