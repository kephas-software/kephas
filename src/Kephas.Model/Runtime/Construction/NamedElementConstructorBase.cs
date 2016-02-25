// --------------------------------------------------------------------------------------------------------------------
// <copyright file="NamedElementConstructorBase.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Base implementation of an element information provider based on the .NET runtime.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Model.Runtime.Construction
{
    using System.Text;

    using Kephas.Dynamic;
    using Kephas.Model.Elements;
    using Kephas.Model.Factory;
    using Kephas.Model.Runtime.Construction.Internal;
    using Kephas.Reflection;

    /// <summary>
    /// Base implementation of a named element information provider based on the .NET runtime.
    /// </summary>
    /// <typeparam name="TModel">Type of the model.</typeparam>
    /// <typeparam name="TModelContract">Type of the model contract.</typeparam>
    /// <typeparam name="TRuntime">Type of the runtime definition.</typeparam>
    public abstract class NamedElementConstructorBase<TModel, TModelContract, TRuntime> : IRuntimeModelElementConstructor<TModel, TModelContract, TRuntime>
        where TModel : NamedElementBase<TModelContract>
        where TModelContract : class, INamedElement
        where TRuntime : class
    {
        /// <summary>
        /// Gets the element name discriminator.
        /// </summary>
        /// <value>
        /// The element name discriminator.
        /// </value>
        /// <remarks>
        /// This dicriminator can be used as a suffix in the name to identify the element type.
        /// </remarks>
        protected virtual string ElementNameDiscriminator => null;

        /// <summary>
        /// Tries to create an element information structure based on the provided runtime element
        /// information.
        /// </summary>
        /// <param name="constructionContext">Context for the construction.</param>
        /// <param name="runtimeElement">The runtime element.</param>
        /// <returns>
        /// A new element information based on the provided runtime element information, or <c>null</c>
        /// if the runtime element information is not supported.
        /// </returns>
        public virtual INamedElement TryCreateModelElement(IModelConstructionContext constructionContext, object runtimeElement)
        {
            var typedRuntimeInfo = runtimeElement as TRuntime;
            if (typedRuntimeInfo == null)
            {
                return null;
            }

            var elementInfo = this.TryCreateModelElementCore(constructionContext, typedRuntimeInfo);
            ((INamedElementConstructor)elementInfo)?.AddPart(typedRuntimeInfo);
            return elementInfo;
        }

        /// <summary>
        /// Tries to compute the name for the provided runtime element.
        /// </summary>
        /// <param name="constructionContext">Context for the construction.</param>
        /// <param name="runtimeElement">The runtime element.</param>
        /// <returns>
        /// A string containing the name, or <c>null</c> if the name could not be computed.
        /// </returns>
        public string TryComputeName(IModelConstructionContext constructionContext, object runtimeElement)
        {
            return this.TryComputeNameCore(runtimeElement);
        }

        /// <summary>
        /// Core implementation of trying to get the element information.
        /// </summary>
        /// <param name="constructionContext">Context for the construction.</param>
        /// <param name="runtimeElement">The runtime element.</param>
        /// <returns>
        /// A new element information based on the provided runtime element information, or <c>null</c>
        /// if the runtime element information is not supported.
        /// </returns>
        protected abstract TModel TryCreateModelElementCore(IModelConstructionContext constructionContext, TRuntime runtimeElement);

        /// <summary>
        /// Computes the model element name based on the runtime element.
        /// </summary>
        /// <param name="runtimeElement">The runtime element.</param>
        /// <returns>The element name, or <c>null</c> if the name could not be computed.</returns>
        protected virtual string TryComputeNameCore(object runtimeElement)
        {
            var memberInfo = runtimeElement as IElementInfo;
            if (memberInfo == null)
            {
                return null;
            }

            var nameBuilder = new StringBuilder(memberInfo.Name);

            var typeInfo = runtimeElement as IDynamicTypeInfo;
            var isInterface = typeInfo?.TypeInfo.IsInterface;
            if (isInterface.HasValue && isInterface.Value && nameBuilder[0] == 'I')
            {
                nameBuilder.Remove(0, 1);
            }

            var discriminator = this.ElementNameDiscriminator;
            if (!string.IsNullOrEmpty(discriminator))
            {
                if (memberInfo.Name.EndsWith(discriminator))
                {
                    nameBuilder.Remove(nameBuilder.Length - discriminator.Length, discriminator.Length);
                }
            }

            return nameBuilder.ToString();
        }
    }
}