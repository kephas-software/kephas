// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IDataSourceProvider.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Declares the IDataSourceProvider interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Data.DataSources
{
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;

    using Kephas.Reflection;
    using Kephas.Services;

    /// <summary>
    /// Shared application service contract providing the data source for editors.
    /// </summary>
    [SharedAppServiceContract(AllowMultiple = true)]
    public interface IDataSourceProvider
    {
        /// <summary>
        /// Determines whether the provider can handle the list source request.
        /// </summary>
        /// <param name="propertyInfo">Information describing the property.</param>
        /// <param name="context">The context.</param>
        /// <returns>
        /// True if we can handle, false if not.
        /// </returns>
        bool CanHandle(IPropertyInfo propertyInfo, IDataSourceContext context);

        /// <summary>
        /// Gets the data source asynchronously.
        /// </summary>
        /// <param name="propertyInfo">Information describing the property.</param>
        /// <param name="context">The context.</param>
        /// <param name="cancellationToken">The cancellation token (optional).</param>
        /// <returns>
        /// The list source promise.
        /// </returns>
        Task<IEnumerable<object>> GetDataSourceAsync(
            IPropertyInfo propertyInfo,
            IDataSourceContext context,
            CancellationToken cancellationToken = default);
    }
}