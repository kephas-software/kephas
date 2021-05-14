// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ITypeAssociationGraphProvider.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Data.Model.Associations
{
    using System;

    using Kephas.Graphs;
    using Kephas.Operations;
    using Kephas.Reflection;
    using Kephas.Services;

    /// <summary>
    /// Service for providing the association graph.
    /// </summary>
    [SingletonAppServiceContract]
    public interface ITypeAssociationGraphProvider
    {
        /// <summary>
        /// Gets an operation result yielding a graph of <see cref="ITypeInfo"/>.
        /// </summary>
        /// <param name="options">Optional. Options for configuring the graph.</param>
        /// <returns>An operation result yielding a graph of <see cref="ITypeInfo"/>.</returns>
        IOperationResult<Graph<ITypeInfo, ITypeAssociation>> GetAssociationGraph(Action<IAssociationContext>? options = null);
    }
}