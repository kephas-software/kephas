// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IJsonConverter.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Marker interface for composable JSON converters.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Serialization.Json.Converters
{
    using Kephas.Services;

    /// <summary>
    /// Marker interface for composable JSON converters.
    /// </summary>
    [SingletonAppServiceContract(AllowMultiple = true)]
    public interface IJsonConverter
    {
    }
}