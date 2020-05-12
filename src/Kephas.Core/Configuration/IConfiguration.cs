// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IConfiguration.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Declares the IConfiguration interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Configuration
{
    using System.Threading;
    using System.Threading.Tasks;

    using Kephas.Dynamic;
    using Kephas.Operations;
    using Kephas.Services;

    /// <summary>
    /// Singleton service contract for getting.
    /// </summary>
    /// <typeparam name="TSettings">Type of the settings.</typeparam>
    [SingletonAppServiceContract(AsOpenGeneric = true)]
    public interface IConfiguration<TSettings> : IExpando
        where TSettings : class, new()
    {
        /// <summary>
        /// Gets the settings associated to this configuration.
        /// </summary>
        /// <value>
        /// The settings.
        /// </value>
        TSettings Settings { get; }

        /// <summary>
        /// Updates the settings in the configuration store.
        /// </summary>
        /// <param name="settings">Optional. The settings to be updated. If no settings are provided, the current settings are used for the update.</param>
        /// <param name="cancellationToken">Optional. The cancellation token.</param>
        /// <returns>
        /// A <see cref="Task"/> representing the asynchronous operation yielding an operation result
        /// with a true value in case of successful update and a false value if the settings could not be updated.
        /// </returns>
        Task<IOperationResult<bool>> UpdateSettingsAsync(
            TSettings? settings = null,
            CancellationToken cancellationToken = default);
    }
}