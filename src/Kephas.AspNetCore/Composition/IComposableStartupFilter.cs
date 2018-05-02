﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IComposableStartupFilter.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Declares the IComposableStartupFilter interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.AspNetCore.Composition
{
    using Kephas.Services;

    using Microsoft.AspNetCore.Hosting;

    /// <summary>
    /// Interface for composable startup filter.
    /// </summary>
    [SharedAppServiceContract]
    public interface IComposableStartupFilter : IStartupFilter
    {
    }
}