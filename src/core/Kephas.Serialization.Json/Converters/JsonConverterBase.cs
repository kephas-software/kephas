// --------------------------------------------------------------------------------------------------------------------
// <copyright file="JsonConverterBase.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Base for composable JSON converters.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Serialization.Json.Converters
{
    using Newtonsoft.Json;

    /// <summary>
    /// Base for composable JSON converters.
    /// </summary>
    public abstract class JsonConverterBase : JsonConverter, IJsonConverter
    {
    }
}