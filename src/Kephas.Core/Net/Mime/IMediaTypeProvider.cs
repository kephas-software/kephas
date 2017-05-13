// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IMediaTypeProvider.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Declares the IFormatProvider interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Net.Mime
{
    using System;

    using Kephas.Services;

    /// <summary>
    /// Shared application service contract providing the media type based on its name.
    /// </summary>
    [SharedAppServiceContract]
    public interface IMediaTypeProvider
    {
        /// <summary>
        /// Gets the media type based on the media type name.
        /// </summary>
        /// <param name="mediaTypeName">The media type name.</param>
        /// <param name="throwIfNotFound">True to throw if a format is not found (optional).</param>
        /// <returns>
        /// The media type or <c>null</c>.
        /// </returns>
        Type GetMediaType(string mediaTypeName, bool throwIfNotFound = true);
    }
}