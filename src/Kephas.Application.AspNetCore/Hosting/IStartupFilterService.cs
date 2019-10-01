// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IComposableStartupFilter.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Declares the IComposableStartupFilter interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Application.AspNetCore.Hosting
{
    using Kephas.Services;

    using Microsoft.AspNetCore.Hosting;

    /// <summary>
    /// Singleton application service contract for startup filters.
    /// </summary>
    [SingletonAppServiceContract(AllowMultiple = true, ContractType = typeof(IStartupFilter))]
    public interface IStartupFilterService : IStartupFilter
    {
    }
}