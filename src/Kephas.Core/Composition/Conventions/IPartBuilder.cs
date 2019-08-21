// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IPartBuilder.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Declares the IPartBuilder interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Composition.Conventions
{
    /// <summary>
    /// Interface for part builder.
    /// </summary>
    public interface IPartBuilder
    {
        /// <summary>
        /// Mark the part as being shared within the entire composition.
        /// </summary>
        /// <returns>A part builder allowing further configuration of the part.</returns>
        IPartBuilder Singleton();

        /// <summary>
        /// Mark the part as being shared within the scope.
        /// </summary>
        /// <returns>
        /// A part builder allowing further configuration of the part.
        /// </returns>
        IPartBuilder Scoped();
    }
}