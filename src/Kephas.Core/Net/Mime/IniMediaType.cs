// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IniMediaType.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the initialize format class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Net.Mime
{
    /// <summary>
    /// Marker class for the INI format.
    /// </summary>
    [SupportedMediaTypes(new[] { MediaTypeNames.Text.Ini })]
    [SupportedFileExtensions(new[] { "ini" })]
    public class IniMediaType : IMediaType
    {
    }
}