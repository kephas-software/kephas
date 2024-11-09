// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EntityContextFeatureMetadata.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using Kephas.DependencyInjection;

namespace Kephas.Entities.Features;

/// <summary>
/// Marker interface for <see cref="IEntityContext"/> features.
/// </summary>
[SingletonService(MetadataType = typeof(EntityContextFeatureMetadata))]
public interface IEntityContextFeature;