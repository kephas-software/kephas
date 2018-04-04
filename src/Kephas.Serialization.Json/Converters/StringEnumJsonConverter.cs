// --------------------------------------------------------------------------------------------------------------------
// <copyright file="StringEnumJsonConverter.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Composable <see cref="StringEnumConverter" />.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Serialization.Json.Converters
{
    using Newtonsoft.Json.Converters;

    /// <summary>
    /// Composable <see cref="StringEnumConverter"/>.
    /// </summary>
    public class StringEnumJsonConverter : StringEnumConverter, IJsonConverter
    {
    }
}