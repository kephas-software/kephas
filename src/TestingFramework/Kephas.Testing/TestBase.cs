// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TestBase.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the test base class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Testing
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;

    using Kephas.Application;
    using Kephas.Cryptography;
    using Kephas.Diagnostics.Logging;
    using Kephas.Interaction;
    using Kephas.Logging;
    using Kephas.Operations;
    using Kephas.Reflection;
    using Kephas.Runtime;
    using Kephas.Serialization;
    using Kephas.Services;
    using Kephas.Services.Builder;
    using NSubstitute;
    using NSubstitute.Core;

    /// <summary>
    /// Base class for tests.
    /// </summary>
    /// <content>
    /// It includes:
    /// * Creating mocks for:
    ///   * <see cref="Kephas.Services.IInjectableFactory"/>.
    /// </content>
    public class TestBase
    {
        /// <summary>
        /// Creates a <see cref="IAppServiceCollectionBuilder"/> for further configuration.
        /// </summary>
        /// <param name="ambientServices">Optional. The ambient services. If not provided, a new instance
        ///                               will be created as linked to the newly created container.</param>
        /// <param name="logManager">Optional. Manager for log.</param>
        /// <param name="appRuntime">Optional. The application runtime.</param>
        /// <returns>
        /// A LiteInjectorBuilder.
        /// </returns>
        protected virtual IAppServiceCollectionBuilder CreateServicesBuilder(
            IAmbientServices? ambientServices = null,
            ILogManager? logManager = null,
            IAppRuntime? appRuntime = null)
        {
            var log = new StringBuilder();
            logManager ??= ambientServices?.TryGetServiceInstance<ILogManager>() ?? new DebugLogManager(log);
            appRuntime ??= this.CreateDefaultAppRuntime(logManager);

            ambientServices = (ambientServices ?? new AppServiceCollection())
                .Add(logManager)
                .Add(log);

            return new AppServiceCollectionBuilder(ambientServices)
                .WithAppRuntime(appRuntime)
                .WithAssemblies(this.GetAssemblies())
                .WithParts(this.GetDefaultParts());
        }

        /// <summary>
        /// Gets the default convention types to be considered when building the container. By default it includes Kephas.Core.
        /// </summary>
        /// <returns>
        /// An enumerator that allows foreach to be used to process the default convention types in
        /// this collection.
        /// </returns>
        protected virtual IEnumerable<Assembly> GetAssemblies()
        {
            return new List<Assembly>
                       {
                           typeof(IAmbientServices).Assembly,       /* Kephas.Services */
                           typeof(IEventHub).Assembly,              /* Kephas.Interaction */
                           typeof(ISerializationService).Assembly,  /* Kephas.Serialization */
                       };
        }

        /// <summary>
        /// Gets the default parts to be included in the container.
        /// </summary>
        /// <returns>
        /// An enumerator that allows foreach to be used to process the default parts in this collection.
        /// </returns>
        protected virtual IEnumerable<Type> GetDefaultParts()
        {
            return new List<Type>();
        }

        /// <summary>
        /// Creates a new instance of <see cref="AppServiceCollection"/>
        /// with the provider <see cref="IRuntimeTypeRegistry"/> or a newly created one.
        /// </summary>
        /// <param name="typeRegistry">Optional. The type registry.</param>
        /// <returns>The newly created <see cref="AppServiceCollection"/> instance.</returns>
        protected virtual IAmbientServices CreateAmbientServices(IRuntimeTypeRegistry? typeRegistry = null)
        {
            return new AppServiceCollection()
                .Add(typeRegistry ?? RuntimeTypeRegistry.Instance, b => b.ExternallyOwned());
        }

        /// <summary>
        /// Creates default application runtime.
        /// </summary>
        /// <param name="logManager">Manager for log.</param>
        /// <returns>
        /// The new default application runtime.
        /// </returns>
        protected virtual IAppRuntime CreateDefaultAppRuntime(ILogManager logManager)
        {
            var appRuntime = new StaticAppRuntime(new AppRuntimeSettings
                {
                    GetLogger = logManager.GetLogger,
                    IsAppAssembly = this.IsNotTestAssembly,
                });

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
        /// Creates an injectable factory mock.
        /// </summary>
        /// <typeparam name="T">Type of the injectable.</typeparam>
        /// <param name="ctor">The constructor for the created injectable.</param>
        /// <returns>
        /// The new injectable factory.
        /// </returns>
        protected IInjectableFactory CreateInjectableFactoryMock<T>(Func<T> ctor)
            where T : class
        {
            var factory = Substitute.For<IInjectableFactory>();
            factory.Create<T>(Arg.Any<object[]>())
                .Returns(ci => ctor());
            factory.Create(typeof(T), Arg.Any<object[]>())
                .Returns(ci => ctor());
            return factory;
        }

        /// <summary>
        /// Creates an injectable factory mock.
        /// </summary>
        /// <typeparam name="T">Type of the injectable.</typeparam>
        /// <param name="ctor">The constructor for the created injectable.</param>
        /// <returns>
        /// The new injectable factory.
        /// </returns>
        protected IInjectableFactory CreateInjectableFactoryMock<T>(Func<object[], T> ctor)
            where T : class
        {
            var factory = Substitute.For<IInjectableFactory>();
            factory.Create<T>(Arg.Any<object[]>())
                .Returns(ci => ctor(ci.Arg<object[]>()));
            factory.Create(typeof(T), Arg.Any<object[]>())
                .Returns(ci => ctor(ci.Arg<object[]>()));
            return factory;
        }

        /// <summary>
        /// Creates an injectable factory mock.
        /// </summary>
        /// <typeparam name="T">Type of the injectable.</typeparam>
        /// <param name="ctor">The constructor for the created injectable.</param>
        /// <returns>
        /// The new injectable factory.
        /// </returns>
        protected IInjectableFactory CreateInjectableFactoryMock<T>(Func<CallInfo, object[], T> ctor)
            where T : class
        {
            var factory = Substitute.For<IInjectableFactory>();
            factory.Create<T>(Arg.Any<object[]>())
                .Returns(ci => ctor(ci, ci.Arg<object[]>()));
            factory.Create(typeof(T), Arg.Any<object[]>())
                .Returns(ci => ctor(ci, ci.Arg<object[]>()));
            return factory;
        }

        /// <summary>
        /// Creates a serialization service mock, aware of <see cref="Kephas.Services.IInjectableFactory"/>.
        /// </summary>
        /// <returns>
        /// The new serialization service mock.
        /// </returns>
        protected ISerializationService CreateSerializationServiceMock()
        {
            var serializationService = Substitute.For<ISerializationService, IInjectableFactoryAware>(/*Behavior.Strict*/);
            var factoryMock = this.CreateInjectableFactoryMock(
                () => new SerializationContext(Substitute.For<IServiceProvider>(), serializationService));
            ((IInjectableFactoryAware)serializationService).InjectableFactory.Returns(factoryMock);
            return serializationService;
        }

        /// <summary>
        /// Creates a serialization service mock, aware of <see cref="Kephas.Services.IInjectableFactory"/>,
        /// and with a handler for the provided media type.
        /// </summary>
        /// <typeparam name="TMediaType">Type of the media type.</typeparam>
        /// <param name="serializer">The serializer.</param>
        /// <returns>
        /// The new serialization service mock.
        /// </returns>
        protected ISerializationService CreateSerializationServiceMock<TMediaType>(ISerializer serializer)
        {
            var factoryMock = this.CreateInjectableFactoryMock(() => new SerializationContext(Substitute.For<IServiceProvider>(), Substitute.For<ISerializationService>()));
            var serializationService = new DefaultSerializationService(
                factoryMock,
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

        public interface IInjectableFactoryAware
        {
            IInjectableFactory InjectableFactory { get; }
        }
    }
}
