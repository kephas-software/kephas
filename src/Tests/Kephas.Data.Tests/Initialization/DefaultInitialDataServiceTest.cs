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
    using Kephas.Data.Setup;
    using Kephas.Data.Setup.Composition;

    using NSubstitute;

    using NUnit.Framework;

    [TestFixture]
    public class DefaultInitialDataServiceTest
    {
        [Test]
        public async Task CreateDataAsync_filter_initial_data_kinds()
        {
            var handler1 = Substitute.For<IDataInstaller>();
            var handler2 = Substitute.For<IDataInstaller>();
            var factories = new List<IExportFactory<IDataInstaller, DataInstallerMetadata>>
                                {
                                    new ExportFactory<IDataInstaller, DataInstallerMetadata>(() => handler1, new DataInstallerMetadata("system")),
                                    new ExportFactory<IDataInstaller, DataInstallerMetadata>(() => handler2, new DataInstallerMetadata("test")),
                                };
            var service = new DefaultDataSetupManager(factories);
            var context = Substitute.For<IDataSetupContext>();
            context.DataKinds.Returns((IEnumerable<string>)new[] { "test" });
            var result = await service.InstallDataAsync(context);

            handler1.Received(0).InstallDataAsync(context, Arg.Any<CancellationToken>());
            handler2.Received(1).InstallDataAsync(context, Arg.Any<CancellationToken>());
        }
    }
}