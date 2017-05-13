// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ClassifierBase.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
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

    using Kephas.Activation;
    using Kephas.Model.Construction;
    using Kephas.Model.Elements.Annotations;
    using Kephas.Model.Resources;
    using Kephas.Reflection;

    /// <summary>
    /// Base abstract class for classifiers.
    /// </summary>
    /// <typeparam name="TModelContract">The type of the model contract (the model interface).</typeparam>
    public abstract class ClassifierBase<TModelContract> : ModelElementBase<TModelContract>, IClassifier
        where TModelContract : IClassifier
    {
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
        private Func<IClassifier, bool> isAspectOf;

        /// <summary>
        /// Initializes a new instance of the <see cref="ClassifierBase{TModelContract}" /> class.
        /// </summary>
        /// <param name="constructionContext">Context for the construction.</param>
        /// <param name="name">The name.</param>
        protected ClassifierBase(IModelConstructionContext constructionContext, string name)
            : base(constructionContext, name)
        {
            this.BaseTypes = ModelHelper.EmptyClassifiers;
            this.BaseMixins = ModelHelper.EmptyClassifiers;
            this.GenericTypeArguments = ModelHelper.EmptyClassifiers;
        }

        /// <summary>
        /// Gets the projection where the model element is defined.
        /// </summary>
        /// <value>
        /// The projection.
        /// </value>
        public IModelProjection Projection { get; } // TODO set the projection

        /// <summary>
        /// Gets the classifier properties.
        /// </summary>
        /// <value>
        /// The classifier properties.
        /// </value>
        public IEnumerable<IProperty> Properties => this.Members.OfType<IProperty>();

        /// <summary>
        /// Gets the members.
        /// </summary>
        /// <value>
        /// The members.
        /// </value>
        IEnumerable<IElementInfo> ITypeInfo.Members => this.Members;

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
        public IClassifier BaseClassifier { get; private set; }

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
        public string Namespace => this.Projection?.FullName;

        /// <summary>
        /// Gets the bases of this <see cref="ITypeInfo"/>. They include the real base and also the implemented interfaces.
        /// </summary>
        /// <value>
        /// The bases.
        /// </value>
        public IEnumerable<ITypeInfo> BaseTypes { get; private set; }

        /// <summary>
        /// Gets a read-only list of <see cref="ITypeInfo"/> objects that represent the type parameters of a generic type definition (open generic).
        /// </summary>
        /// <value>
        /// The generic arguments.
        /// </value>
        public IReadOnlyList<ITypeInfo> GenericTypeParameters { get; private set; }

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
        public ITypeInfo GenericTypeDefinition { get; private set; }

        /// <summary>
        /// Gets the enumeration of properties.
        /// </summary>
        IEnumerable<IPropertyInfo> ITypeInfo.Properties => this.Properties;

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
        /// Gets a member by the provided name.
        /// </summary>
        /// <param name="name">The member name.</param>
        /// <param name="throwIfNotFound">True to throw if the requested member is not found.</param>
        /// <returns>
        /// The requested member, or <c>null</c>.
        /// </returns>
        IElementInfo ITypeInfo.GetMember(string name, bool throwIfNotFound)
        {
            return this.GetMember(name, throwIfNotFound);
        }

        /// <summary>
        /// Creates an instance with the provided arguments (if any).
        /// </summary>
        /// <param name="args">The arguments.</param>
        /// <returns>
        /// The new instance.
        /// </returns>
        public virtual object CreateInstance(IEnumerable<object> args = null)
        {
            throw new ActivationException(string.Format(Strings.ClassifierBase_CannotInstantiateAbstractTypeInfo_Exception, this, typeof(ITypeInfo), this));
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
        protected virtual Func<IClassifier, bool> ComputeIsAspectOf()
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
            var parts = ((IAggregatedElementInfo)this).Parts.OfType<ITypeInfo>().ToList();

            // compute base: types, classifier and mixins
            var baseTypes = parts.SelectMany(t => t.BaseTypes);
            this.BaseTypes = baseTypes.Select(t => this.ModelSpace.TryGetClassifier(t, findContext: constructionContext) ?? t).ToList();
            var classifierBaseTypes = this.BaseTypes.OfType<IClassifier>().ToList();
            this.BaseClassifier = classifierBaseTypes.SingleOrDefault(c => !c.IsMixin);
            this.BaseMixins = new ReadOnlyCollection<IClassifier>(classifierBaseTypes.Where(c => c.IsMixin).ToList());

            // compute generic arguments
            if (parts.Count > 1 && parts.Any(p => p.IsGenericType()))
            {
                throw new ModelConstructionException(string.Format(Strings.ClassifierBase_MultipleGenericPartsNotSupported_Exception, this.FullName, string.Join(", ", parts.Select(p => p.FullName))));
            }

            var genericPart = parts.Count == 1 ? parts[0] : null;
            if (genericPart != null && genericPart.IsGenericType())
            {
                this.GenericTypeParameters = genericPart.GenericTypeParameters;
                this.GenericTypeArguments = (genericPart as IClassifier)?.GenericTypeArguments
                                            ?? new ReadOnlyCollection<ITypeInfo>(
                                                   genericPart.GenericTypeArguments.Select(
                                                       t => this.ModelSpace.TryGetClassifier(t, findContext: constructionContext) ?? t).ToList());

                this.GenericTypeDefinition = genericPart.GenericTypeDefinition != null
                                                 ? this.ModelSpace.TryGetClassifier(genericPart.GenericTypeDefinition, findContext: constructionContext)
                                                 : null;
            }

            base.OnCompleteConstruction(constructionContext);
        }
    }
}