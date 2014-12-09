using System.Reflection;
using Kephas.Model.Elements;

namespace Kephas.Model.Factory
{
    /// <summary>
    /// Factory for model dimensions.
    /// </summary>
    public class ModelDimensionFactory : ElementFactoryBase<ModelDimension, ModelDimensionConstructorInfo>
    {
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

            var constructorInfo = new ModelDimensionConstructorInfo(nativeElement)
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
        protected override ModelDimension CreateElement(ModelDimensionConstructorInfo elementConstructorInfo)
        {
            var attr = elementConstructorInfo.ModelDimensionAttribute;
            return new ModelDimension(this.GetName(elementConstructorInfo), attr.Index, attr.IsAggregatable);
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
            const string suffix = "Dimension";
            var dimName = elementConstructorInfo.NativeTypeInfo.Name;
            dimName = dimName.EndsWith(suffix) 
                ? dimName.Substring(0, dimName.Length - suffix.Length)
                : dimName;

            if (dimName.StartsWith("I"))
            {
                dimName = dimName.Substring(1);
            }

            return dimName;
        }
    }
}