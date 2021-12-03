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
        /// Sets the data context.
        /// </summary>
        /// <typeparam name="TContext">Type of the context.</typeparam>
        /// <param name="dataImportContext">The data import context.</param>
        /// <param name="dataSpace">The data space.</param>
        /// <returns>
        /// This <paramref name="dataImportContext"/>.
        /// </returns>
        public static TContext DataSpace<TContext>(
            this TContext dataImportContext,
            IDataSpace dataSpace)
            where TContext : class, IDataImportContext
        {
            dataImportContext = dataImportContext ?? throw new System.ArgumentNullException(nameof(dataImportContext));

            dataImportContext.DataSpace = dataSpace;

            return dataImportContext;
        }

        /// <summary>
        /// Sets the data conversion options configuration.
        /// </summary>
        /// <typeparam name="TContext">Type of the context.</typeparam>
        /// <param name="dataImportContext">The data import context.</param>
        /// <param name="optionsConfig">The data conversion options configuration.</param>
        /// <returns>
        /// This <paramref name="dataImportContext"/>.
        /// </returns>
        public static TContext SetDataConversionConfig<TContext>(
            this TContext dataImportContext,
            Action<object, IDataConversionContext> optionsConfig)
            where TContext : class, IDataImportContext
        {
            dataImportContext = dataImportContext ?? throw new System.ArgumentNullException(nameof(dataImportContext));

            dataImportContext.DataConversionConfig = optionsConfig;

            return dataImportContext;
        }

        /// <summary>
        /// Sets the persist changes options configuration.
        /// </summary>
        /// <typeparam name="TContext">Type of the context.</typeparam>
        /// <param name="dataImportContext">The data import context to act on.</param>
        /// <param name="optionsConfig">The persist changes options configuration.</param>
        /// <returns>
        /// This <paramref name="dataImportContext"/>.
        /// </returns>
        public static TContext SetPersistChangesConfig<TContext>(
            this TContext dataImportContext,
            Action<IPersistChangesContext> optionsConfig)
            where TContext : class, IDataImportContext
        {
            dataImportContext = dataImportContext ?? throw new System.ArgumentNullException(nameof(dataImportContext));

            dataImportContext.PersistChangesConfig = optionsConfig;

            return dataImportContext;
        }
    }
}