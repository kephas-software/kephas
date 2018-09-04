// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IModelElement.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Contract providing base information about a model element.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Model
{
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;

    using Kephas.Diagnostics.Contracts;

    /// <summary>
    /// Contract providing base information about a model element.
    /// </summary>
    public interface IModelElement : INamedElement
    {
        /// <summary>
        /// Gets the members of this model element.
        /// </summary>
        /// <value>
        /// The model element members.
        /// </value>
        IEnumerable<INamedElement> Members { get; }

        /// <summary>
        /// Gets the member with the specified qualified name.
        /// </summary>
        /// <param name="qualifiedName">The qualified name of the member.</param>
        /// <param name="throwOnNotFound">If set to <c>true</c> and the member is not found, an exception occurs; otherwise <c>null</c> is returned if the member is not found.</param>
        /// <returns>The member with the provided qualified name or <c>null</c>.</returns>
        INamedElement GetMember(string qualifiedName, bool throwOnNotFound = true);
    }

    /// <summary>
    /// Extensions for <see cref="IModelElement"/>.
    /// </summary>
    [SuppressMessage("StyleCop.CSharp.OrderingRules", "SA1202:ElementsMustBeOrderedByAccess", Justification = "Reviewed. Suppression is OK here.")]
    [SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1402:FileMayOnlyContainASingleClass", Justification = "Reviewed. Suppression is OK here.")]
    public static class ModelElementExtensions
    {
        /// <summary>
        /// Gets the model element's own members, excluding those declared by the base element or mixins.
        /// </summary>
        /// <param name="modelElement">The model element.</param>
        /// <returns>The members declared exclusively at the element level.</returns>
        public static IEnumerable<INamedElement> GetDeclaredMembers(this IModelElement modelElement)
        {
            Requires.NotNull(modelElement, nameof(modelElement));

            return modelElement.Members.Where(m => m.DeclaringContainer == modelElement);
        }
    }
}