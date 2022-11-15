// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AppServiceInfoBuilderExtensions.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Services.Builder;

/// <summary>
/// Extension methods for <see cref="IAppServiceInfoBuilder"/>.
/// </summary>
public static class AppServiceInfoBuilderExtensions
{
    /// <summary>
    /// Sets the <see cref="AppServiceMetadata.ProcessingPriority"/> metadata.
    /// </summary>
    /// <param name="builder">The builder.</param>
    /// <param name="priority">The priority.</param>
    /// <returns>The provided builder.</returns>
    public static IAppServiceInfoBuilder ProcessingPriority(this IAppServiceInfoBuilder builder, Priority priority)
    {
        builder = builder ?? throw new ArgumentNullException(nameof(builder));
        builder.AddMetadata(nameof(AppServiceMetadata.ProcessingPriority), priority);

        return builder;
    }

    /// <summary>
    /// Sets the <see cref="AppServiceMetadata.OverridePriority"/> metadata.
    /// </summary>
    /// <param name="builder">The builder.</param>
    /// <param name="priority">The priority.</param>
    /// <returns>The provided builder.</returns>
    public static IAppServiceInfoBuilder OverridePriority(this IAppServiceInfoBuilder builder, Priority priority)
    {
        builder = builder ?? throw new ArgumentNullException(nameof(builder));
        builder.AddMetadata(nameof(AppServiceMetadata.OverridePriority), priority);

        return builder;
    }

    /// <summary>
    /// Sets the <see cref="AppServiceMetadata.IsOverride"/> metadata to <c>true</c>.
    /// </summary>
    /// <param name="builder">The builder.</param>
    /// <returns>The provided builder.</returns>
    public static IAppServiceInfoBuilder IsOverride(this IAppServiceInfoBuilder builder)
    {
        builder = builder ?? throw new ArgumentNullException(nameof(builder));
        builder.AddMetadata(nameof(AppServiceMetadata.IsOverride), true);

        return builder;
    }
}