// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IDataImportContext.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
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
    using Kephas.Operations;

    /// <summary>
    /// Contract for data import contexts.
    /// </summary>
    public interface IDataImportContext : IDataIOContext
    {
        /// <summary>
        /// Gets or sets the data space.
        /// </summary>
        /// <value>
        /// The data space.
        /// </value>
        IDataSpace DataSpace { get; set; }

        /// <summary>
        /// Gets or sets the data conversion options configuration.
        /// </summary>
        /// <value>
        /// The data conversion options configuration.
        /// </value>
        Action<object, IDataConversionContext> DataConversionConfig { get; set; }

        /// <summary>
        /// Gets or sets the persist changes options configuration.
        /// </summary>
        /// <value>
        /// The persist changes options configuration.
        /// </value>
        Action<IPersistChangesContext> PersistChangesConfig { get; set; }
    }

    /// <summary>
    /// Extensions for <see cref="IDataImportContext"/>.
    /// </summary>
    public static class DataImportContextExtensions
    {
        /// <summary>
        /// The result key.
        /// </summary>
        private const string ResultKey = "Result";

        /// <summary>
        /// Ensures that a result is set in the options.
        /// </summary>
        /// <param name="self">The self.</param>
        /// <param name="resultFactory">The result factory.</param>
        /// <returns>The result, once it is set into the options.</returns>
        public static IOperationResult EnsureResult(this IDataImportContext self, Func<IOperationResult> resultFactory = null)
        {
            Requires.NotNull(self, nameof(self));

            if (!(self[ResultKey] is IOperationResult result))
            {
                resultFactory = resultFactory ?? (() => new OperationResult());
                self[ResultKey] = result = resultFactory();
            }

            return result;
        }

        /// <summary>
        /// Gets the result from the options.
        /// </summary>
        /// <param name="self">The self.</param>
        /// <returns>The result, once it is set into the options.</returns>
        public static IOperationResult GetResult(this IDataImportContext self)
        {
            Requires.NotNull(self, nameof(self));

            return self[ResultKey] as IOperationResult;
        }

        /// <summary>
        /// Sets the data context.
        /// </summary>
        /// <param name="dataImportContext">The data import context.</param>
        /// <param name="dataSpace">The data space.</param>
        /// <returns>
        /// This <paramref name="dataImportContext"/>.
        /// </returns>
        public static IDataImportContext DataSpace(
            this IDataImportContext dataImportContext,
            IDataSpace dataSpace)
        {
            Requires.NotNull(dataImportContext, nameof(dataImportContext));

            dataImportContext.DataSpace = dataSpace;

            return dataImportContext;
        }

        /// <summary>
        /// Sets the data conversion options configuration.
        /// </summary>
        /// <param name="dataImportContext">The data import context.</param>
        /// <param name="optionsConfig">The data conversion options configuration.</param>
        /// <returns>
        /// This <paramref name="dataImportContext"/>.
        /// </returns>
        public static IDataImportContext DataConversionConfig(
            this IDataImportContext dataImportContext,
            Action<object, IDataConversionContext> optionsConfig)
        {
            Requires.NotNull(dataImportContext, nameof(dataImportContext));

            dataImportContext.DataConversionConfig = optionsConfig;

            return dataImportContext;
        }

        /// <summary>
        /// Sets the persist changes options configuration.
        /// </summary>
        /// <param name="dataImportContext">The data import context to act on.</param>
        /// <param name="optionsConfig">The persist changes options configuration.</param>
        /// <returns>
        /// This <paramref name="dataImportContext"/>.
        /// </returns>
        public static IDataImportContext PersistChangesConfig(
            this IDataImportContext dataImportContext,
            Action<IPersistChangesContext> optionsConfig)
        {
            Requires.NotNull(dataImportContext, nameof(dataImportContext));

            dataImportContext.PersistChangesConfig = optionsConfig;

            return dataImportContext;
        }
    }
}