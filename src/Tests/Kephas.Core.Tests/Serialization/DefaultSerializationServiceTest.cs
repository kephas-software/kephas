// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DefaultSerializationServiceTest.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Tests for <see cref="DefaultSerializationService" />
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Core.Tests.Serialization
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;

    using Kephas.Composition;
    using Kephas.Serialization;
    using Kephas.Serialization.Composition;
    using Kephas.Serialization.Json;
    using Kephas.Serialization.Xml;
    using Kephas.Services;

    using NSubstitute;

    using NUnit.Framework;

    /// <summary>
    /// Tests for <see cref="DefaultSerializationService"/>
    /// </summary>
    [TestFixture]
    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented", Justification = "Reviewed. Suppression is OK here.")]
    public class DefaultSerializationServiceTest
    {
        [Test]
        [TestCase(typeof(XmlFormat))]
        [TestCase(typeof(JsonFormat))]
        public void GetSerializer_WithContext_Exception(Type formatType)
        {
            var serializationService = new DefaultSerializationService(Substitute.For<IAmbientServices>(), new List<IExportFactory<ISerializer, SerializerMetadata>>());
            var context = new SerializationContext(serializationService, formatType);
            Assert.Throws<KeyNotFoundException>(() => serializationService.GetSerializer(context));
        }

        [Test]
        public void GetSerializer_NoContext_Exception()
        {
            var serializationService = new DefaultSerializationService(Substitute.For<IAmbientServices>(), new List<IExportFactory<ISerializer, SerializerMetadata>>());
            Assert.Throws<KeyNotFoundException>(() => serializationService.GetSerializer());
        }

        [Test]
        public void GetSerializer()
        {
            var factories = new List<IExportFactory<ISerializer, SerializerMetadata>>();
            factories.Add(this.GetSerializerFactory(typeof(JsonFormat)));
            var serializationService = new DefaultSerializationService(Substitute.For<IAmbientServices>(), factories);
            var serializer = serializationService.GetSerializer();
            Assert.IsNotNull(serializer);
        }

        [Test]
        public void GetSerializer_WithOverride()
        {
            var factories = new List<IExportFactory<ISerializer, SerializerMetadata>>();
            var oldSerializer = Substitute.For<ISerializer>();
            var newSerializer = Substitute.For<ISerializer>();
            factories.Add(this.GetSerializerFactory(typeof(JsonFormat), oldSerializer, Priority.Normal));
            factories.Add(this.GetSerializerFactory(typeof(JsonFormat), newSerializer, Priority.AboveNormal));
            var serializationService = new DefaultSerializationService(Substitute.For<IAmbientServices>(), factories);
            var serializer = serializationService.GetSerializer();
            Assert.AreSame(serializer, newSerializer);
        }

        private IExportFactory<ISerializer, SerializerMetadata> GetSerializerFactory(
            Type formatType,
            ISerializer serializer = null,
            Priority overridePriority = Priority.Normal)
        {
            var metadata = new SerializerMetadata(formatType, overridePriority: (int)overridePriority);
            serializer = serializer ?? Substitute.For<ISerializer>();
            var export = Substitute.For<IExport<ISerializer, SerializerMetadata>>();
            export.Value.Returns(serializer);
            export.Metadata.Returns(metadata);

            var factory = Substitute.For<IExportFactory<ISerializer, SerializerMetadata>>();
            factory.Metadata.Returns(metadata);
            factory.CreateExport().Returns(export);

            return factory;
        }
    }
}