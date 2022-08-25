// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LazyService.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Extensions.DependencyInjection;

using Kephas.Services;

public class LazyService<T, TImplementation> : Lazy<T>
    where TImplementation : class, T
{
    public LazyService(IServiceProvider serviceProvider)
        : base(() => serviceProvider.GetRequiredService<TImplementation>())
    {
    }
}

public class LazyService<T, TImplementation, TMetadata> : Lazy<T, TMetadata>
    where TImplementation : class, T
{
    public LazyService(IServiceProvider serviceProvider)
        : base(
            () => serviceProvider.GetRequiredService<TImplementation>(),
            ServiceHelper.GetServiceMetadata<TMetadata>(typeof(TImplementation), typeof(T)))
    {
    }
}