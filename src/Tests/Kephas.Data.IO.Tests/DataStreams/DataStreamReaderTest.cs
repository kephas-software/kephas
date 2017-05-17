// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DataStreamReaderTest.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Implements the data stream reader test class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Data.IO.Tests.DataStreams
{
    using System.IO;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;

    using Kephas.Data.IO.DataStreams;
    using Kephas.Net.Mime;
    using Kephas.Serialization;

    using NSubstitute;

    using NUnit.Framework;

    [TestFixture]
    public class DataStreamReaderTest
    {
        [Test]
        public async Task ReadAsync()
        {
            var entity = "123";

            var mediaTypeProvider = Substitute.For<IMediaTypeProvider>();
            mediaTypeProvider.GetMediaType(Arg.Any<string>(), Arg.Any<bool>()).Returns(typeof(JsonMediaType));

            var serializer = Substitute.For<ISerializer>();
            serializer.DeserializeAsync(Arg.Any<TextReader>(), Arg.Any<ISerializationContext>(), Arg.Any<CancellationToken>())
                .Returns(entity);

            var serializationService = Substitute.For<ISerializationService>();
            serializationService.GetSerializer(Arg.Any<ISerializationContext>()).Returns(serializer);

            var reader = new DataStreamReader(serializationService, mediaTypeProvider);
            using (var dataStream = new DataStream(new MemoryStream(new byte[] { 0, 1, 2 }), "test", ownsStream: true))
            {
                var result = await reader.ReadAsync(dataStream);
                Assert.AreSame(entity, result);
            }
        }
    }
}