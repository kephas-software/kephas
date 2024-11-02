// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AppServiceMetadata.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Metadata for application services.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System.Collections;
using System.Diagnostics.CodeAnalysis;

namespace Kephas.DependencyInjection.Metadata;

/// <summary>
/// Metadata for application services.
/// </summary>
public record ServiceMetadata : IReadOnlyDictionary<string, object?>, IHasPriority, IHasImplementationType, IHasServiceKey, IHasServiceType
{
    private readonly IDictionary<string, object?> metadata;

    /// <summary>
    /// Initializes a new instance of the <see cref="ServiceMetadata"/> class.
    /// </summary>
    /// <param name="metadata">The metadata.</param>
    public ServiceMetadata(IDictionary<string, object?>? metadata)
    {
        this.metadata = metadata ?? new Dictionary<string, object?>();
        if (metadata == null)
        {
            return;
        }

        Priority = (Priority)Convert.ChangeType(TryGetValue(metadata, nameof(Priority), Priority.Normal)!, typeof(int));
        ServiceKey = (string?)TryGetValue(metadata, nameof(ServiceKey));
        OverrideBaseImplementation = (bool)TryGetValue(metadata, nameof(OverrideBaseImplementation), false)!;
        ImplementationType = (Type?)TryGetValue(metadata, nameof(ImplementationType));
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ServiceMetadata" /> class.
    /// </summary>
    /// <param name="priority">Optional. The processing priority.</param>
    /// <param name="serviceKey">Optional. The service key.</param>
    /// <param name="overrideBaseImplementation">Optional. Indicates whether the service overrides its base.</param>
    public ServiceMetadata(Priority priority = 0, string? serviceKey = null, bool overrideBaseImplementation = false)
        : this((IDictionary<string, object?>?)null)
    {
        Priority = priority;
        ServiceKey = serviceKey;
        OverrideBaseImplementation = overrideBaseImplementation;
    }

    /// <summary>
    /// Gets or sets the type used to register and retrieve the service. Typically, this is an interface.
    /// </summary>
    public Type? ServiceType { get; set; }

    /// <summary>
    /// Gets or sets the key of the instance implementing the <see cref="IHasServiceType.ServiceType"/>.
    /// </summary>
    /// <remarks>
    /// Typically, this is the name of the service implementation type from which the service type name has been stripped away.
    /// </remarks>
    public string? ServiceKey { get; set; }

    /// <summary>
    /// Gets the order in which the services should be processed.
    /// </summary>
    /// <value>
    /// The processing priority.
    /// </value>
    public Priority Priority { get; }

    /// <summary>
    /// Gets a value indicating whether the service overrides the
    /// service it specializes.
    /// </summary>
    /// <value>
    /// True if the service overrides the service it specializes, false otherwise.
    /// </value>
    public bool OverrideBaseImplementation { get; }

    /// <summary>
    /// Gets or sets the concrete service type implementing the service contract.
    /// </summary>
    /// <value>
    /// The type of the service.
    /// </value>
    public Type? ImplementationType { get; set; }

    /// <summary>
    /// Gets or sets the service dependencies.
    /// </summary>
    /// <value>
    /// The service dependencies.
    /// </value>
    public IEnumerable<Type> Dependencies { get; set; } = [];

    /// <summary>Returns an enumerator that iterates through the collection.</summary>
    /// <returns>An enumerator that can be used to iterate through the collection.</returns>
    IEnumerator<KeyValuePair<string, object?>> IEnumerable<KeyValuePair<string, object?>>.GetEnumerator()
        => metadata.GetEnumerator();

    /// <summary>Returns an enumerator that iterates through a collection.</summary>
    /// <returns>An <see cref="T:System.Collections.IEnumerator" /> object that can be used to iterate through the collection.</returns>
    IEnumerator IEnumerable.GetEnumerator() => metadata.GetEnumerator();

    /// <summary>Gets the number of elements in the collection.</summary>
    /// <returns>The number of elements in the collection.</returns>
    int IReadOnlyCollection<KeyValuePair<string, object?>>.Count => metadata.Count;

    /// <summary>Determines whether the read-only dictionary contains an element that has the specified key.</summary>
    /// <param name="key">The key to locate.</param>
    /// <exception cref="T:System.ArgumentNullException">
    /// <paramref name="key" /> is <see langword="null" />.</exception>
    /// <returns>
    /// <see langword="true" /> if the read-only dictionary contains an element that has the specified key; otherwise, <see langword="false" />.</returns>
    bool IReadOnlyDictionary<string, object?>.ContainsKey(string key) => metadata.ContainsKey(key);

    /// <summary>Gets the value that is associated with the specified key.</summary>
    /// <param name="key">The key to locate.</param>
    /// <param name="value">When this method returns, the value associated with the specified key, if the key is found; otherwise, the default value for the type of the <paramref name="value" /> parameter. This parameter is passed uninitialized.</param>
    /// <exception cref="T:System.ArgumentNullException">
    /// <paramref name="key" /> is <see langword="null" />.</exception>
    /// <returns>
    /// <see langword="true" /> if the object that implements the <see cref="T:System.Collections.Generic.IReadOnlyDictionary`2" /> interface contains an element that has the specified key; otherwise, <see langword="false" />.</returns>
    bool IReadOnlyDictionary<string, object?>.TryGetValue(string key, [MaybeNullWhen(false)] out object? value) =>
        metadata.TryGetValue(key, out value);

    /// <summary>Gets the element that has the specified key in the read-only dictionary.</summary>
    /// <param name="key">The key to locate.</param>
    /// <exception cref="T:System.ArgumentNullException">
    /// <paramref name="key" /> is <see langword="null" />.</exception>
    /// <exception cref="T:System.Collections.Generic.KeyNotFoundException">The property is retrieved and <paramref name="key" /> is not found.</exception>
    /// <returns>The element that has the specified key in the read-only dictionary.</returns>
    public object? this[string key] => metadata[key];

    IEnumerable<string> IReadOnlyDictionary<string, object?>.Keys => metadata.Keys;

    IEnumerable<object?> IReadOnlyDictionary<string, object?>.Values => metadata.Values;

    /// <summary>
    /// Tries to get the value for the provided key. If the requested item cannot be found, the default value is returned.
    /// </summary>
    /// <typeparam name="TKey">The type of the key.</typeparam>
    /// <typeparam name="TValue">The type of the value.</typeparam>
    /// <param name="dictionary">The dictionary.</param>
    /// <param name="key">The item's key.</param>
    /// <param name="defaultValue">The default value to return if the item could not be found.</param>
    /// <returns>The found value, or the default value.</returns>
    protected static TValue? TryGetValue<TKey, TValue>(IDictionary<TKey, TValue> dictionary, [DisallowNull] TKey key, TValue? defaultValue = default)
    {
        dictionary = dictionary ?? throw new ArgumentNullException(nameof(dictionary));
        key = key ?? throw new ArgumentNullException(nameof(key));

        return dictionary.TryGetValue(key, out var value) ? value : defaultValue;
    }
}
