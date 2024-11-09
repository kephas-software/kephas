// --------------------------------------------------------------------------------------------------------------------
// <copyright file="StoredProcedureAttribute.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Entities.StoredProcedures;

/// <summary>
/// Annotates entities with stored procedure storage related information.
/// </summary>
/// <param name="name">The name of the stored procedure.</param>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
public class StoredProcedureAttribute(string name) : Attribute()
{
    /// <summary>
    /// Gets the stored procedure name.
    /// </summary>
    public string Name { get; } = name;

    /// <summary>
    /// Gets the storage kind.
    /// </summary>
    public StorageKind StorageKind { get; init; } = StorageKind.StoredProcedure;
}