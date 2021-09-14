// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SettingsTypeConstructor.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Model.Runtime.Construction
{
    using Kephas.Configuration.Reflection;
    using Kephas.Model.Construction;
    using Kephas.Model.Elements;
    using Kephas.Runtime;

    /// <summary>
    /// A constructor for <see cref="SettingsType"/>.
    /// </summary>
    public class SettingsTypeConstructor : ClassifierConstructorBase<SettingsType, ISettingsType>
    {
        /// <summary>
        /// Determines whether a model element can be created for the provided runtime element.
        /// </summary>
        /// <param name="constructionContext">Context for the construction.</param>
        /// <param name="runtimeElement">The runtime element.</param>
        /// <returns>
        /// <c>true</c> if a model element can be created, <c>false</c> if not.
        /// </returns>
        protected override bool CanCreateModelElement(IModelConstructionContext constructionContext, IRuntimeTypeInfo runtimeElement)
        {
            return runtimeElement is ISettingsInfo;
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
        protected override SettingsType? TryCreateModelElementCore(IModelConstructionContext constructionContext, IRuntimeTypeInfo runtimeElement)
        {
            return new SettingsType(constructionContext, (ISettingsInfo)runtimeElement, this.TryComputeName(runtimeElement, constructionContext)!);
        }

        /// <summary>
        /// Computes the model element name based on the runtime element. The "Settings" ending is stripped away.
        /// </summary>
        /// <param name="runtimeElement">The runtime element.</param>
        /// <param name="constructionContext">The construction context.</param>
        /// <returns>The element name, or <c>null</c> if the name could not be computed.</returns>
        protected override string? TryComputeNameCore(object runtimeElement, IModelConstructionContext constructionContext)
        {
            const string ending = "Settings";
            var runtimeType = (ISettingsInfo)runtimeElement;
            var name = runtimeType.Name;
            if (name.EndsWith(ending) && name != ending)
            {
                return name[ending.Length..];
            }

            return name;
        }
    }
}