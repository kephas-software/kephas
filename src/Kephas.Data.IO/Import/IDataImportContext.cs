// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IDataImportContext.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Declares the IDataImportContext interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Data.IO.Import
{
    using System;

    using Kephas.Data;
    using Kephas.Data.Commands;
    using Kephas.Data.Conversion;
    using Kephas.Data.IO;
    using Kephas.Diagnostics.Contracts;

    /// <summary>
    /// Contract for data import contexts.
    /// </summary>
    public interface IDataImportContext : IDataIOContext
    {
        /// <summary>
        /// Gets or sets the source data context.
        /// </summary>
        /// <value>
        /// The source data context.
        /// </value>
        IDataContext SourceDataContext { get; set; }

        /// <summary>
        /// Gets or sets the target data context.
        /// </summary>
        /// <value>
        /// The target data context.
        /// </value>
        IDataContext TargetDataContext { get; set; }

        /// <summary>
        /// Gets or sets the data conversion context configuration.
        /// </summary>
        /// <value>
        /// The data conversion context configuration.
        /// </value>
        Action<object, IDataConversionContext> DataConversionContextConfig { get; set; }

        /// <summary>
        /// Gets or sets the persist changes context configuration.
        /// </summary>
        /// <value>
        /// The persist changes context configuration.
        /// </value>
        Action<IPersistChangesContext> PersistChangesContextConfig { get; set; }
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

            if (!(self[ResultKey] is IDataIOResult result))
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