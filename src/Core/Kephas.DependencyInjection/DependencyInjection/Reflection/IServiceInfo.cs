// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IAppServiceInfo.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Declares the IAppServiceInfo interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System.Diagnostics.CodeAnalysis;
using System.Text;
using Kephas.DependencyInjection.Metadata;

namespace Kephas.DependencyInjection.Reflection;

/// <summary>
/// Contract interface providing information about an application service.
/// </summary>
public interface IServiceInfo : IFormattable, IHasServiceType
{
    /// <summary>
    /// Gets the application service lifetime.
    /// </summary>
    /// <value>
    /// The application service lifetime.
    /// </value>
    AppServiceLifetime Lifetime { get; }

    /// <summary>
    /// Gets a value indicating whether the contract should be exported as an open generic.
    /// </summary>
    /// <value>
    ///   <c>true</c> if the contract should be exported as an open generic; otherwise, <c>false</c>.
    /// </value>
    bool AsOpenGeneric { get; }

    /// <summary>
    /// Gets the supported metadata.
    /// </summary>
    /// <value>
    /// The metadata.
    /// </value>
    IDictionary<string, object?>? Metadata => null;

    /// <summary>
    /// Gets the type declaring the service type.
    /// </summary>
    /// <remarks>
    /// This is the type annotated with one of the: [<see cref="TransientServiceAttribute"/>], [<see cref="ScopedServiceAttribute"/>],
    /// or [<see cref="SingletonServiceAttribute"/>], and which declares the actual service type.
    /// Typically, this is a generic type used for collecting metadata, redirecting to the non-generic service type.
    /// </remarks>
    Type? ServiceDeclarationType
    {
        [return: DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)]
        get => ServiceType;
    }

    /// <summary>
    /// Gets or sets the supported metadata type.
    /// </summary>
    /// <value>
    /// The supported metadata type.
    /// </value>
    Type? MetadataType
    {
        [return: DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)]
        get;
        set;
    }

    /// <summary>
    /// Gets the instancing strategy: factory, implementation type, or instance.
    /// </summary>
    object? InstancingStrategy { get; }

    /// <summary>
    /// Gets the service instance.
    /// </summary>
    /// <value>
    /// The service instance.
    /// </value>
    object? Instance => ImplementationType != null || Factory != null ? null : InstancingStrategy;

    /// <summary>
    /// Gets the type of the service instance.
    /// </summary>
    /// <value>
    /// The type of the service instance.
    /// </value>
    Type? ImplementationType
    {
        [return: DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)]
        get => InstancingStrategy as Type;
    }

    /// <summary>
    /// Gets the service instance factory.
    /// </summary>
    /// <value>
    /// The service instance factory.
    /// </value>
    Func<IServiceProvider, object>? Factory => InstancingStrategy as Func<IServiceProvider, object>;

    /// <summary>
    /// Adds the metadata with the provided name and value.
    /// </summary>
    /// <param name="name">The metadata name.</param>
    /// <param name="value">The metadata value.</param>
    /// <returns>This <see cref="IServiceInfo"/>.</returns>
    public IServiceInfo AddMetadata(string name, object? value)
    {
        if (Metadata == null)
        {
            throw new InvalidOperationException("Cannot add metadata as long as the instance is not set.");
        }

        Metadata[name] = value;

        return this;
    }

    /// <summary>
    /// Adds the metadata with the provided name and value.
    /// </summary>
    /// <param name="metadata">The metadata to add.</param>
    /// <returns>This <see cref="IServiceInfo"/>.</returns>
    public IServiceInfo AddMetadata(IDictionary<string, object?>? metadata)
    {
        if (metadata == null)
        {
            return this;
        }

        foreach (var (name, value) in metadata)
        {
            AddMetadata(name, value);
        }

        return this;
    }

    /// <summary>Formats the value of the current instance using the specified format.</summary>
    /// <param name="format">The format to use.
    /// -or-
    /// A null reference (<see langword="Nothing" /> in Visual Basic) to use the default format defined for the type of the <see cref="T:System.IFormattable" /> implementation.</param>
    /// <param name="formatProvider">The provider to use to format the value.
    /// -or-
    /// A null reference (<see langword="Nothing" /> in Visual Basic) to obtain the numeric format information from the current locale setting of the operating system.</param>
    /// <returns>The value of the current instance in the specified format.</returns>
    string IFormattable.ToString(string? format, IFormatProvider? formatProvider)
    {
        if (string.IsNullOrEmpty(format))
        {
            return ToString() ?? string.Empty;
        }

        if (string.Equals("json", format, StringComparison.OrdinalIgnoreCase))
        {
            return ToJsonString();
        }
        
        throw new FormatException($"Invalid format string: '{format}'.");
    }

    private string ToJsonString()
    {
        var sb = new StringBuilder();
        sb.Append($"{{ lifetime: '{Lifetime}'");

        if (AsOpenGeneric)
        {
            sb.Append(", asOpenGeneric: true");
        }

        if (ImplementationType != null)
        {
            sb.Append($", implementationType: '{ImplementationType}'");
        }

        if (Instance != null)
        {
            sb.Append($", instance: '{Instance}'");
        }

        if (Factory != null)
        {
            sb.Append(", factory: '(function)'");
        }

        if (MetadataType != null)
        {
            sb.Append($", metadataType: '{MetadataType}'");
        }

        if (Metadata is { Count: > 0 })
        {
            sb.Append(", metadata: { ");

            foreach (var (key, value) in Metadata)
            {
                sb.Append($"'{key}': '{value}', ");
            }

            sb.Length -= 2;
            sb.Append(" }");
        }

        sb.Append(" }");

        return sb.ToString();
    }
}
