// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ModelDimensionFactory.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Factory for model dimensions.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Model.Factory
{
    using System.Reflection;

    using Kephas.Model.AttributedModel;
    using Kephas.Model.Elements;

    /// <summary>
    /// Factory for model dimensions.
    /// </summary>
    public class ModelDimensionFactory : ElementFactoryBase<IModelDimension, ModelDimensionConstructorInfo>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ModelDimensionFactory"/> class.
        /// </summary>
        /// <param name="modelSpaceProvider">The model space provider.</param>
        public ModelDimensionFactory(IModelSpaceProvider modelSpaceProvider)
            : base(modelSpaceProvider)
        {
        }

        /// <summary>
        /// Tries to get the element constructor information.
        /// </summary>
        /// <param name="nativeElement">The native element.</param>
        /// <returns>
        /// The element constructor information, if available, otherwise <c>null</c>.
        /// </returns>
        protected override ModelDimensionConstructorInfo TryGetElementConstructorInfo(MemberInfo nativeElement)
        {
            var typeInfo = nativeElement as TypeInfo;
            if (typeInfo == null || !typeInfo.IsInterface)
            {
                return null;
            }

            var modelDimensionAttribute = typeInfo.GetCustomAttribute<ModelDimensionAttribute>();
            if (modelDimensionAttribute == null)
            {
                return null;
            }

            var constructorInfo = new ModelDimensionConstructorInfo(ModelSpaceProvider.GetModelSpace(), nativeElement)
                                    {
                                        ModelDimensionAttribute = modelDimensionAttribute,
                                    };

            return constructorInfo;
        }

        /// <summary>
        /// Creates the element based on the provided constructor information.
        /// </summary>
        /// <param name="elementConstructorInfo">The element constructor information.</param>
        /// <returns>The newly created element.</returns>
        protected override IModelDimension CreateElement(ModelDimensionConstructorInfo elementConstructorInfo)
        {
            var attr = elementConstructorInfo.ModelDimensionAttribute;
            return new ModelDimension(ModelSpaceProvider.GetModelSpace(), this.GetName(elementConstructorInfo), attr.Index, attr.IsAggregatable);
        }

        /// <summary>
        /// Gets the dimension name.
        /// </summary>
        /// <param name="elementConstructorInfo">The element constructor information.</param>
        /// <returns>
        /// The dimension name.
        /// </returns>
        /// <remarks>
        /// By default, the dimension name is computed from the type name where the Dimension suffix
        /// is trimmed away.
        /// </remarks>
        protected virtual string GetName(ModelDimensionConstructorInfo elementConstructorInfo)
        {
            const string Suffix = "Dimension";
            var dimName = elementConstructorInfo.NativeTypeInfo.Name;
            dimName = dimName.EndsWith(Suffix) 
                ? dimName.Substring(0, dimName.Length - Suffix.Length)
                : dimName;

            if (dimName.StartsWith("I"))
            {
                dimName = dimName.Substring(1);
            }

            return dimName;
        }
    }
}