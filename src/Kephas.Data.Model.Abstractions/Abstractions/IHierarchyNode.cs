// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IHierarchyNode.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Declares the IHierarchyNode interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Data.Model.Abstractions
{
    using System.ComponentModel.DataAnnotations;

    using Kephas.Data.Model.Abstractions.Resources;
    using Kephas.Model.AttributedModel;

    /// <summary>
    /// Interface for hierarchy node.
    /// </summary>
    /// <typeparam name="TId">The type of the ID.</typeparam>
    /// <typeparam name="TEntity">The type of the entity.</typeparam>
    [Mixin]
    public interface IHierarchyNode<TId, TEntity> : IIdentifiable<TId>
        where TEntity : class, IIdentifiable<TId>
    {
        /// <summary>
        /// Gets the parent reference.
        /// </summary>
        /// <value>
        /// The parent reference.
        /// </value>
        [Display(ResourceType = typeof(ModelStrings), Name = "HierarchyNode_ParentRef_Name")]
        IRef<TEntity> ParentRef { get; }
    }
}