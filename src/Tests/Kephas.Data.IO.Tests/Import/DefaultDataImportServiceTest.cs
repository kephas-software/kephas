// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DefaultDataImportServiceTest.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Implements the default data import service test class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Data.IO.Tests.Import
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Threading;
    using System.Threading.Tasks;

    using Kephas.Data.Capabilities;
    using Kephas.Data.Commands;
    using Kephas.Data.Conversion;
    using Kephas.Data.IO.DataStreams;
    using Kephas.Data.IO.Import;

    using NSubstitute;

    using NUnit.Framework;

    [TestFixture]
    public class DefaultDataImportServiceTest
    {
        [Test]
        public async Task ImportDataAsync()
        {
            var reader = Substitute.For<IDataStreamReadService>();
            var conversionService = Substitute.For<IDataConversionService>();
            var resolver = Substitute.For<IDataImportProjectedTypeResolver>();

            var sourceDataContext = Substitute.For<IDataContext>();
            var targetDataContext = Substitute.For<IDataContext>();

            var changedTargetEntities = new List<IEntityInfo>();

            var service = new DefaultDataImportService(reader, conversionService, resolver);
            using (var dataStream = new DataStream(new MemoryStream(), ownsStream: true))
            {
                reader.ReadAsync(dataStream, Arg.Any<IDataIOContext>(), Arg.Any<CancellationToken>())
                    .Returns(Task.FromResult((object)"hello"));

                sourceDataContext.AttachEntity(Arg.Any<object>())
                    .Returns(ci => new EntityInfo(ci.Arg<object>()));
                targetDataContext.AttachEntity(Arg.Any<object>())
                    .Returns(
                        ci =>
                            {
                                var ei = new EntityInfo(ci.Arg<object>());
                                changedTargetEntities.Add(ei);
                                return ei;
                            });
                var persistChangesCommand = Substitute.For<IPersistChangesCommand>();
                targetDataContext.CreateCommand(typeof(IPersistChangesCommand))
                    .Returns(persistChangesCommand);
                resolver.ResolveProjectedType(Arg.Any<Type>(), Arg.Any<IDataImportContext>(), Arg.Any<bool>())
                    .Returns(ci => ci.Arg<Type>());
                conversionService.ConvertAsync("hello", Arg.Any<string>(), Arg.Any<IDataConversionContext>(), Arg.Any<CancellationToken>())
                    .Returns(Task.FromResult<IDataConversionResult>(DataConversionResult.FromTarget("kitty")));

                var context = new DataImportContext(sourceDataContext, targetDataContext);
                var result = await service.ImportDataAsync(dataStream, context);

                Assert.AreEqual(DataIOOperationState.CompletedSuccessfully, result.OperationState);
                Assert.AreEqual(0, result.Exceptions.Count);
                Assert.AreEqual(1, result.Messages.Count);
            }

            Assert.AreEqual(1, changedTargetEntities.Count);
            Assert.AreEqual("kitty", changedTargetEntities[0].Entity);
            Assert.AreEqual(ChangeState.AddedOrChanged, changedTargetEntities[0].ChangeState);
        }
    }
}