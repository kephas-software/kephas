// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DefaultInitialDataServiceTest.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the default initial data service test class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Data.Tests.Initialization
{
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;

    using Kephas.Composition;
    using Kephas.Composition.ExportFactories;
    using Kephas.Data.Initialization;
    using Kephas.Data.Initialization.Composition;

    using NSubstitute;

    using NUnit.Framework;

    [TestFixture]
    public class DefaultInitialDataServiceTest
    {
        [Test]
        public async Task CreateDataAsync_filter_initial_data_kinds()
        {
            var handler1 = Substitute.For<IInitialDataHandler>();
            var handler2 = Substitute.For<IInitialDataHandler>();
            var factories = new List<IExportFactory<IInitialDataHandler, InitialDataHandlerMetadata>>
                                {
                                    new ExportFactory<IInitialDataHandler, InitialDataHandlerMetadata>(() => handler1, new InitialDataHandlerMetadata("system")),
                                    new ExportFactory<IInitialDataHandler, InitialDataHandlerMetadata>(() => handler2, new InitialDataHandlerMetadata("test")),
                                };
            var service = new DefaultInitialDataService(factories);
            var context = Substitute.For<IInitialDataContext>();
            context.InitialDataKinds.Returns((IEnumerable<string>)new[] { "test" });
            var result = await service.CreateDataAsync(context);

            handler1.Received(0).CreateDataAsync(context, Arg.Any<CancellationToken>());
            handler2.Received(1).CreateDataAsync(context, Arg.Any<CancellationToken>());
        }
    }
}