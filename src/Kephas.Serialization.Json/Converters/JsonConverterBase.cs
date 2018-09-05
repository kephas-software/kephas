// --------------------------------------------------------------------------------------------------------------------
// <copyright file="JsonConverterBase.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
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