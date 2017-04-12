﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DataCommandFactoryTest.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Defines the DataCommandFactoryTest type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Data.Tests.Commands.Factory
{
    using System.Collections.Generic;

    using Kephas.Composition;
    using Kephas.Composition.ExportFactories;
    using Kephas.Data.Commands;
    using Kephas.Data.Commands.Composition;
    using Kephas.Data.Commands.Factory;

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
                                                                       new ExportFactory<IDataCommand, DataCommandMetadata>(() => betterCmd, new DataCommandMetadata(typeof(string), overridePriority: -100))
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
    }
}