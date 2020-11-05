// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PermissionTypeConstructor.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the permission type constructor class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Model.Security.Authorization.Runtime.Construction
{
    using Kephas.Model.Construction;
    using Kephas.Model.Runtime.Construction;
    using Kephas.Model.Security.Authorization.Elements;
    using Kephas.Runtime;
    using Kephas.Security.Authorization.Reflection;

    /// <summary>
    /// Classifier constructor for <see cref="PermissionType"/>.
    /// </summary>
    public class PermissionTypeConstructor : ClassifierConstructorBase<PermissionType, IPermissionType>
    {
        /// <summary>
        /// The permission discriminator.
        /// </summary>
        public static readonly string PermissionDiscriminator = "Permission";

        /// <summary>
        /// Gets the element name discriminator.
        /// </summary>
        /// <value>
        /// The element name discriminator.
        /// </value>
        /// <remarks>
        /// This discriminator can be used as a suffix in the name to identify the element type.
        /// </remarks>
        protected override string ElementNameDiscriminator => PermissionDiscriminator;

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
            return runtimeElement is IPermissionInfo;
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
        protected override PermissionType TryCreateModelElementCore(IModelConstructionContext constructionContext, IRuntimeTypeInfo runtimeElement)
        {
            return new PermissionType(constructionContext, this.TryComputeName(runtimeElement, constructionContext));
        }
    }
}