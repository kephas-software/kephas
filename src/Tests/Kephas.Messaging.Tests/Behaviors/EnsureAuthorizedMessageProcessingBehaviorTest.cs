﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EnsureAuthorizedMessageProcessingBehaviorTest.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the ensure authorized message processing behavior test class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Messaging.Tests.Behaviors;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Kephas.Messaging.Authorization.Behaviors;
using Kephas.Security.Authorization;
using Kephas.Security.Permissions.AttributedModel;
using Kephas.Services;
using NSubstitute;
using NUnit.Framework;

[TestFixture]
public class EnsureAuthorizedMessageProcessingBehaviorTest
{
    [Test]
    public async Task BeforeProcessAsync_free_message()
    {
        var authService = Substitute.For<IAuthorizationService>();
        authService.AuthorizeAsync(Arg.Any<IContext>(), Arg.Any<IEnumerable<object>>(), Arg.Any<object>(), Arg.Any<Action<IAuthorizationContext>>(), Arg.Any<CancellationToken>())
            .Returns(ci =>
            {
                throw new AuthorizationException("Should not be called!");
                return Task.FromResult(false);
            });
        var behavior = new EnsureAuthorizedMessageProcessingBehavior(authService, Substitute.For<IAuthorizationScopeService>());
        var message = new FreeMessage();
        // this should not crash, no required permissions
        await behavior.BeforeProcessAsync(
            message,
            new MessagingContext(Substitute.For<IServiceProvider>(), Substitute.For<IMessageProcessor>()),
            default);
    }

    [Test]
    public async Task BeforeProcessAsync_auth_message_fail()
    {
        var authService = Substitute.For<IAuthorizationService>();
        authService.AuthorizeAsync(Arg.Any<IContext>(), Arg.Any<IEnumerable<object>>(), Arg.Any<object>(), Arg.Any<Action<IAuthorizationContext>>(), Arg.Any<CancellationToken>())
            .Returns(ci =>
            {
                throw new AuthorizationException($"Should be called with {string.Join(",", ci.Arg<IEnumerable<object>>())}!");
                return Task.FromResult(false);
            });
        var behavior = new EnsureAuthorizedMessageProcessingBehavior(authService, Substitute.For<IAuthorizationScopeService>());
        var message = new NonFreeMessage();
        Assert.ThrowsAsync<AuthorizationException>(() => behavior.BeforeProcessAsync(
            message,
            new MessagingContext(Substitute.For<IServiceProvider>(), Substitute.For<IMessageProcessor>()),
            default), "Should be called with gigi!");
    }

    [Test]
    public async Task BeforeProcessAsync_auth_message_success()
    {
        var authService = Substitute.For<IAuthorizationService>();
        authService.AuthorizeAsync(Arg.Any<IContext>(), Arg.Any<IEnumerable<object>>(), Arg.Any<object>(), Arg.Any<Action<IAuthorizationContext>>(), Arg.Any<CancellationToken>())
            .Returns(ci =>
            {
                var perms = ci.Arg<IEnumerable<object>>().ToList();
                Assert.AreEqual(1, perms.Count);
                Assert.AreEqual(typeof(GigiPermission), perms[0]);
                return Task.FromResult(true);
            });
        var behavior = new EnsureAuthorizedMessageProcessingBehavior(authService, Substitute.For<IAuthorizationScopeService>());
        var message = new NonFreeMessage();
        await behavior.BeforeProcessAsync(
            message,
            new MessagingContext(Substitute.For<IServiceProvider>(), Substitute.For<IMessageProcessor>()),
            default);
    }

    [Test]
    public async Task BeforeProcessAsync_auth_object_message_success()
    {
        var authService = Substitute.For<IAuthorizationService>();
        authService.AuthorizeAsync(Arg.Any<IContext>(), Arg.Any<IEnumerable<object>>(), Arg.Any<object>(), Arg.Any<Action<IAuthorizationContext>>(), Arg.Any<CancellationToken>())
            .Returns(ci =>
            {
                var perms = ci.Arg<IEnumerable<object>>().ToList();
                Assert.AreEqual(1, perms.Count);
                Assert.AreEqual(typeof(GigiPermission), perms[0]);
                return Task.FromResult(true);
            });
        var behavior = new EnsureAuthorizedMessageProcessingBehavior(authService, Substitute.For<IAuthorizationScopeService>());
        var message = new NonFree();
        await behavior.BeforeProcessAsync(
            message.ToMessage(),
            new MessagingContext(Substitute.For<IServiceProvider>(), Substitute.For<IMessageProcessor>()),
            default);
    }

    public class FreeMessage : IMessage { }

    [RequiresPermission(typeof(GigiPermission))]
    public class NonFreeMessage : IMessage { }

    [RequiresPermission(typeof(GigiPermission))]
    public class NonFree { }

    [PermissionInfo]
    public class GigiPermission {}
}