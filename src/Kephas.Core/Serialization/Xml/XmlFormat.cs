// --------------------------------------------------------------------------------------------------------------------
// <copyright file="XmlFormat.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Marker class for XML format.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Serialization.Xml
{
    using Kephas.Net.Mime;

    /// <summary>
    /// Marker class for XML format.
    /// </summary>
    [SupportedMediaTypes(new[] { MediaTypeNames.Application.Xml, MediaTypeNames.Text.Xml })]
    public sealed class XmlFormat : IFormat
    {
    }
}