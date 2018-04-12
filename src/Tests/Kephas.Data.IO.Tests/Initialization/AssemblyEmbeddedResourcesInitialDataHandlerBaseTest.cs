// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AssemblyEmbeddedResourcesInitialDataHandlerBaseTest.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the assembly embedded resources initial data handler base test class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Data.IO.Tests.Initialization
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Kephas.Data.IO.DataStreams;
    using Kephas.Data.IO.Import;
    using Kephas.Data.IO.Initialization;
    using Kephas.Services;

    using NSubstitute;

    using NUnit.Framework;

    [TestFixture]
    public class AssemblyEmbeddedResourcesInitialDataHandlerBaseTest
    {
        [Test]
        public void GetDataFilePaths_explicit_file_names()
        {
            var handler = new AssemblyEmbeddedInitialDataHandler(fileNames: new [] { "my-embedded-data.json" });
            var filePaths = handler.GetDataFilePaths().ToList();
            Assert.AreEqual(1, filePaths.Count);
            Assert.AreEqual("Kephas.Data.IO.Tests.Data.my-embedded-data.json", filePaths[0]);
        }

        [Test]
        public void GetDataFilePaths_implicit_all_file_names()
        {
            var handler = new AssemblyEmbeddedInitialDataHandler();
            var filePaths = handler.GetDataFilePaths().ToList();
            Assert.AreEqual(2, filePaths.Count);
            Assert.AreEqual("Kephas.Data.IO.Tests.Data.my-embedded-data.json", filePaths[0]);
            Assert.AreEqual("Kephas.Data.IO.Tests.Data.my-embedded-data-2.json", filePaths[1]);
        }

        [Test]
        public void CreateDataSource_missing_resource()
        {
            var handler = new AssemblyEmbeddedInitialDataHandler();
            Assert.Throws<ArgumentException>(() => handler.CreateDataSource("Kephas.Data.IO.Tests.Data.my-data.json"));
        }

        public class AssemblyEmbeddedInitialDataHandler : AssemblyEmbeddedResourcesInitialDataHandlerBase
        {
            private readonly IEnumerable<string> fileNames;

            public AssemblyEmbeddedInitialDataHandler(IDataImportService dataImportService = null, Func<IContext, IDataContext> sourceDataContextProvider = null, Func<IContext, IDataContext> targetDataContextProvider = null, IEnumerable<string> fileNames = null)
                : base(
                    dataImportService ?? Substitute.For<IDataImportService>(),
                    sourceDataContextProvider ?? (ctx => Substitute.For<IDataContext>()),
                    targetDataContextProvider ?? (ctx => Substitute.For<IDataContext>()))
            {
                this.fileNames = fileNames;
            }

            protected override IEnumerable<string> GetDataFileNames()
            {
                return this.fileNames;
            }

            public new IEnumerable<string> GetDataFilePaths()
            {
                return base.GetDataFilePaths();
            }

            public new DataStream CreateDataSource(string dataFilePath)
            {
                return base.CreateDataSource(dataFilePath);
            }
        }
    }
}