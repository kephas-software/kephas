// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DataImportContext.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Implements the data import context class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Data.IO.Import
{
    using System;

    using Kephas.Data;
    using Kephas.Data.IO;
    using Kephas.Diagnostics.Contracts;

    /// <summary>
    /// A data import context.
    /// </summary>
    public class DataImportContext : DataIOContext, IDataImportContext
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DataImportContext"/> class.
        /// </summary>
        /// <param name="sourceDataContext">The source data context.</param>
        /// <param name="targetDataContext">The target data context.</param>
        public DataImportContext(IDataContext sourceDataContext, IDataContext targetDataContext)
        {
            Requires.NotNull(sourceDataContext, nameof(sourceDataContext));
            Requires.NotNull(targetDataContext, nameof(targetDataContext));

            this.SourceDataContext = sourceDataContext;
            this.TargetDataContext = targetDataContext;
        }

        /// <summary>
        /// Gets or sets the source data context.
        /// </summary>
        /// <value>
        /// The source data context.
        /// </value>
        public IDataContext SourceDataContext { get; set; }

        /// <summary>
        /// Gets or sets the target data context.
        /// </summary>
        /// <value>
        /// The target data context.
        /// </value>
        public IDataContext TargetDataContext { get; set; }
    }

    /// <summary>
    /// Extensions for <see cref="IDataImportContext"/>.
    /// </summary>
    public static class DataImportContextExtensions
    {
        /// <summary>
        /// The result key.
        /// </summary>
        private const string ResultKey = "SYSTEM_Result";

        /// <summary>
        /// Ensures that a result is set in the options.
        /// </summary>
        /// <param name="self">The self.</param>
        /// <param name="resultFactory">The result factory.</param>
        /// <returns>The result, once it is set into the options.</returns>
        public static IDataIOResult EnsureResult(this IDataImportContext self, Func<IDataIOResult> resultFactory = null)
        {
            Requires.NotNull(self, nameof(self));

            var result = self[ResultKey] as IDataIOResult;
            if (result == null)
            {
                resultFactory = resultFactory ?? (() => new DataIOResult());
                self[ResultKey] = result = resultFactory();
            }

            return result;
        }

        /// <summary>
        /// Gets the result from the options.
        /// </summary>
        /// <param name="self">The self.</param>
        /// <returns>The result, once it is set into the options.</returns>
        public static IDataIOResult GetResult(this IDataImportContext self)
        {
            Requires.NotNull(self, nameof(self));

            return self[ResultKey] as IDataIOResult;
        }
    }
}