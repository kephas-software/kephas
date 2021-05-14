// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EntityPermissionScopeResolverBase.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Data.Security.Permissions
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Kephas.Data.Model.Associations;
    using Kephas.Data.Reflection;
    using Kephas.Graphs;
    using Kephas.Reflection;
    using Kephas.Security.Permissions;
    using Kephas.Security.Permissions.AttributedModel;
    using Kephas.Services;

    /// <summary>
    /// Base service for permission scope resolver for entities.
    /// </summary>
    public abstract class EntityPermissionScopeResolverBase : IPermissionScopeResolver
    {
        /// <summary>
        /// The entity scope name key.
        /// </summary>
        private const string EntityScopeNameKey = "EntityScopeName";

        /// <summary>
        /// The entity scope type key.
        /// </summary>
        private const string EntityScopeTypeKey = "EntityScopeType";

        /// <summary>
        /// The entity scope path key.
        /// </summary>
        private const string EntityScopePathKey = "EntityScopePath";

        /// <summary>
        /// Initializes a new instance of the <see cref="EntityPermissionScopeResolverBase"/> class.
        /// </summary>
        /// <param name="typeRegistry">The type registry.</param>
        /// <param name="associationGraphProvider">The association graph provider.</param>
        /// <param name="contextFactory">The context factory.</param>
        protected EntityPermissionScopeResolverBase(
            ITypeRegistry typeRegistry,
            ITypeAssociationGraphProvider associationGraphProvider,
            IContextFactory contextFactory)
        {
            this.TypeRegistry = typeRegistry;
            this.AssociationGraphProvider = associationGraphProvider;
            this.ContextFactory = contextFactory;
        }

        /// <summary>
        /// Gets the type registry.
        /// </summary>
        protected ITypeRegistry TypeRegistry { get; }

        /// <summary>
        /// Gets the association graph provider.
        /// </summary>
        protected ITypeAssociationGraphProvider AssociationGraphProvider { get; }

        /// <summary>
        /// Gets the context factory.
        /// </summary>
        protected IContextFactory ContextFactory { get; }

        /// <summary>
        /// Tries to resolve the permission scope name of the provided type information,
        /// provided it is defined within a scope.
        /// </summary>
        /// <param name="typeInfo">The type information.</param>
        /// <param name="context">Optional. The operation context.</param>
        /// <returns>The permission scope name.</returns>
        public string? ResolveScopeName(ITypeInfo typeInfo, IContext? context = null)
        {
            if (typeInfo is not IEntityInfo entityInfo)
            {
                var attr = typeInfo.Annotations.OfType<IPermissionScopeAnnotation>().FirstOrDefault();
                return attr?.ScopeName;
            }

            if (typeInfo.HasDynamicMember(EntityScopeNameKey))
            {
                return typeInfo[EntityScopeNameKey] as string;
            }

            var scopeName = this.ComputeEntityScopeName(typeInfo);
            typeInfo[EntityScopeNameKey] = scopeName;
            return scopeName;
        }


        /// <summary>
        /// An IModelSpace extension method that calculates the entity scope name.
        /// </summary>
        /// <param name="e">The type to process.</param>
        /// <returns>
        /// The calculated entity scope name.
        /// </returns>
        protected virtual string? ComputeEntityScopeName(ITypeInfo e)
        {
            var scopeType = this.ComputeEntityScopeType(e);
            var attr = scopeType?.GetAttributes<Attribute>().OfType<IPermissionScopeAnnotation>().FirstOrDefault();
            string? scopeName = null;
            if (attr != null)
            {
                scopeName = attr.ScopeName;
                scopeName = string.IsNullOrEmpty(scopeName) ? scopeType!.Name : scopeName;
            }

            return scopeName;
        }

        /// <summary>
        /// An IModelSpace extension method that calculates the entity scope name.
        /// </summary>
        /// <param name="e">The type to process.</param>
        /// <returns>
        /// The calculated entity scope name.
        /// </returns>
        protected virtual ITypeInfo? ComputeEntityScopeType(ITypeInfo e)
        {
            var searchedEntities = new List<ITypeInfo>();
            var toSearch = new Queue<ITypeInfo>(new[] { e });
            var graph = this.GetEntityAssociationGraph();

            while (toSearch.Count > 0)
            {
                var current = toSearch.Dequeue();
                searchedEntities.Add(current);

                if (e.HasDynamicMember(EntityScopeTypeKey))
                {
                    return e[EntityScopeTypeKey] as IEntityInfo;
                }

                var attr = current?.GetAttributes<Attribute>().OfType<IPermissionScopeAnnotation>().FirstOrDefault();
                if (attr != null)
                {
                    return current;
                }

                var currentNode = graph.FindNodesByValue(current!).First();
                foreach (IGraphEdge<ITypeInfo, ITypeAssociation> edge in currentNode.OutgoingEdges)
                {
                    // the entity scope name is relevant only for aggregation & composition
                    var association = edge.Value;
                    if (association?.Kind is TypeAssociationKind.Aggregation or TypeAssociationKind.Composition)
                    {
                        var masterEntity = edge.To.Value;
                        if (!searchedEntities.Contains(masterEntity) && !toSearch.Contains(masterEntity))
                        {
                            toSearch.Enqueue(masterEntity);
                        }
                    }
                }
            }

            return e;
        }

        /// <summary>
        /// An IModelSpace extension method that gets entity association graph.
        /// </summary>
        /// <returns>
        /// The entity association graph.
        /// </returns>
        protected virtual Graph<ITypeInfo, ITypeAssociation> GetEntityAssociationGraph()
        {
            const string EntityAssociationGraphKey = "EntityAssociationGraph";
            if (this.TypeRegistry[EntityAssociationGraphKey] is Graph<ITypeInfo, ITypeAssociation> dependencyGraph)
            {
                return dependencyGraph;
            }

            dependencyGraph = this.AssociationGraphProvider
                .GetAssociationGraph(ctx =>
                {
                    ctx.TypeRegistry = this.TypeRegistry;
                    ctx.TypeFilter = (t, c) => t is IEntityInfo;
                })
                .Value;
            this.TypeRegistry[EntityAssociationGraphKey] = dependencyGraph;
            return dependencyGraph;
        }
    }
}