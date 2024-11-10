// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DomainDependenciesAttribute.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using Kephas.DependencyInjection.Metadata;

namespace Kephas.Ontology;

/// <summary>
/// Metadata attribute for the <see cref="IDomain"/> dependencies.
/// </summary>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
public class DomainDependenciesAttribute(params Type[] value)
    : ServiceMetadataValueAttributeBase<IEnumerable<Type>>(value);

/// <summary>
/// Metadata attribute for the <see cref="IDomain"/> dependencies.
/// </summary>
/// <typeparam name="T">The single domain dependency.</typeparam>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
public class DomainDependenciesAttribute<T>()
    : DomainDependenciesAttribute(typeof(T))
    where T : IDomain;

/// <summary>
/// Metadata attribute for the <see cref="IDomain"/> dependencies.
/// </summary>
/// <typeparam name="T1">The first domain dependency.</typeparam>
/// <typeparam name="T2">The second domain dependency.</typeparam>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
public class DomainDependenciesAttribute<T1, T2>()
    : DomainDependenciesAttribute(typeof(T1), typeof(T2))
    where T1 : IDomain
    where T2 : IDomain;

/// <summary>
/// Metadata attribute for the <see cref="IDomain"/> dependencies.
/// </summary>
/// <typeparam name="T1">The first domain dependency.</typeparam>
/// <typeparam name="T2">The second domain dependency.</typeparam>
/// <typeparam name="T3">The third domain dependency.</typeparam>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
public class DomainDependenciesAttribute<T1, T2, T3>()
    : DomainDependenciesAttribute(typeof(T1), typeof(T2), typeof(T3))
    where T1 : IDomain
    where T2 : IDomain
    where T3 : IDomain;

/// <summary>
/// Metadata attribute for the <see cref="IDomain"/> dependencies.
/// </summary>
/// <typeparam name="T1">The first domain dependency.</typeparam>
/// <typeparam name="T2">The second domain dependency.</typeparam>
/// <typeparam name="T3">The third domain dependency.</typeparam>
/// <typeparam name="T4">The fourth domain dependency.</typeparam>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
public class DomainDependenciesAttribute<T1, T2, T3, T4>()
    : DomainDependenciesAttribute(typeof(T1), typeof(T2), typeof(T3), typeof(T4))
    where T1 : IDomain
    where T2 : IDomain
    where T3 : IDomain
    where T4 : IDomain;

/// <summary>
/// Metadata attribute for the <see cref="IDomain"/> dependencies.
/// </summary>
/// <typeparam name="T1">The first domain dependency.</typeparam>
/// <typeparam name="T2">The second domain dependency.</typeparam>
/// <typeparam name="T3">The third domain dependency.</typeparam>
/// <typeparam name="T4">The fourth domain dependency.</typeparam>
/// <typeparam name="T5">The fifth domain dependency.</typeparam>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
public class DomainDependenciesAttribute<T1, T2, T3, T4, T5>()
    : DomainDependenciesAttribute(typeof(T1), typeof(T2), typeof(T3), typeof(T4), typeof(T5))
    where T1 : IDomain
    where T2 : IDomain
    where T3 : IDomain
    where T4 : IDomain
    where T5 : IDomain;