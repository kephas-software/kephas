// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DefaultDataSetupManagerTest.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the default data setup manager test class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using Kephas.Injection;
using Kephas.Injection.ExportFactories;

namespace Kephas.Data.Tests.Setup
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;

    using Kephas.Composition;
    using Kephas.Data.Setup;
    using Kephas.Data.Setup.Composition;
    using Kephas.Operations;

    using NSubstitute;

    using NUnit.Framework;

    [TestFixture]
    public class DefaultDataSetupManagerTest : DataTestBase
    {
        [Test]
        public async Task InstallDataAsync_proper_order()
        {
            var ctxFactory = this.CreateContextFactoryMock(() => new DataSetupContext(Substitute.For<IInjector>()));
            var list = new List<string>();
            var installerFactories = this.GetInstallerFactories(list);
            var manager = new DefaultDataSetupManager(ctxFactory, installerFactories);
            var result = await manager.InstallDataAsync();

            Assert.AreEqual(2, list.Count);
            Assert.AreEqual("2", list[0]);
            Assert.AreEqual("1", list[1]);
        }

        [Test]
        public async Task InstallDataAsync_filtered()
        {
            var ctxFactory = this.CreateContextFactoryMock(() => new DataSetupContext(Substitute.For<IInjector>()));
            var list = new List<string>();
            var installerFactories = this.GetInstallerFactories(list);
            var manager = new DefaultDataSetupManager(ctxFactory, installerFactories);
            var result = await manager.InstallDataAsync(ctx => ctx.Targets(new[] { "1" }));

            Assert.AreEqual(1, list.Count);
            Assert.AreEqual("1", list[0]);
        }

        [Test]
        public async Task UninstallDataAsync_proper_order()
        {
            var ctxFactory = this.CreateContextFactoryMock(() => new DataSetupContext(Substitute.For<IInjector>()));
            var list = new List<string>();
            var installerFactories = this.GetInstallerFactories(list);
            var manager = new DefaultDataSetupManager(ctxFactory, installerFactories);
            var result = await manager.UninstallDataAsync();

            Assert.AreEqual(2, list.Count);
            Assert.AreEqual("u-1", list[0]);
            Assert.AreEqual("u-2", list[1]);
        }

        [Test]
        public async Task UninstallDataAsync_filtered()
        {
            var ctxFactory = this.CreateContextFactoryMock(() => new DataSetupContext(Substitute.For<IInjector>()));
            var list = new List<string>();
            var installerFactories = this.GetInstallerFactories(list);
            var manager = new DefaultDataSetupManager(ctxFactory, installerFactories);
            var result = await manager.UninstallDataAsync(ctx => ctx.Targets(new[] { "2" }));

            Assert.AreEqual(1, list.Count);
            Assert.AreEqual("u-2", list[0]);
        }

        private List<IExportFactory<IDataInstaller, DataInstallerMetadata>> GetInstallerFactories(List<string> list)
        {
            var installer1 = Substitute.For<IDataInstaller>();
            installer1.InstallDataAsync(Arg.Any<Action<IDataSetupContext>>(), Arg.Any<CancellationToken>()).Returns(
                ci =>
                    {
                        list.Add("1");
                        return Task.FromResult(Substitute.For<IOperationResult>());
                    });
            installer1.UninstallDataAsync(Arg.Any<Action<IDataSetupContext>>(), Arg.Any<CancellationToken>()).Returns(
                ci =>
                    {
                        list.Add("u-1");
                        return Task.FromResult(Substitute.For<IOperationResult>());
                    });

            var installer2 = Substitute.For<IDataInstaller>();
            installer2.InstallDataAsync(Arg.Any<Action<IDataSetupContext>>(), Arg.Any<CancellationToken>()).Returns(
                ci =>
                    {
                        list.Add("2");
                        return Task.FromResult(Substitute.For<IOperationResult>());
                    });
            installer2.UninstallDataAsync(Arg.Any<Action<IDataSetupContext>>(), Arg.Any<CancellationToken>()).Returns(
                ci =>
                    {
                        list.Add("u-2");
                        return Task.FromResult(Substitute.For<IOperationResult>());
                    });

            var installerFactories = new List<IExportFactory<IDataInstaller, DataInstallerMetadata>>
                                         {
                                             new ExportFactory<IDataInstaller, DataInstallerMetadata>(
                                                 () => installer1,
                                                 new DataInstallerMetadata("1", processingPriority: 2)),
                                             new ExportFactory<IDataInstaller, DataInstallerMetadata>(
                                                 () => installer2,
                                                 new DataInstallerMetadata("2", processingPriority: 1))
                                         };
            return installerFactories;
        }
    }
}