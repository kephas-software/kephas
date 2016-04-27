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
    using System.Collections.Generic;
    using System.Linq;

    using Kephas.Model.Construction;
    using Kephas.Model.Elements.Annotations;

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
        /// Initializes a new instance of the <see cref="ClassifierBase{TModelContract}" /> class.
        /// </summary>
        /// <param name="constructionContext">Context for the construction.</param>
        /// <param name="name">The name.</param>
        protected ClassifierBase(IModelConstructionContext constructionContext, string name)
            : base(constructionContext, name)
        {
        }

        /// <summary>
        /// Gets the projection where the model element is defined.
        /// </summary>
        /// <value>
        /// The projection.
        /// </value>
        public IModelProjection Projection { get; }

        /// <summary>
        /// Gets the classifier properties.
        /// </summary>
        /// <value>
        /// The classifier properties.
        /// </value>
        public IEnumerable<IProperty> Properties => this.Members.OfType<IProperty>();

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
        /// Gets the namespace of the type.
        /// </summary>
        /// <value>
        /// The namespace of the type.
        /// </value>
        public string Namespace => this.Projection?.FullName;

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
    }
}