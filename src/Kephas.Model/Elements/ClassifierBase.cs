// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ClassifierBase.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Base abstract class for classifiers.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Model.Elements
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Reflection;

    using Kephas.Activation;
    using Kephas.Model.Construction;
    using Kephas.Model.Construction.Internal;
    using Kephas.Model.Elements.Annotations;
    using Kephas.Model.Resources;
    using Kephas.Reflection;
    using Kephas.Services;

    /// <summary>
    /// Base abstract class for classifiers.
    /// </summary>
    /// <typeparam name="TModelContract">The type of the model contract (the model interface).</typeparam>
    public abstract class ClassifierBase<TModelContract> : ModelElementBase<TModelContract>, IClassifier, ITypeInfo
        where TModelContract : IClassifier
    {
        /// <summary>
        /// The bases of this <see cref="ITypeInfo"/>. They include the real base and also the implemented interfaces.
        /// </summary>
        /// <value>
        /// The bases.
        /// </value>
        private IEnumerable<ITypeInfo> baseTypes;

        /// <summary>
        /// True if this object is a mixin.
        /// </summary>
        private bool? isMixin;

        /// <summary>
        /// True if this object is an aspect.
        /// </summary>
        private bool? isAspect;

        /// <summary>
        /// The function indicating whether this classifier is an aspect of other classifiers.
        /// </summary>
        private Func<IClassifier, bool>? isAspectOf;

        /// <summary>
        /// Initializes a new instance of the <see cref="ClassifierBase{TModelContract}" /> class.
        /// </summary>
        /// <param name="constructionContext">Context for the construction.</param>
        /// <param name="name">The name.</param>
        protected ClassifierBase(IModelConstructionContext constructionContext, string name)
            : base(constructionContext, name)
        {
            this.baseTypes = Array.Empty<ITypeInfo>();
            this.BaseMixins = Array.Empty<IClassifier>();
            this.GenericTypeArguments = Array.Empty<ITypeInfo>();
        }

        /// <summary>
        /// Gets or sets the projection where the model element is defined.
        /// </summary>
        /// <value>
        /// The projection.
        /// </value>
        public IModelProjection? Projection { get; protected internal set; } // TODO set the projection

        /// <summary>
        /// Gets the classifier properties.
        /// </summary>
        /// <value>
        /// The classifier properties.
        /// </value>
        public IEnumerable<IProperty> Properties => this.Members.OfType<IProperty>();

        /// <summary>
        /// Gets the classifier methods.
        /// </summary>
        /// <value>
        /// The classifier methods.
        /// </value>
        public IEnumerable<IMethod> Methods => this.Members.OfType<IMethod>();

        /// <summary>
        /// Gets a value indicating whether this classifier is a mixin.
        /// </summary>
        /// <value>
        /// <c>true</c> if this classifier is a mixin, <c>false</c> if not.
        /// </value>
        public bool IsMixin
            => // during construction, compute each time this flag, after that only once.
                this.ConstructionMonitor.IsInProgress
                    ? this.ComputeIsMixin()
                    : (this.isMixin ?? (this.isMixin = this.ComputeIsMixin()).Value);

        /// <summary>
        /// Gets the base classifier.
        /// </summary>
        /// <value>
        /// The base classifier.
        /// </value>
        public IClassifier? BaseClassifier { get; private set; }

        /// <summary>
        /// Gets the base mixins.
        /// </summary>
        /// <value>
        /// The base mixins.
        /// </value>
        public IEnumerable<IClassifier> BaseMixins { get; private set; }

        /// <summary>
        /// Gets a value indicating whether this classifier is an aspect of other classifiers.
        /// </summary>
        /// <returns>
        /// <c>true</c> if this classifier is an aspect of other classifiers, <c>false</c> if not.
        /// </returns>
        public bool IsAspect
            => // during construction, compute each time this flag, after that only once.
                this.ConstructionMonitor.IsInProgress
                    ? this.ComputeIsAspect()
                    : (this.isAspect ?? (this.isAspect = this.ComputeIsAspect()).Value);

        /// <summary>
        /// Gets the namespace of the type.
        /// </summary>
        /// <value>
        /// The namespace of the type.
        /// </value>
        public string Namespace => this.Projection?.FullName ?? string.Empty;

        /// <summary>
        /// Gets the bases of this <see cref="ITypeInfo"/>. They include the real base and also the implemented interfaces.
        /// </summary>
        /// <value>
        /// The bases.
        /// </value>
        IEnumerable<ITypeInfo> ITypeInfo.BaseTypes => this.baseTypes;

        /// <summary>
        /// Gets the container type registry.
        /// </summary>
        ITypeRegistry ITypeInfo.TypeRegistry => this.ModelSpace;

        /// <summary>
        /// Gets a read-only list of <see cref="ITypeInfo"/> objects that represent the type parameters of a generic type definition (open generic).
        /// </summary>
        /// <value>
        /// The generic arguments.
        /// </value>
        public IReadOnlyList<ITypeInfo> GenericTypeParameters { get; private set; } = Array.Empty<ITypeInfo>();

        /// <summary>
        /// Gets a read-only list of <see cref="ITypeInfo"/> objects that represent the type arguments of a closed generic type.
        /// </summary>
        /// <value>
        /// The generic arguments.
        /// </value>
        public IReadOnlyList<ITypeInfo> GenericTypeArguments { get; private set; }

        /// <summary>
        /// Gets a <see cref="ITypeInfo"/> object that represents a generic type definition from which the current generic type can be constructed.
        /// </summary>
        /// <value>
        /// The generic type definition.
        /// </value>
        public ITypeInfo? GenericTypeDefinition { get; private set; }

#if NETSTANDARD2_0
        /// <summary>
        /// Gets the enumeration of properties.
        /// </summary>
        IEnumerable<IPropertyInfo> ITypeInfo.Properties => this.Properties;

        /// <summary>
        /// Gets the members.
        /// </summary>
        /// <value>
        /// The members.
        /// </value>
        IEnumerable<IElementInfo> ITypeInfo.Members => this.Members;

        /// <summary>
        /// Gets a member by the provided name.
        /// </summary>
        /// <param name="name">The member name.</param>
        /// <param name="throwIfNotFound">True to throw if the requested member is not found.</param>
        /// <returns>
        /// The requested member, or <c>null</c>.
        /// </returns>
        IElementInfo? ITypeInfo.GetMember(string name, bool throwIfNotFound) => this.GetMember(name, throwIfNotFound);
#endif

        /// <summary>
        /// Indicates whether this classifier is an aspect of the provided classifier.
        /// </summary>
        /// <param name="classifier">The classifier.</param>
        /// <returns>
        /// <c>true</c> if this classifier is an aspect of the provided classifier, <c>false</c> if not.
        /// </returns>
        public virtual bool IsAspectOf(IClassifier classifier)
        {
            // during construction, compute each time the function, after that only once.
            if (this.ConstructionMonitor.IsInProgress)
            {
                return this.ComputeIsAspectOf()?.Invoke(classifier) ?? false;
            }

            if (this.isAspectOf == null)
            {
                this.isAspectOf = this.ComputeIsAspectOf() ?? (_ => false);
            }

            return this.isAspectOf(classifier);
        }

        /// <summary>
        /// Creates an instance with the provided arguments (if any).
        /// </summary>
        /// <param name="args">The arguments.</param>
        /// <returns>
        /// The new instance.
        /// </returns>
        public virtual object CreateInstance(IEnumerable<object?>? args = null)
        {
            throw new ActivationException(string.Format(Strings.ClassifierBase_CannotInstantiateAbstractTypeInfo_Exception, this, typeof(ITypeInfo), this));
        }

        /// <summary>
        /// Constructs a generic type baed on the provided type arguments.
        /// </summary>
        /// <param name="typeArguments">The type arguments.</param>
        /// <param name="constructionContext">The construction context (optional).</param>
        /// <returns>
        /// A constructed <see cref="ITypeInfo"/>.
        /// </returns>
        public ITypeInfo MakeGenericType(IEnumerable<ITypeInfo> typeArguments, IContext? constructionContext = null)
        {
            // TODO redirect generic type creation through ModelSpace
            throw new NotImplementedException();
        }

        /// <summary>
        /// Calculates the flag indicating whether the classifier is a mixin or not.
        /// </summary>
        /// <returns>
        /// <c>true</c> if the classifier is a mixin, <c>false</c> otherwise.
        /// </returns>
        protected virtual bool ComputeIsMixin()
        {
            return this.Members.OfType<MixinAnnotation>().Any() || this.Name.EndsWith("Mixin");
        }

        /// <summary>
        /// Calculates the flag indicating whether the classifier is an aspect or not.
        /// </summary>
        /// <returns>
        /// <c>true</c> if the classifier is an aspect, <c>false</c> otherwise.
        /// </returns>
        protected virtual bool ComputeIsAspect()
        {
            return this.Members.OfType<AspectAnnotation>().Any();
        }

        /// <summary>
        /// For an aspect, calculates the function to select the classifiers for which this classifier is an aspect.
        /// </summary>
        /// <returns>
        /// A function.
        /// </returns>
        protected virtual Func<IClassifier, bool>? ComputeIsAspectOf()
        {
            var aspectAnnotation = this.Members.OfType<AspectAnnotation>().FirstOrDefault();
            return aspectAnnotation?.IsAspectOf;
        }

        /// <summary>
        /// Called when the construction is complete.
        /// </summary>
        /// <param name="constructionContext">Context for the construction.</param>
        protected override void OnCompleteConstruction(IModelConstructionContext constructionContext)
        {
            var parts = this.Parts.OfType<ITypeInfo>().ToList();

            // compute base: types, classifier and mixins
            this.baseTypes = this.ComputeBaseTypes(constructionContext, parts);
            this.BaseClassifier = this.ComputeBaseClassifier(constructionContext, this.baseTypes);
            this.BaseMixins = this.ComputeBaseMixins(constructionContext, this.baseTypes);

            // compute generic arguments
            this.ComputeGenericInformation(constructionContext, parts);

            // add members from the bases
            this.AddMembersFromBaseClassifiers();

            base.OnCompleteConstruction(constructionContext);
        }

        /// <summary>
        /// Calculates the base types.
        /// </summary>
        /// <param name="constructionContext">Context for the construction.</param>
        /// <param name="parts">The parts.</param>
        /// <returns>
        /// The calculated base types.
        /// </returns>
        protected virtual IList<ITypeInfo> ComputeBaseTypes(IModelConstructionContext constructionContext, IList<ITypeInfo> parts)
        {
            var eligibleTypes = this.GetClassifierDependencies(constructionContext, parts);

            var baseTypesList = new List<ITypeInfo>();
            foreach (var eligibleType in eligibleTypes)
            {
                // TODO the next sentence should apply at any level.
                // ignore types which are already base for one of the eligible types.
                if (eligibleTypes.Any(t => t.BaseTypes.Contains(eligibleType)))
                {
                    continue;
                }

                // ignore open generics if a closed generic is a dependency
                if (eligibleType.IsGenericTypeDefinition()
                    && eligibleTypes.Any(t => t.GenericTypeDefinition == eligibleType))
                {
                    continue;
                }

                baseTypesList.Add(eligibleType);
            }

            return baseTypesList;
        }

        /// <summary>
        /// Calculates the base classifier.
        /// </summary>
        /// <param name="constructionContext">Context for the construction.</param>
        /// <param name="baseTypes">List of base types.</param>
        /// <returns>
        /// The calculated base classifier.
        /// </returns>
        protected virtual IClassifier? ComputeBaseClassifier(
            IModelConstructionContext constructionContext,
            IEnumerable<ITypeInfo> baseTypes)
        {
            var bases = baseTypes.OfType<IClassifier>().Where(c => !c.IsMixin).ToList();
            if (bases.Count == 0)
            {
                return null;
            }

            if (bases.Count > 1)
            {
                throw new AmbiguousMatchException(string.Format(Strings.ClassifierBase_AmbiguousBase_Exception, this, string.Join(", ", bases)));
            }

            return bases[0];
        }

        /// <summary>
        /// Calculates the base mixins.
        /// </summary>
        /// <param name="constructionContext">Context for the construction.</param>
        /// <param name="baseTypes">List of base types.</param>
        /// <returns>
        /// The calculated base mixins.
        /// </returns>
        protected virtual IEnumerable<IClassifier> ComputeBaseMixins(
            IModelConstructionContext constructionContext,
            IEnumerable<ITypeInfo> baseTypes)
        {
            return new ReadOnlyCollection<IClassifier>(baseTypes.OfType<IClassifier>().Where(c => c.IsMixin).ToList());
        }

        /// <summary>
        /// Calculates the generic information.
        /// </summary>
        /// <param name="constructionContext">Context for the construction.</param>
        /// <param name="parts">The parts.</param>
        private void ComputeGenericInformation(IModelConstructionContext constructionContext, List<ITypeInfo> parts)
        {
            var genericParts = parts.Where(p => p.IsGenericType()).ToList();
            if (genericParts.Count > 1)
            {
                throw new ModelConstructionException(
                    string.Format(
                        Strings.ClassifierBase_MultipleGenericPartsNotSupported_Exception,
                        this.FullName,
                        string.Join(", ", genericParts.Select(p => p.FullName))));
            }

            var genericPart = genericParts.Count == 1 ? genericParts[0] : null;
            if (genericPart != null && genericPart.IsGenericType())
            {
                this.GenericTypeParameters = genericPart.GenericTypeParameters;
                this.GenericTypeArguments = (genericPart as IClassifier)?.GenericTypeArguments
                                            ?? new ReadOnlyCollection<ITypeInfo>(
                                                genericPart.GenericTypeArguments
                                                    .Select(
                                                        t => this.ModelSpace.TryGetClassifier(
                                                                 t,
                                                                 findContext: constructionContext) ?? t).ToList());

                this.GenericTypeDefinition = genericPart.GenericTypeDefinition != null
                                                 ? this.ModelSpace.TryGetClassifier(
                                                     genericPart.GenericTypeDefinition,
                                                     findContext: constructionContext)
                                                 : null;
            }
        }

        /// <summary>
        /// Adds members from base classifiers.
        /// </summary>
        private void AddMembersFromBaseClassifiers()
        {
            // construct the base members list from the bases.
            var baseMembers = new Dictionary<string, object>();
            if (this.BaseClassifier != null)
            {
                foreach (var member in this.BaseClassifier.Members)
                {
                    baseMembers.Add(member.Name, member);
                }
            }

            foreach (var mixin in this.BaseMixins)
            {
                foreach (var member in mixin.Members.Where(m => m.IsInherited))
                {
                    if (baseMembers.TryGetValue(member.Name, out object conflictingMember))
                    {
                        // if the hierarchy brings the same member, take it only once
                        if (member == conflictingMember)
                        {
                            continue;
                        }

                        // add the conflicting members to a collection
                        if (conflictingMember is IList<INamedElement> collection)
                        {
                            collection.Add(member);
                        }
                        else
                        {
                            collection = new List<INamedElement> { (INamedElement)conflictingMember, member };
                            baseMembers[member.Name] = collection;
                        }
                    }
                    else
                    {
                        baseMembers.Add(member.Name, member);
                    }
                }
            }

            // add the base members
            foreach (var baseMemberMap in baseMembers)
            {
                var declaredMember = this.Members.FirstOrDefault(m => m.Name == baseMemberMap.Key);
                if (declaredMember != null)
                {
                    var ownMemberBuilder = declaredMember as IConstructibleElement;
                    if (baseMemberMap.Value is IList<INamedElement> collection)
                    {
                        foreach (var baseMember in collection)
                        {
                            ownMemberBuilder?.AddPart(baseMember);
                        }
                    }
                    else
                    {
                        ownMemberBuilder?.AddPart(baseMemberMap.Value);
                    }
                }
                else
                {
                    if (baseMemberMap.Value is IList<INamedElement> collection)
                    {
                        var resolvedMember = this.TryResolveConflictingMembers(collection);

                        if (resolvedMember == null)
                        {
                            throw new ModelConstructionException(
                                string.Format(
                                    Strings.ClassifierBase_ConflictingMembersInBases_Exception,
                                    baseMemberMap.Key,
                                    this.Name,
                                    string.Join(", ", collection.Select(e => ((IElementInfo)e).DeclaringContainer?.Name))));
                        }

                        this.AddMember(resolvedMember);
                    }
                    else
                    {
                        this.AddMember((INamedElement)baseMemberMap.Value);
                    }
                }
            }
        }

        /// <summary>
        /// Tries to resolve conflicting members and return the resolved member.
        /// </summary>
        /// <param name="conflictingMembers">The conflicting members.</param>
        /// <returns>
        /// The resolved member or <c>null</c>.
        /// </returns>
        private INamedElement? TryResolveConflictingMembers(IList<INamedElement> conflictingMembers)
        {
            var resolvedMember = conflictingMembers[0];
            for (var i = 1; i < conflictingMembers.Count; i++)
            {
                var challengingMember = conflictingMembers[i];

                // if the resolved member aggregates the challenging member,
                // the challenging member may be safely ignored.
                if (resolvedMember.Aggregates(challengingMember))
                {
                    continue;
                }

                // if the challenging member aggregates the resolved member,
                // consider the challenging the resolved one.
                if (challengingMember.Aggregates(resolvedMember))
                {
                    resolvedMember = challengingMember;
                    continue;
                }

                // there is no dependency between the two members, cannot solve the conflict.
                return null;
            }

            return resolvedMember;
        }
    }
}