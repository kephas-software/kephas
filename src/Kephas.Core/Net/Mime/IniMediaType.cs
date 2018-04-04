// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IniMediaType.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
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
    public class IniMediaType : IMediaType
    {
    }
}