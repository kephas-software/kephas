// --------------------------------------------------------------------------------------------------------------------
// <copyright file="JsonFormat.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Marker class for JSON format.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Serialization.Json
{
    using Kephas.Net.Mime;

    /// <summary>
    /// Marker class for JSON format.
    /// </summary>
    [SupportedMediaTypes(new[] { MediaTypeNames.Application.Json })]
    public sealed class JsonFormat : IFormat
    {
    }
}