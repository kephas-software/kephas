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
    using Kephas.Data.Reflection;
    using Kephas.Dynamic;
    using Kephas.Graphs;
    using Kephas.Operations;
    using Kephas.Reflection;
    using Kephas.Runtime;
    using Kephas.Services;

    /// <summary>
    /// Provides a graph of associations.
    /// </summary>
    [OverridePriority(Priority.Low)]
    public class TypeAssociationGraphProvider : ITypeAssociationGraphProvider
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TypeAssociationGraphProvider"/> class.
        /// </summary>
        /// <param name="injectableFactory">The injectable factory.</param>
        public TypeAssociationGraphProvider(IInjectableFactory injectableFactory)
        {
            this.InjectableFactory = injectableFactory;
        }

        /// <summary>
        /// Gets the context factory.
        /// </summary>
        protected IInjectableFactory InjectableFactory { get; }

        /// <summary>
        /// Gets an operation result yielding a graph of <see cref="ITypeInfo"/>.
        /// </summary>
        /// <param name="options">Optional. Options for configuring the graph.</param>
        /// <returns>An operation result yielding a graph of <see cref="ITypeInfo"/>.</returns>
        public IOperationResult<Graph<ITypeInfo, ITypeAssociation>> GetAssociationGraph(Action<IAssociationContext>? options = null)
        {
            using var context = this.CreateAssociationContext(options);
            this.EnsureInitialized(context);

            var graph = new Graph<ITypeInfo, ITypeAssociation>();
            var result = new OperationResult<Graph<ITypeInfo, ITypeAssociation>>(graph);

            var entityTypes = this.GetGraphTypes(context).ToList();
            foreach (var entityType in entityTypes)
            {
                graph.AddNode(entityType);
            }

            var rolesCache = new Dictionary<IPropertyInfo, ITypeAssociationRole>();
            foreach (var entityType in entityTypes)
            {
                foreach (var property in entityType.Properties)
                {
                    var role = this.TryGetAssociationRole(context, entityType, property, entityTypes, rolesCache);
                    if (role == null || !entityTypes.Contains(role.RefType))
                    {
                        continue;
                    }

                    var otherRole = this.TryGetOtherAssociationRole(context, role, entityTypes, rolesCache);
                    var association = new TypeAssociation(role, otherRole);
                    var fromType = association.DependentRole.Type;
                    var toType = association.PrimaryRole.Type;
                    var fromNode = graph.FindNodesByValue(fromType).First();
                    if (fromNode.OutgoingEdges.OfType<IGraphEdge<ITypeInfo, ITypeAssociation>>().All(e => association.Name != e.Value?.Name))
                    {
                        var edge = graph.AddEdge(fromType, toType, association);
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
                ? context.TypeRegistry!
                : context.TypeRegistry!.Where(t => context.TypeFilter(t, context));
        }

        /// <summary>
        /// Creates the association context.
        /// </summary>
        /// <param name="options">Options for configuring the context.</param>
        /// <returns>A new instance implementing <see cref="IAssociationContext"/>.</returns>
        protected virtual IAssociationContext CreateAssociationContext(Action<IAssociationContext>? options)
        {
            return this.InjectableFactory.Create<AssociationContext>().Merge(options);
        }

        /// <summary>
        /// Gets the association role data from the property information.
        /// </summary>
        /// <param name="context">The association context.</param>
        /// <param name="property">The property information.</param>
        /// <returns>A tuple containing the referenced type and the multiplicity.</returns>
        protected virtual (ITypeInfo refType, TypeAssociationMultiplicity multiplicity) GetAssociationRoleData(IAssociationContext context, IPropertyInfo property)
        {
            var refType = property.ValueType;
            var multiplicity = TypeAssociationMultiplicity.Unspecified;
            if (property is IRefPropertyInfo refProperty)
            {
                refType = refProperty.RefType;
                multiplicity = TypeAssociationMultiplicity.One;
            }
            else if (refType.IsConstructedGenericType())
            {
                if (refType.GenericTypeDefinition == context.RefTypeInfo)
                {
                    refType = refType.GenericTypeArguments[0];
                    multiplicity = TypeAssociationMultiplicity.One;
                }
                else if (refType.GenericTypeDefinition == context.CollectionTypeInfo)
                {
                    refType = refType.GenericTypeArguments[0];
                    multiplicity = TypeAssociationMultiplicity.Many;
                }
            }

            return (refType, multiplicity);
        }

        private ITypeAssociationRole? TryGetAssociationRole(
            IAssociationContext context,
            ITypeInfo entityType,
            IPropertyInfo property,
            IEnumerable<ITypeInfo> entityTypes,
            IDictionary<IPropertyInfo, ITypeAssociationRole> rolesCache)
        {
            if (rolesCache.TryGetValue(property, out var role))
            {
                return role;
            }

            var (refType, multiplicity) = this.GetAssociationRoleData(context, property);

            if (!entityTypes.Contains(refType))
            {
                rolesCache.Remove(property);
                return null;
            }

            var partAttr = property.GetAttribute<EntityPartAttribute>();
            var partKind = partAttr?.Kind;
            role = new TypeAssociationRole(entityType, property)
            {
                RefType = refType,
                RoleKind = multiplicity != TypeAssociationMultiplicity.Unspecified
                    ? this.GetRoleKind(partKind)
                    : TypeAssociationKind.Composition,
                Multiplicity = multiplicity,
            };

            rolesCache[property] = role;
            return role;
        }

        private ITypeAssociationRole? TryGetOtherAssociationRole(
            IAssociationContext context,
            ITypeAssociationRole role,
            IEnumerable<ITypeInfo> entityTypes,
            IDictionary<IPropertyInfo, ITypeAssociationRole> rolesCache)
        {
            foreach (var property in role.RefType.Properties)
            {
                var otherRole = this.TryGetAssociationRole(context, role.RefType, property, entityTypes, rolesCache);
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

        private void EnsureInitialized(IAssociationContext context)
        {
            context.TypeRegistry = context.TypeRegistry ?? throw new ArgumentNullException(nameof(context.TypeRegistry));
            var typeRegistry = context.TypeRegistry;

            context.RefTypeInfo ??= typeRegistry.GetTypeInfo(typeof(IRef<>), throwOnNotFound: false);
            context.CollectionTypeInfo ??= typeRegistry.GetTypeInfo(typeof(ICollection<>), throwOnNotFound: false);
        }
    }
}