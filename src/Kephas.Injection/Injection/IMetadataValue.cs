// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IMetadataValue.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Contract for metadata values.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Injection
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Non-typed contract for metadata values.
    /// </summary>
    public interface IMetadataValue : IMetadataProvider
    {
        /// <summary>
        /// The attribute suffix.
        /// </summary>
        private const string AttributeSuffix = "Attribute";

        /// <summary>
        /// Gets the metadata value.
        /// </summary>
        /// <value>
        /// The metadata value.
        /// </value>
        object? Value { get; }

        /// <summary>
        /// Gets the metadata name. If the name is not provided, it is inferred from the attribute type name.
        /// </summary>
        string? Name => null;

        /// <summary>
        /// Gets the metadata as an enumeration of (name, value) pairs.
        /// </summary>
        /// <returns>An enumeration of (name, value) pairs.</returns>
        IEnumerable<(string name, object? value)> IMetadataProvider.GetMetadata()
        {
            var thisType = this.GetType();
            var name = this.Name ?? (this is Attribute ? GetMetadataNameFromAttributeType(thisType) : thisType.Name);
            yield return (name, this.Value);
        }

        /// <summary>
        /// Gets the metadata name from the attribute type.
        /// </summary>
        /// <param name="attributeType">Type of the attribute.</param>
        /// <returns>The metadata name from the attribute type.</returns>
        public static string GetMetadataNameFromAttributeType(Type attributeType)
        {
            var name = attributeType.Name;
            return name.EndsWith(AttributeSuffix) ? name[..^AttributeSuffix.Length] : name;
        }
    }

    /// <summary>
    /// Contract for metadata values.
    /// </summary>
    /// <typeparam name="TValue">The type of the value.</typeparam>
    public interface IMetadataValue<out TValue> : IMetadataValue
    {
        /// <summary>
        /// Gets the metadata value.
        /// </summary>
        /// <value>
        /// The metadata value.
        /// </value>
        object? IMetadataValue.Value => this.Value;

        /// <summary>
        /// Gets the metadata value.
        /// </summary>
        /// <value>
        /// The metadata value.
        /// </value>
        new TValue Value { get; }
    }
}