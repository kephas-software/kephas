// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EntityContextTypeAttribute.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using Kephas.DependencyInjection.Metadata;

namespace Kephas.Entities;

/// <summary>
/// Metadata attribute providing the entity context type.
/// </summary>
/// <typeparam name="T">The entity context type.</typeparam>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
public sealed class EntityContextTypeAttribute<T>() : ServiceMetadataValueAttributeBase<Type>(typeof(T));