// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ISerializationService.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Contract for serialization services.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Serialization
{
    using Kephas.Composition;
    using Kephas.Services;

    /// <summary>
    /// Contract for serialization services.
    /// </summary>
    [SharedAppServiceContract]
    public interface ISerializationService : ICompositionContextAware
    {
        /// <summary>
        /// Gets a serializer for the provided context.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <returns>
        /// The serializer.
        /// </returns>
        ISerializer GetSerializer(ISerializationContext context = null);
    }
}