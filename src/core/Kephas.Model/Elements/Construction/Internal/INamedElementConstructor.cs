// --------------------------------------------------------------------------------------------------------------------
// <copyright file="INamedElementConstructor.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Constructor interface for named elements.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Model.Elements.Construction.Internal
{
    /// <summary>
    /// Constructor interface for named elements.
    /// </summary>
    internal interface INamedElementConstructor
    {
        /// <summary>
        /// Sets the element container.
        /// </summary>
        /// <param name="container">The element container.</param>
        void SetContainer(IModelElement container);

        /// <summary>
        /// Sets the full qualified name.
        /// </summary>
        /// <param name="fullQualifiedName">The full qualified name.</param>
        void SetFullQualifiedName(string fullQualifiedName);

        /// <summary>
        /// Completes the construction of the element.
        /// </summary>
        void CompleteConstruction();
    }
}