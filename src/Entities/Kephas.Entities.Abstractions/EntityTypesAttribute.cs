// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EntityTypesAttribute.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using Kephas.DependencyInjection.Metadata;

namespace Kephas.Entities;

/// <summary>
/// Metadata attribute providing an enumeration of supported entity types.
/// </summary>
/// <param name="entityTypes">The supported entity types.</param>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
public sealed class EntityTypesAttribute(params Type[] entityTypes) : ServiceMetadataValueAttributeBase<IEnumerable<Type>>(entityTypes);