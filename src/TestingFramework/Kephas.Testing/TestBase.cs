// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TestBase.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the test base class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using Kephas.Injection;
using Kephas.Injection.ExportFactories;

namespace Kephas.Testing
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Threading;
    using System.Threading.Tasks;

    using Kephas.Application;
    using Kephas.Cryptography;
    using Kephas.Interaction;
    using Kephas.Logging;
    using Kephas.Operations;
    using Kephas.Reflection;
    using Kephas.Serialization;
    using Kephas.Services;
    using NSubstitute;
    using NSubstitute.Core;

    /// <summary>
    /// Base class for tests.
    /// </summary>
    /// <content>
    /// It includes:
    /// * Creating mocks for:
    ///   * <see cref="IContextFactory"/>.
    ///   * <see cref="ISerializationService"/>.
    /// </content>
    public class TestBase
    {
        /// <summary>
        /// Creates default application runtime.
        /// </summary>
        /// <param name="logManager">Manager for log.</param>
        /// <returns>
        /// The new default application runtime.
        /// </returns>
        protected virtual IAppRuntime CreateDefaultAppRuntime(ILogManager logManager)
        {
            var appRuntime = new StaticAppRuntime(
                                         logManager.GetLogger,
                                         defaultAssemblyFilter: this.IsNotTestAssembly);
            return appRuntime;
        }

        /// <summary>
        /// Query if 'a' is not test assembly.
        /// </summary>
        /// <param name="assembly">An Assembly to process.</param>
        /// <returns>
        /// True if not test assembly, false if not.
        /// </returns>
        protected virtual bool IsNotTestAssembly(AssemblyName assembly)
        {
            return !assembly.IsSystemAssembly()
                && !assembly.FullName.StartsWith("NUnit", StringComparison.OrdinalIgnoreCase)
                && !assembly.FullName.StartsWith("xunit", StringComparison.OrdinalIgnoreCase)
                && !assembly.FullName.StartsWith("JetBrains", StringComparison.OrdinalIgnoreCase)
                && !assembly.FullName.StartsWith("ReSharper", StringComparison.OrdinalIgnoreCase)
                && !assembly.Name.EndsWith("Testing", StringComparison.OrdinalIgnoreCase)
                && !assembly.Name.EndsWith("Test", StringComparison.OrdinalIgnoreCase)
                && !assembly.Name.EndsWith("Tests", StringComparison.OrdinalIgnoreCase);
        }

        /// <summary>
        /// Creates a context factory mock.
        /// </summary>
        /// <typeparam name="TContext">Type of the context.</typeparam>
        /// <param name="ctor">The constructor for the created context.</param>
        /// <returns>
        /// The new context factory.
        /// </returns>
        protected IContextFactory CreateContextFactoryMock<TContext>(Func<TContext> ctor)
            where TContext : class
        {
            var contextFactory = Substitute.For<IContextFactory>();
            contextFactory.CreateContext<TContext>(Arg.Any<object[]>())
                .Returns(ci => ctor());
            return contextFactory;
        }

        /// <summary>
        /// Creates a context factory mock.
        /// </summary>
        /// <typeparam name="TContext">Type of the context.</typeparam>
        /// <param name="ctor">The constructor for the created context.</param>
        /// <returns>
        /// The new context factory.
        /// </returns>
        protected IContextFactory CreateContextFactoryMock<TContext>(Func<object[], TContext> ctor)
            where TContext : class
        {
            var contextFactory = Substitute.For<IContextFactory>();
            contextFactory.CreateContext<TContext>(Arg.Any<object[]>())
                .Returns(ci => ctor(ci.Arg<object[]>()));
            return contextFactory;
        }

        /// <summary>
        /// Creates a serialization service mock, aware of <see cref="IContextFactory"/>.
        /// </summary>
        /// <returns>
        /// The new serialization service mock.
        /// </returns>
        protected ISerializationService CreateSerializationServiceMock()
        {
            var serializationService = Substitute.For<ISerializationService>(/*Behavior.Strict*/);
            var contextFactoryMock = this.CreateContextFactoryMock(() => new SerializationContext(Substitute.For<IInjector>(), serializationService));
            return serializationService;
        }

        /// <summary>
        /// Creates a serialization service mock, aware of <see cref="IContextFactory"/>,
        /// and with a handler for the provided media type.
        /// </summary>
        /// <typeparam name="TMediaType">Type of the media type.</typeparam>
        /// <param name="serializer">The serializer.</param>
        /// <returns>
        /// The new serialization service mock.
        /// </returns>
        protected ISerializationService CreateSerializationServiceMock<TMediaType>(ISerializer serializer)
        {
            var contextFactoryMock = this.CreateContextFactoryMock(() => new SerializationContext(Substitute.For<IInjector>(), Substitute.For<ISerializationService>()));
            var serializationService = new DefaultSerializationService(
                contextFactoryMock,
                new List<IExportFactory<ISerializer, SerializerMetadata>>
                {
                    new ExportFactory<ISerializer, SerializerMetadata>(() => serializer, new SerializerMetadata(typeof(TMediaType))),
                });
            return serializationService;
        }

        /// <summary>
        /// Creates an event hub mock.
        /// </summary>
        /// <returns>
        /// The new event hub mock.
        /// </returns>
        protected IEventHub CreateEventHubMock()
        {
            var eventHub = Substitute.For<IEventHub>();

            var dict = new Dictionary<IEventSubscription, Func<object, IContext, CancellationToken, Task>>();

            eventHub.PublishAsync(Arg.Any<object>(), Arg.Any<IContext>(), Arg.Any<CancellationToken>())
                .Returns(
                    async ci =>
                    {
                        foreach (var func in dict.Values)
                        {
                            await func(ci.Arg<object>(), ci.Arg<IContext>(), ci.Arg<CancellationToken>());
                        }

                        return Enumerable.Empty<object?>().ToOperationResult();
                    });

            eventHub.Subscribe(Arg.Any<Type>(), Arg.Any<Func<object, IContext?, CancellationToken, Task>>())
                .Returns(
                    ci =>
                    {
                        var subscription = Substitute.For<IEventSubscription>();
                        var match = ci.Arg<Type>();
                        var func = ci.Arg<Func<object, IContext?, CancellationToken, Task>>();
                        dict.Add(subscription, (e, c, t) =>
                        {
                            if (match == e.GetType())
                            {
                                return func(e, c, t);
                            }

                            return Task.FromResult(0);
                        });

                        subscription.When(s => s.Dispose())
                            .Do(cis => dict.Remove(subscription));

                        return subscription;
                    });

            eventHub.Subscribe(Arg.Any<Func<object, bool>>(), Arg.Any<Func<object, IContext?, CancellationToken, Task>>())
                .Returns(
                    ci =>
                    {
                        var subscription = Substitute.For<IEventSubscription>();
                        var match = ci.Arg<Func<object, bool>>();
                        var func = ci.Arg<Func<object, IContext, CancellationToken, Task>>();
                        dict.Add(subscription, (e, c, t) =>
                        {
                            if (match(e))
                            {
                                return func(e, c, t);
                            }

                            return Task.FromResult(0);
                        });

                        subscription.When(s => s.Dispose())
                            .Do(cis => dict.Remove(subscription));

                        return subscription;
                    });

            return eventHub;
        }

        /// <summary>
        /// Creates an encryption service mock.
        /// </summary>
        /// <returns>
        /// The new encryption service mock.
        /// </returns>
        protected virtual IEncryptionService CreateEncryptionServiceMock()
        {
            var encryptionService = Substitute.For<IEncryptionService>();

            encryptionService.WhenForAnyArgs(s => s.Encrypt(null, null, null))
                .Do(this.ReverseBytes);
            encryptionService.WhenForAnyArgs(s => s.Decrypt(null, null, null))
                .Do(this.ReverseBytes);

            encryptionService.EncryptAsync(null, null, null, default)
                .ReturnsForAnyArgs(Task.FromResult(0))
                .AndDoes(this.ReverseBytes);
            encryptionService.DecryptAsync(null, null, null, default)
                .ReturnsForAnyArgs(Task.FromResult(0))
                .AndDoes(this.ReverseBytes);

            return encryptionService;
        }

        private void ReverseBytes(CallInfo ci)
        {
            var inputStream = ci.ArgAt<Stream>(0);
            var outputStream = ci.ArgAt<Stream>(1);
            var inputArray = ((MemoryStream)inputStream).ToArray();
            var outputArray = inputArray.Reverse().ToArray();
            outputStream.Write(outputArray, 0, outputArray.Length);
        }
    }
}
