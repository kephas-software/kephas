﻿namespace Kephas.Injection
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;

    using Kephas.Services;
    using Kephas.Threading.Tasks;

    /// <summary>
    /// Extension methods for <see cref="IExportFactory{T}"/>.
    /// </summary>
    public static class ExportFactoryExtensions
    {
        /// <summary>
        /// Convenience method that creates the exported value.
        /// </summary>
        /// <typeparam name="T">The exported value type.</typeparam>
        /// <param name="exportFactory">The export factory.</param>
        /// <returns>
        /// The exported value.
        /// </returns>
        public static T CreateExportedValue<T>(this IExportFactory<T> exportFactory)
        {
            exportFactory = exportFactory ?? throw new ArgumentNullException(nameof(exportFactory));

            return exportFactory.CreateExport().Value;
        }

        /// <summary>
        /// Convenience method that creates the exported value and initializes it.
        /// </summary>
        /// <typeparam name="T">The exported value type.</typeparam>
        /// <param name="exportFactory">The export factory.</param>
        /// <param name="context">The context.</param>
        /// <returns>
        /// The exported value.
        /// </returns>
        public static T CreateInitializedValue<T>(
            this IExportFactory<T> exportFactory,
            IContext? context)
            where T : class
        {
            exportFactory = exportFactory ?? throw new ArgumentNullException(nameof(exportFactory));
            context = context ?? throw new ArgumentNullException(nameof(context));

            var export = exportFactory.CreateExport().Value;
            ServiceHelper.Initialize(export, context);
            return export;
        }

        /// <summary>
        /// Convenience method that creates the exported value and initializes it asynchronously.
        /// </summary>
        /// <typeparam name="T">The exported value type.</typeparam>
        /// <param name="exportFactory">The export factory.</param>
        /// <param name="context">The context.</param>
        /// <param name="cancellationToken">Optional. The cancellation token.</param>
        /// <returns>
        /// The exported value.
        /// </returns>
        public static async Task<T> CreateInitializedValueAsync<T>(
            this IExportFactory<T> exportFactory,
            IContext? context,
            CancellationToken cancellationToken = default)
            where T : class
        {
            exportFactory = exportFactory ?? throw new ArgumentNullException(nameof(exportFactory));
            context = context ?? throw new ArgumentNullException(nameof(context));

            var export = exportFactory.CreateExport().Value;
            await ServiceHelper.InitializeAsync(export, context, cancellationToken).PreserveThreadContext();
            return export;
        }
    }
}