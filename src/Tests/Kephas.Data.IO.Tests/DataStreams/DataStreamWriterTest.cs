// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DataStreamWriterTest.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Implements the data stream writer test class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Data.IO.Tests.DataStreams
{
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;

    using Kephas.Data.IO.DataStreams;
    using Kephas.Net.Mime;
    using Kephas.Serialization;

    using NSubstitute;

    using NUnit.Framework;

    [TestFixture]
    public class DataStreamWriterTest
    {
        [Test]
        public async Task WriteAsync()
        {
            var serializedEntity = "123";

            var mediaTypeProvider = Substitute.For<IMediaTypeProvider>();
            mediaTypeProvider.GetMediaType(Arg.Any<string>(), Arg.Any<bool>()).Returns(typeof(JsonMediaType));

            var serializer = Substitute.For<ISerializer>();
            serializer.When(s => s.SerializeAsync(Arg.Any<object>(), Arg.Any<TextWriter>(), Arg.Any<ISerializationContext>(), Arg.Any<CancellationToken>()))
                .Do(
                    ci =>
                        {
                            var textWriter = ci.Arg<TextWriter>();
                            textWriter.Write(serializedEntity);
                        });

            var serializationService = Substitute.For<ISerializationService>();
            serializationService.GetSerializer(Arg.Any<ISerializationContext>()).Returns(serializer);

            var writer = new DataStreamWriter(serializationService, mediaTypeProvider);
            var memStream = new MemoryStream();
            using (var dataStream = new DataStream(memStream, "test", ownsStream: true))
            {
                await writer.WriteAsync(new [] { "abcd" }, dataStream);
                var str = Encoding.UTF8.GetString(memStream.ToArray()).Substring(1); // cut the first unicode char
                Assert.AreEqual(serializedEntity, str);
            }
        }
    }
}