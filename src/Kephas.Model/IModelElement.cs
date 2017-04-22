// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IModelElement.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Contract providing base information about a model element.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Model
{
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Diagnostics.Contracts;
    using System.Linq;

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
        public static IEnumerable<INamedElement> GetOwnMembers(this IModelElement modelElement)
        {
            Contract.Requires(modelElement != null);

            return modelElement.Members.Where(m => m.Container == modelElement);
        }
    }
}