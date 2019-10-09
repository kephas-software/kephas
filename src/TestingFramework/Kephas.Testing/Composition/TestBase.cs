// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TestBase.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the test base class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Testing.Composition
{
    using System;
    using Kephas.Composition;
    using Kephas.Serialization;
    using Kephas.Services;
    using NSubstitute;

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
        /// Creates a context factory mock.
        /// </summary>
        /// <typeparam name="TContext">Type of the context.</typeparam>
        /// <param name="ctor">The constructor for the created context.</param>
        /// <returns>
        /// The new context factory.
        /// </returns>
        protected IContextFactory CreateContextFactoryMock<TContext>(Func<TContext> ctor)
            where TContext : IContext
        {
            var contextFactory = Substitute.For<IContextFactory>();
            contextFactory.CreateContext<TContext>(Arg.Any<object[]>())
                .Returns(ci => ctor());
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
            var serializationService = Substitute.For<ISerializationService, IContextFactoryAware>(/*Behavior.Strict*/);
            var contextFactoryMock = this.CreateContextFactoryMock(() => new SerializationContext(Substitute.For<ICompositionContext>(), serializationService));
            ((IContextFactoryAware)serializationService).ContextFactory.Returns(contextFactoryMock);
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
            var serializationService = this.CreateSerializationServiceMock();
            serializationService.GetSerializer(Arg.Is<ISerializationContext>(ctx => ctx != null && ctx.MediaType == typeof(TMediaType)))
                .Returns(serializer);
            return serializationService;
        }

        public interface IContextFactoryAware { public IContextFactory ContextFactory { get; } }
    }
}
