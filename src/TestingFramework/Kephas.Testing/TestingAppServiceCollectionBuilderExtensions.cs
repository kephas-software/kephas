// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TestingAppServiceCollectionBuilderExtensions.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Testing;

using Kephas.Injection.Builder;
using Kephas.Testing.Injection;

/// <summary>
/// Extension methods for <see cref="IAppServiceCollectionBuilder"/> in testing.
/// </summary>
public static class TestingAppServiceCollectionBuilderExtensions
{
    /// <summary>
    /// Add the types to the builder.
    /// </summary>
    /// <param name="builder">The <see cref="IAppServiceCollectionBuilder"/> instance.</param>
    /// <param name="types">The types to add.</param>
    /// <returns>The provided <see cref="IAppServiceCollectionBuilder"/> instance.</returns>
    public static IAppServiceCollectionBuilder WithParts(this IAppServiceCollectionBuilder builder, IEnumerable<Type> types)
    {
        builder = builder ?? throw new ArgumentNullException(nameof(builder));

        builder.AppServiceInfosProviders.Add(new PartsAppServiceInfosProvider(types));

        return builder;
    }

    /// <summary>
    /// Add the types to the builder.
    /// </summary>
    /// <param name="builder">The <see cref="IAppServiceCollectionBuilder"/> instance.</param>
    /// <param name="types">The types to add.</param>
    /// <returns>The provided <see cref="IAppServiceCollectionBuilder"/> instance.</returns>
    public static IAppServiceCollectionBuilder WithParts(this IAppServiceCollectionBuilder builder, params Type[] types) =>
        WithParts(builder, (IEnumerable<Type>)types);
}