// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ModelRefPropertiesProvider.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the model reference properties provider class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Data.Model.Analysis
{
    using System.Collections.Generic;
    using System.Linq;

    using Kephas.Data.Analysis;
    using Kephas.Model;
    using Kephas.Reflection;
    using Kephas.Runtime;
    using Kephas.Services;

    /// <summary>
    /// A reference properties provider which uses the model space.
    /// </summary>
    [OverridePriority(Priority.BelowNormal)]
    public class ModelRefPropertiesProvider : DefaultRefPropertiesProvider
    {
        /// <summary>
        /// The model space.
        /// </summary>
        private readonly IModelSpace modelSpace;

        /// <summary>
        /// Initializes a new instance of the <see cref="ModelRefPropertiesProvider"/> class.
        /// </summary>
        /// <param name="modelSpaceProvider">The model space provider.</param>
        public ModelRefPropertiesProvider(IModelSpaceProvider modelSpaceProvider)
        {
            this.modelSpace = modelSpaceProvider.GetModelSpace();
        }

        /// <summary>
        /// Enumerates compute reference properties in this collection.
        /// </summary>
        /// <param name="typeInfo">Information describing the type.</param>
        /// <returns>
        /// An enumerator that allows foreach to be used to process compute reference properties in this
        /// collection.
        /// </returns>
        protected override IEnumerable<IPropertyInfo> ComputeRefProperties(ITypeInfo typeInfo)
        {
            var classifier = this.modelSpace.TryGetClassifier(typeInfo);
            if (classifier != null)
            {
                return classifier.Properties.Where(this.IsRefProperty).ToList();
            }

            return base.ComputeRefProperties(typeInfo);
        }

        /// <summary>
        /// Query if 'propertyInfo' is reference property.
        /// </summary>
        /// <param name="propertyInfo">Information describing the property.</param>
        /// <returns>
        /// True if reference property, false if not.
        /// </returns>
        protected override bool IsRefProperty(IPropertyInfo propertyInfo)
        {
            if (propertyInfo.ValueType is IClassifier classifierPropertyType)
            {
                return classifierPropertyType
                    .Parts.OfType<IRuntimeTypeInfo>()
                    .Any(t => typeof(IRef).IsAssignableFrom(t.TypeInfo));
            }

            return base.IsRefProperty(propertyInfo);
        }
    }
}