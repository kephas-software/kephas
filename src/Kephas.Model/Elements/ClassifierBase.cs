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

    using Kephas.Reflection;

    /// <summary>
    /// Base abstract class for classifiers.
    /// </summary>
    /// <typeparam name="TModelContract">The type of the model contract.</typeparam>
    /// <typeparam name="TClassifierInfo">The type of the classifier information.</typeparam>
    public abstract class ClassifierBase<TModelContract, TClassifierInfo> : ModelElementBase<TModelContract, TClassifierInfo>, IClassifier
        where TClassifierInfo : class, ITypeInfo
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ClassifierBase{TModelContract, TClassifierInfo}" /> class.
        /// </summary>
        /// <param name="elementInfo">Information describing the element.</param>
        /// <param name="modelSpace">The model space.</param>
        protected ClassifierBase(TClassifierInfo elementInfo, IModelSpace modelSpace)
            : base(elementInfo, modelSpace)
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
        public IEnumerable<IProperty> Properties { get; }
    }
}