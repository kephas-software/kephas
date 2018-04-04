// --------------------------------------------------------------------------------------------------------------------
// <copyright file="XmlMediaType.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
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
    public sealed class XmlMediaType : IMediaType
    {
    }
}