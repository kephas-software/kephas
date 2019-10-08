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
    using Kephas.Composition;
    using Kephas.Serialization;
    using Kephas.Services;
    using NSubstitute;

    public class TestBase
    {
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
            var serializationService = this.CreateSerializationServiceMock();
            serializationService.GetSerializer(Arg.Is<ISerializationContext>(ctx => ctx != null && ctx.MediaType == typeof(TMediaType)))
                .Returns(serializer);
            return serializationService;
        }

        public interface IContextFactoryAware { public IContextFactory ContextFactory { get; } }
    }
}
