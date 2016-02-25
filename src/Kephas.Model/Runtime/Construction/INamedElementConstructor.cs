// --------------------------------------------------------------------------------------------------------------------
// <copyright file="INamedElementConstructor.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Declares the INamedElementConstructor interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Model.Runtime.Construction
{
    /// <summary>
    /// Interface for named element constructor.
    /// </summary>
    internal interface INamedElementConstructor
    {
        /// <summary>
        /// Sets the element container.
        /// </summary>
        /// <param name="container">The element container.</param>
        void SetContainer(IModelElement container);

        /// <summary>
        /// Sets the full name.
        /// </summary>
        /// <param name="fullName">The full name.</param>
        void SetFullName(string fullName);

        /// <summary>
        /// Completes the construction of the element.
        /// </summary>
        void CompleteConstruction();

        /// <summary>
        /// Adds the member to the members list.
        /// </summary>
        /// <param name="member">The member.</param>
        void AddMember(INamedElement member);

        /// <summary>
        /// Adds a part to the aggregated element.
        /// </summary>
        /// <param name="part">The part to be added.</param>
        void AddPart(object part);
    }
}