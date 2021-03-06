﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="XmlMediaType.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Marker class for XML format.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Net.Mime
{
    /// <summary>
    /// Marker class for XML format.
    /// </summary>
    [SupportedMediaTypes(new[] { MediaTypeNames.Application.Xml, MediaTypeNames.Text.Xml })]
    [SupportedFileExtensions(new[] { "xml" })]
    public sealed class XmlMediaType : IMediaType
    {
    }
}