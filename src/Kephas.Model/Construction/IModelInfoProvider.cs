// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IModelInfoProvider.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
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
        Task<IEnumerable<IElementInfo>> GetElementInfosAsync(IModelConstructionContext constructionContext, CancellationToken cancellationToken = default(CancellationToken));
    }
}