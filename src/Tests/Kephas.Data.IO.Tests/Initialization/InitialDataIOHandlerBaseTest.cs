// --------------------------------------------------------------------------------------------------------------------
// <copyright file="InitialDataIOHandlerBaseTest.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the initial data i/o handler base test class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Data.IO.Tests.Initialization
{
    using System;
    using System.Collections.Generic;
    using System.IO;

    using Kephas.Data.IO.DataStreams;
    using Kephas.Data.IO.Import;
    using Kephas.Data.IO.Initialization;
    using Kephas.Services;

    using NSubstitute;

    using NUnit.Framework;

    [TestFixture]
    public class InitialDataIOHandlerBaseTest
    {
        [Test]
        public void CreateDataSource_missing_file()
        {
            var handler = new InitialDataHandler();
            Assert.Throws<IOException>(() => handler.CreateDataSource("dummy file which does not exist"));
        }

        public class InitialDataHandler : InitialDataIOHandlerBase
        {
            private readonly IEnumerable<string> filePaths;

            public InitialDataHandler(IDataImportService dataImportService = null, Func<IContext, IDataContext> sourceDataContextProvider = null, Func<IContext, IDataContext> targetDataContextProvider = null, IEnumerable<string> filePaths = null)
                : base(
                    dataImportService ?? Substitute.For<IDataImportService>(),
                    sourceDataContextProvider ?? (ctx => Substitute.For<IDataContext>()),
                    targetDataContextProvider ?? (ctx => Substitute.For<IDataContext>()))
            {
                this.filePaths = filePaths;
            }

            /// <summary>
            /// Gets the data files to be imported.
            /// </summary>
            /// <returns>
            /// An enumerator that allows foreach to be used to process the data files in this collection.
            /// </returns>
            protected override IEnumerable<string> GetDataFilePaths()
            {
                return this.filePaths;
            }

            /// <summary>
            /// Creates a data source for the import operation.
            /// </summary>
            /// <param name="dataFilePath">The data file path.</param>
            /// <returns>
            /// The new data source.
            /// </returns>
            public new DataStream CreateDataSource(string dataFilePath)
            {
                return base.CreateDataSource(dataFilePath);
            }
        }
    }
}