// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ModelDimensionConstructorInfo.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Structure providing information about constructing model dimensions.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Model.Factory
{
    using System.Reflection;

    using Kephas.Model.AttributedModel;

    /// <summary>
    /// Structure providing information about constructing model dimensions.
    /// </summary>
    public class ModelDimensionConstructorInfo : ElementConstructorInfo
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ModelDimensionConstructorInfo" /> class.
        /// </summary>
        /// <param name="modelSpace">The model space.</param>
        /// <param name="nativeElement">The native element.</param>
        public ModelDimensionConstructorInfo(IModelSpace modelSpace, MemberInfo nativeElement)
            : base(modelSpace, nativeElement)
        {
        }

        /// <summary>
        /// Gets or sets the model dimension attribute.
        /// </summary>
        /// <value>
        /// The model dimension attribute.
        /// </value>
        public ModelDimensionAttribute ModelDimensionAttribute { get; set; }
    }
}