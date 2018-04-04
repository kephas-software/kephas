﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="JsonMediaType.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
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