// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IJsonConverter.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
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
    [SharedAppServiceContract]
    public interface IJsonConverter
    {
    }
}