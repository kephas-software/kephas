// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RazorProjectEngineContextExtensions.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Templating.Razor;

using System.Diagnostics.CodeAnalysis;

using Microsoft.AspNetCore.Razor.Language;

/// <summary>
/// Provides extension methods for <see cref="ITemplateProcessingContext"/>.
/// </summary>
public static class RazorProjectEngineContextExtensions
{
    /// <summary>
    /// Gets the root namespace.
    /// </summary>
    /// <typeparam name="T">The context type.</typeparam>
    /// <param name="context">The context.</param>
    /// <returns>The root namespace.</returns>
    public static string? RootNamespace<T>(this T context)
        where T : ITemplateProcessingContext
    {
        return context?[nameof(RootNamespace)] as string;
    }

    /// <summary>
    /// Sets the root namespace.
    /// </summary>
    /// <typeparam name="T">The context type.</typeparam>
    /// <param name="context">The context.</param>
    /// <param name="rootNamespace">The root namespace.</param>
    /// <returns>The provided context.</returns>
    public static T? RootNamespace<T>([DisallowNull] this T context, string? rootNamespace)
        where T : ITemplateProcessingContext
    {
        context = context ?? throw new ArgumentNullException(nameof(context));
        context[nameof(RootNamespace)] = rootNamespace;
        return context;
    }

    /// <summary>
    /// Gets the type of the model.
    /// </summary>
    /// <typeparam name="T">The context type.</typeparam>
    /// <param name="context">The context.</param>
    /// <returns>The model type.</returns>
    public static Type? ModelType<T>(this T context)
        where T : ITemplateProcessingContext
    {
        return context?[nameof(ModelType)] as Type;
    }

    /// <summary>
    /// Sets the type of the model.
    /// </summary>
    /// <typeparam name="T">The context type.</typeparam>
    /// <param name="context">The context.</param>
    /// <param name="baseTypeName">Type of the model.</param>
    /// <returns>The provided context.</returns>
    public static T ModelType<T>([DisallowNull] this T context, Type? baseTypeName)
        where T : ITemplateProcessingContext
    {
        context = context ?? throw new ArgumentNullException(nameof(context));
        context[nameof(ModelType)] = baseTypeName;
        return context;
    }

    /// <summary>
    /// Gets the template page's base type name.
    /// </summary>
    /// <typeparam name="T">The context type.</typeparam>
    /// <param name="context">The context.</param>
    /// <returns>The base type name.</returns>
    public static string? BaseTypeName<T>(this T context)
        where T : ITemplateProcessingContext
    {
        return context?[nameof(BaseTypeName)] as string;
    }

    /// <summary>
    /// Sets the template page's base type name.
    /// </summary>
    /// <typeparam name="T">The context type.</typeparam>
    /// <param name="context">The context.</param>
    /// <param name="baseTypeName">The template page's base type name.</param>
    /// <returns>The provided context.</returns>
    public static T BaseTypeName<T>([DisallowNull] this T context, string? baseTypeName)
        where T : ITemplateProcessingContext
    {
        context = context ?? throw new ArgumentNullException(nameof(context));
        context[nameof(BaseTypeName)] = baseTypeName;
        return context;
    }

    /// <summary>
    /// Gets the action to configure the engine builder.
    /// </summary>
    /// <typeparam name="T">The context type.</typeparam>
    /// <param name="context">The context.</param>
    /// <returns>The engine configure action.</returns>
    public static Action<RazorProjectEngineBuilder>? ConfigureEngine<T>(this T context)
        where T : ITemplateProcessingContext
    {
        return context?[nameof(ConfigureEngine)] as Action<RazorProjectEngineBuilder>;
    }

    /// <summary>
    /// Sets the action to configure the engine builder.
    /// </summary>
    /// <typeparam name="T">The context type.</typeparam>
    /// <param name="context">The context.</param>
    /// <param name="configure">The configure action.</param>
    /// <returns>The provided context.</returns>
    public static T ConfigureEngine<T>(this T context, Action<RazorProjectEngineBuilder>? configure)
        where T : ITemplateProcessingContext
    {
        context = context ?? throw new ArgumentNullException(nameof(context));
        context[nameof(ConfigureEngine)] = configure;
        return context;
    }
}