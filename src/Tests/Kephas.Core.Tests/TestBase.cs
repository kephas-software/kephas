﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TestBase.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the test base class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Core.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;

    using Kephas.Application;
    using Kephas.Composition;
    using Kephas.Composition.ExportFactories;
    using Kephas.Logging;
    using Kephas.Reflection;
    using Kephas.Serialization;
    using Kephas.Serialization.Composition;
    using Kephas.Services;
    using NSubstitute;

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
                                         name => logManager.GetLogger(name),
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
                && !assembly.Name.EndsWith("Testing", StringComparison.OrdinalIgnoreCase)
                && !assembly.Name.EndsWith("Test", StringComparison.OrdinalIgnoreCase)
                && !assembly.Name.EndsWith("Tests", StringComparison.OrdinalIgnoreCase);
        }

        /// <summary>
        /// Creates a context factory.
        /// </summary>
        /// <typeparam name="TContext">Type of the context.</typeparam>
        /// <param name="ctor">The constructor for the created context.</param>
        /// <returns>
        /// The new context factory.
        /// </returns>
        protected IContextFactory CreateContextFactory<TContext>(Func<TContext> ctor)
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
        protected IContextFactory CreateContextFactoryMock<TContext>(Func<TContext> ctor)
            where TContext : class
        {
            var contextFactory = Substitute.For<IContextFactory>();
            contextFactory.CreateContext<TContext>(Arg.Any<object[]>())
                .Returns(ci => ctor());
            return contextFactory;
        }

        protected ISerializationService CreateSerializationServiceMock()
        {
            var serializationService = Substitute.For<ISerializationService, IContextFactoryAware>(/*Behavior.Strict*/);
            var contextFactoryMock = Substitute.For<IContextFactory>();
            ((IContextFactoryAware)serializationService).ContextFactory.Returns(contextFactoryMock);
            contextFactoryMock.CreateContext<SerializationContext>(Arg.Any<object[]>())
                .Returns(ci => new SerializationContext(Substitute.For<ICompositionContext>(), serializationService));
            return serializationService;
        }

        protected ISerializationService CreateSerializationServiceMock<TMediaType>(ISerializer serializer)
        {
            var contextFactoryMock = this.CreateContextFactoryMock(() => new SerializationContext(Substitute.For<ICompositionContext>(), Substitute.For<ISerializationService>()));
            var serializationService = new DefaultSerializationService(
                contextFactoryMock,
                new List<IExportFactory<ISerializer, SerializerMetadata>>
                {
                    new ExportFactory<ISerializer, SerializerMetadata>(() => serializer, new SerializerMetadata(typeof(TMediaType))),
                });
            return serializationService;
        }

        public interface IContextFactoryAware { IContextFactory ContextFactory { get; } }
    }
}
