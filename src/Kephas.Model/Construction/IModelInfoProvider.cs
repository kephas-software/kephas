// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IModelInfoProvider.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Contract for providers of element infos.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Model.Construction
{
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;

    using Kephas.Reflection;
    using Kephas.Services;

    /// <summary>
    /// Contract for providers of element infos.
    /// </summary>
    [SharedAppServiceContract(AllowMultiple = true)]
    public interface IModelInfoProvider
    {
        /// <summary>
        /// Gets the element infos used for building the model space.
        /// </summary>
        /// <param name="constructionContext">Context for the construction.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>
        /// An awaitable task promising an enumeration of element information.
        /// </returns>
        Task<IEnumerable<IElementInfo>> GetElementInfosAsync(IModelConstructionContext constructionContext, CancellationToken cancellationToken = default);

        /// <summary>
        /// Tries to get an <see cref="IElementInfo"/> based on the provided native element information.
        /// </summary>
        /// <param name="nativeElementInfo">The native element information.</param>
        /// <param name="constructionContext">Context for the construction.</param>
        /// <returns>
        /// The constructed generic type or <c>null</c> if the provider cannot handle the provided type information.
        /// </returns>
        /// <remarks>
        /// A return value of <c>null</c> indicates only that the provided <paramref name="nativeElementInfo"/> cannot be handled.
        /// For any other errors an exception should be thrown.
        /// </remarks>
        IElementInfo TryGetElementInfo(IElementInfo nativeElementInfo, IModelConstructionContext constructionContext);
    }
}