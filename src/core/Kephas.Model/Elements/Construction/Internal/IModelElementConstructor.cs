// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IModelElementConstructor.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Constructor interface for model elements.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Model.Elements.Construction.Internal
{
    /// <summary>
    /// Constructor interface for model elements.
    /// </summary>
    internal interface IModelElementConstructor : INamedElementConstructor
    {
        /// <summary>
        /// Adds the member to the members list.
        /// </summary>
        /// <param name="member">The member.</param>
        void AddMember(INamedElement member);
    }
}