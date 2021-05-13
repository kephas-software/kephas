// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TypeAssociationGraphProvider.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Data.Model.Associations
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Kephas.Data.AttributedModel;
    using Kephas.Dynamic;
    using Kephas.Graphs;
    using Kephas.Operations;
    using Kephas.Reflection;
    using Kephas.Runtime;
    using Kephas.Services;

    /// <summary>
    /// Provides a graph of associations.
    /// </summary>
    public class TypeAssociationGraphProvider
    {
        /// <summary>
        /// The edge relation information key.
        /// </summary>
        internal const string EdgeRelationInfoKey = "EdgeRelationInfo";

        /// <summary>
        /// The navigation end key.
        /// </summary>
        private const string RelationshipRoleKey = "RelationshipRole";

        /// <summary>
        /// The edge name key.
        /// </summary>
        private const string EdgeNameKey = "EdgeName";

        private ITypeInfo? refClassifier;
        private ITypeInfo? collectionClassifier;

        /// <summary>
        /// Initializes a new instance of the <see cref="TypeAssociationGraphProvider"/> class.
        /// </summary>
        /// <param name="typeRegistry">The type registry.</param>
        /// <param name="contextFactory">The context factory.</param>
        public TypeAssociationGraphProvider(
            ITypeRegistry typeRegistry,
            IContextFactory contextFactory)
        {
            this.TypeRegistry = typeRegistry;
            this.ContextFactory = contextFactory;
        }

        /// <summary>
        /// Gets the type registry.
        /// </summary>
        protected ITypeRegistry TypeRegistry { get; }

        /// <summary>
        /// Gets the context factory.
        /// </summary>
        protected IContextFactory ContextFactory { get; }

        /// <summary>
        /// Gets an operation result yielding a graph of <see cref="ITypeInfo"/>.
        /// </summary>
        /// <param name="options">Optional. Options for configuring the graph.</param>
        /// <returns>An operation result yielding a graph of <see cref="ITypeInfo"/>.</returns>
        public IOperationResult<Graph<ITypeInfo>> GetAssociationGraph(Action<IAssociationContext>? options = null)
        {
            this.EnsureInitialized();

            using var associationContext = this.CreateAssociationContext(options);

            var graph = new Graph<ITypeInfo>();
            var result = new OperationResult<Graph<ITypeInfo>>(graph);

            var entityTypes = this.GetGraphTypes(associationContext).ToList();
            foreach (var entityType in entityTypes)
            {
                graph.AddNode(entityType);
            }


            foreach (var entityType in entityTypes)
            {
                foreach (var property in entityType.Properties)
                {
                    var role = this.TryGetAssociationRole(entityType, property, entityTypes);
                    if (role == null || !entityTypes.Contains(role.RefType))
                    {
                        continue;
                    }

                    var otherRole = this.TryGetOtherAssociationRole(role, entityTypes);
                    var relationInfo = new TypeAssociation(role, otherRole);
                    var fromType = relationInfo.DependentRole.Type;
                    var toType = relationInfo.PrimaryRole.Type;
                    var fromNode = graph.FindNodesByValue(fromType).First();
                    if (!fromNode.OutgoingEdges.Any(e => relationInfo.Name.Equals(e[EdgeNameKey])))
                    {
                        var edge = graph.AddEdge(fromType, toType);
                        edge[EdgeRelationInfoKey] = relationInfo;
                        edge[EdgeNameKey] = relationInfo.Name;
                    }
                }
            }

            return result.Complete();
        }

        /// <summary>
        /// Gets the types in the graph according to the provided contextual information.
        /// </summary>
        /// <param name="context">The association context.</param>
        /// <returns>An enumeration of <see cref="ITypeInfo"/>.</returns>
        protected virtual IEnumerable<ITypeInfo> GetGraphTypes(IAssociationContext context)
        {
            return context.TypeFilter == null
                ? this.TypeRegistry
                : this.TypeRegistry.Where(t => context.TypeFilter(t, context));
        }

        /// <summary>
        /// Creates the association context.
        /// </summary>
        /// <param name="options">Options for configuring the context.</param>
        /// <returns>A new instance implementing <see cref="IAssociationContext"/>.</returns>
        protected virtual IAssociationContext CreateAssociationContext(Action<IAssociationContext>? options)
        {
            return this.ContextFactory.CreateContext<AssociationContext>().Merge(options);
        }


        private ITypeAssociationRole? TryGetAssociationRole(ITypeInfo entityType, IPropertyInfo property, IEnumerable<ITypeInfo> entityTypes)
        {
            if (property.HasDynamicMember(RelationshipRoleKey))
            {
                return property[RelationshipRoleKey] as ITypeAssociationRole;
            }

            var propertyType = property.ValueType;
            var isRef = false;
            var isCollection = false;
            if (propertyType.IsConstructedGenericType())
            {
                if (propertyType.GenericTypeDefinition == this.refClassifier)
                {
                    propertyType = propertyType.GenericTypeArguments[0];
                    isRef = true;
                }
                else if (propertyType.GenericTypeDefinition == this.collectionClassifier)
                {
                    propertyType = propertyType.GenericTypeArguments[0];
                    isCollection = true;
                }
            }

            if (!entityTypes.Contains(propertyType))
            {
                property[RelationshipRoleKey] = null;
                return null;
            }

            var partAttr = property.GetAttribute<EntityPartAttribute>();
            var partKind = partAttr?.Kind;
            var navEnd = new TypeAssociationRole(entityType, property)
            {
                RefType = propertyType,
                RoleKind = isRef || isCollection ? this.GetRoleKind(partKind) : TypeAssociationKind.Composition,
                IsCollection = isCollection,
            };
            property[RelationshipRoleKey] = navEnd;
            return navEnd;
        }

        private ITypeAssociationRole? TryGetOtherAssociationRole(ITypeAssociationRole role, IEnumerable<ITypeInfo> entityTypes)
        {
            foreach (var property in role.RefType.Properties)
            {
                var otherRole = this.TryGetAssociationRole(role.RefType, property, entityTypes);
                if (otherRole?.Property == role.Property)
                {
                    // self referencing entities - ignore
                    continue;
                }

                // TODO what if there is a need to disambiguate
                // like two Employee references: DirectChef, Tutor
                if (otherRole != null && otherRole.RefType == role.Type)
                {
                    return otherRole;
                }
            }

            return null;
        }

        /// <summary>
        /// Gets the role kind.
        /// </summary>
        /// <param name="partKind">The part kind.</param>
        /// <returns>
        /// The role kind.
        /// </returns>
        private TypeAssociationKind GetRoleKind(EntityPartKind? partKind)
        {
            return partKind == EntityPartKind.Structural
                ? TypeAssociationKind.Composition
                : partKind == EntityPartKind.Loose
                    ? TypeAssociationKind.Aggregation
                    : TypeAssociationKind.Simple;
        }

        private void EnsureInitialized()
        {
            if (this.refClassifier != null)
            {
                return;
            }

            lock (this.TypeRegistry)
            {
                if (this.refClassifier != null)
                {
                    return;
                }

                this.refClassifier = this.TypeRegistry.GetTypeInfo(typeof(IRef<>), throwOnNotFound: false);
                this.collectionClassifier =
                    this.TypeRegistry.GetTypeInfo(typeof(ICollection<>), throwOnNotFound: false);
            }
        }
    }
}