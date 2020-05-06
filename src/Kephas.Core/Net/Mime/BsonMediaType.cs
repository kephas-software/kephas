// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BsonMediaType.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Net.Mime
{
    /// <summary>
    /// Marker class for BSON format.
    /// </summary>
    [SupportedMediaTypes(new[] { MediaTypeNames.Application.Bson })]
    [SupportedFileExtensions(new[] { "bson" })]
    public sealed class BsonMediaType : IMediaType
    {
    }
}