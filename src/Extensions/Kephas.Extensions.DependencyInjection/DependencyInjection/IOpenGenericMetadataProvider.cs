// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IOpenGenericMetadataProvider.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Extensions.DependencyInjection;

/// <summary>
/// Service providing implementation and metadata for open generic types.
/// </summary>
/// <remarks>
/// The dictionary key is the service type, and the value is a pair with the implementation type and the metadata.
/// </remarks>
public interface IOpenGenericMetadataProvider : IDictionary<Type, List<KeyValuePair<Type, IDictionary<string, object?>>>>;