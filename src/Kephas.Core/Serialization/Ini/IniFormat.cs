// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IniFormat.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Implements the initialize format class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Serialization.Ini
{
    using Kephas.Net.Mime;

    /// <summary>
    /// Marker class for the INI format.
    /// </summary>
    [SupportedMediaTypes(new[] { MediaTypeNames.Text.Ini })]
    public class IniFormat : IFormat
    {
    }
}