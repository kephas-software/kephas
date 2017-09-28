// --------------------------------------------------------------------------------------------------------------------
// <copyright file="NullAppConfiguration.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   A configuration returning no configuration values.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Application.Configuration
{
    using Kephas.Dynamic;
    using Kephas.Services;

    /// <summary>
    /// A configuration returning no configuration values.
    /// </summary>
    [OverridePriority(Priority.Lowest)]
    public class NullAppConfiguration : Expando, IAppConfiguration
    {
    }
}