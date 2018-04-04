// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IJsonConverter.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
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
    [SharedAppServiceContract(AllowMultiple = true)]
    public interface IJsonConverter
    {
    }
}