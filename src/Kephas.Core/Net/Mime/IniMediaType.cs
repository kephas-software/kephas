// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IniMediaType.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
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