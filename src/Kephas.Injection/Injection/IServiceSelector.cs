// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IServiceSelector.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Injection;

using Kephas.Services;

[AppServiceContract(AsOpenGeneric = true)]
public interface IServiceSelector<out TService, out TMetadata>
    where TService : class
{
    TService? TryGetService(Func<TMetadata, bool> selector);

    TService GetService(Func<TMetadata, bool> selector)
    {
        var service = this.TryGetService(selector);
        return service ?? throw new ArgumentException(DependencyInjectionStrings.ServiceSelectorServiceNotFound, nameof(selector));
    }
}
