// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IFormatProvider.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Declares the IFormatProvider interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Serialization
{
    using System;

    using Kephas.Services;

    /// <summary>
    /// Interface for format provider.
    /// </summary>
    [SharedAppServiceContract]
    public interface IFormatProvider
    {
        /// <summary>
        /// Gets the format type based on the MIME type.
        /// </summary>
        /// <param name="mediaType">The media type.</param>
        /// <param name="throwIfNotFound">True to throw if a format is not found (optional).</param>
        /// <returns>
        /// The format type.
        /// </returns>
        Type GetFormatType(string mediaType, bool throwIfNotFound = true);
    }
}