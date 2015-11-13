// --------------------------------------------------------------------------------------------------------------------
// <copyright file="StringEnumJsonConverter.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
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