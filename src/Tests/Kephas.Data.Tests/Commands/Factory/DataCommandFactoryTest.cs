// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DataCommandFactoryTest.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Defines the DataCommandFactoryTest type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using Kephas.Injection;

namespace Kephas.Data.Tests.Commands.Factory
{
    using System.Collections.Generic;
    using Kephas.Data.Commands;
    using Kephas.Data.Commands.Factory;
    using Kephas.Services;
    using NSubstitute;

    using NUnit.Framework;

    [TestFixture]
    public class DataCommandFactoryTest
    {
        [Test]
        public void CreateCommand_success()
        {
            var cmd = Substitute.For<IDataCommand>();
            var factory = new DataCommandFactory<IDataCommand>(new List<IExportFactory<IDataCommand, DataCommandMetadata>>
                                                                   {
                                                                       new ExportFactory<IDataCommand, DataCommandMetadata>(() => cmd, new DataCommandMetadata(typeof(string)))
                                                                   });

            var actualCmd = factory.GetCommandFactory(typeof(string));
            Assert.AreSame(cmd, actualCmd());
        }

        [Test]
        public void CreateCommand_ambiguous_exception()
        {
            var cmd = Substitute.For<IDataCommand>();
            var betterCmd = Substitute.For<IDataCommand>();
            var factory = new DataCommandFactory<IDataCommand>(new List<IExportFactory<IDataCommand, DataCommandMetadata>>
                                                                   {
                                                                       new ExportFactory<IDataCommand, DataCommandMetadata>(() => cmd, new DataCommandMetadata(typeof(string))),
                                                                       new ExportFactory<IDataCommand, DataCommandMetadata>(() => betterCmd, new DataCommandMetadata(typeof(string)))
                                                                   });

            Assert.Throws<AmbiguousMatchDataException>(() => factory.GetCommandFactory(typeof(string)));
        }

        [Test]
        public void CreateCommand_respects_override_priority()
        {
            var cmd = Substitute.For<IDataCommand>();
            var betterCmd = Substitute.For<IDataCommand>();
            var factory = new DataCommandFactory<IDataCommand>(new List<IExportFactory<IDataCommand, DataCommandMetadata>>
                                                                   {
                                                                       new ExportFactory<IDataCommand, DataCommandMetadata>(() => cmd, new DataCommandMetadata(typeof(string))),
                                                                       new ExportFactory<IDataCommand, DataCommandMetadata>(() => betterCmd, new DataCommandMetadata(typeof(string), overridePriority: (Priority)(-100)))
                                                                   });

            var actualCmd = factory.GetCommandFactory(typeof(string));
            Assert.AreSame(betterCmd, actualCmd());
        }

        [Test]
        public void CreateCommand_NotSupported()
        {
            var cmd = Substitute.For<IDataCommand>();
            var factory = new DataCommandFactory<IDataCommand>(new List<IExportFactory<IDataCommand, DataCommandMetadata>>
                                                                   {
                                                                       new ExportFactory<IDataCommand, DataCommandMetadata>(() => cmd, new DataCommandMetadata(typeof(string)))
                                                                   });

            var cmdFactory = factory.GetCommandFactory(typeof(int));
            var actualCmd = cmdFactory();
            Assert.IsNull(actualCmd);
        }

        [Test]
        public void CreateCommand_NotSupported_no_exported_commands()
        {
            var factory = new DataCommandFactory<IDataCommand>(new List<IExportFactory<IDataCommand, DataCommandMetadata>>());
            var cmdFactory = factory.GetCommandFactory(typeof(int));
            var actualCmd = cmdFactory();
            Assert.IsNull(actualCmd);
        }

        [Test]
        public void CreateCommand_respects_most_specific_data_context_type()
        {
            var cmdBase = Substitute.For<ITestCommandBase>();
            var cmd = Substitute.For<ITestCommand>();
            var factory = new DataCommandFactory<IDataCommand>(new List<IExportFactory<IDataCommand, DataCommandMetadata>>
                                                                   {
                                                                       new ExportFactory<IDataCommand, DataCommandMetadata>(() => cmdBase, new DataCommandMetadata(typeof(ITestDataContextBase))),
                                                                       new ExportFactory<IDataCommand, DataCommandMetadata>(() => cmd, new DataCommandMetadata(typeof(ITestDataContext)))
                                                                   });

            var actualCmd = factory.GetCommandFactory(typeof(ITestDataContext));
            Assert.AreSame(cmd, actualCmd());
        }

        [Test]
        public void CreateCommand_finds_for_base_data_context()
        {
            var cmdBase = Substitute.For<ITestCommandBase>();
            var factory = new DataCommandFactory<IDataCommand>(new List<IExportFactory<IDataCommand, DataCommandMetadata>>
                                                                   {
                                                                       new ExportFactory<IDataCommand, DataCommandMetadata>(() => cmdBase, new DataCommandMetadata(typeof(ITestDataContextBase))),
                                                                   });

            var actualCmd = factory.GetCommandFactory(typeof(ITestDataContext));
            Assert.AreSame(cmdBase, actualCmd());
        }

        public interface ITestCommandBase : IDataCommand {}

        public interface ITestCommand : ITestCommandBase {}

        public interface ITestDataContextBase {}

        public interface ITestDataContext: ITestDataContextBase {}
    }
}