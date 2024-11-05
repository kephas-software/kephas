// --------------------------------------------------------------------------------------------------------------------
// <copyright file="JsonEnumConverter.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System.Text.Json.Serialization;

namespace Kephas.Web.Serialization.Json;

/// <summary>
/// The JSON enum converter allowing both strings and integer values.
/// </summary>
public class JsonEnumConverter() : JsonStringEnumConverter(allowIntegerValues: true)
{
}