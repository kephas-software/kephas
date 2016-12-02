// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ISerializationService.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Contract for serialization services.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Serialization
{
    using Kephas.Services;

    /// <summary>
    /// Contract for serialization services.
    /// </summary>
    [SharedAppServiceContract]
    public interface ISerializationService : IAmbientServicesAware
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