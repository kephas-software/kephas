// --------------------------------------------------------------------------------------------------------------------
// <copyright file="JsonMediaType.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Marker class for JSON format.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Net.Mime
{
    /// <summary>
    /// Marker class for JSON format.
    /// </summary>
    [SupportedMediaTypes(new[] { MediaTypeNames.Application.Json })]
    public sealed class JsonMediaType : IMediaType
    {
    }
}