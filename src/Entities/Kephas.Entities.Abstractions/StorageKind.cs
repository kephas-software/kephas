// --------------------------------------------------------------------------------------------------------------------
// <copyright file="StorageKind.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Entities;

/// <summary>
/// Enumerates the storage kind.
/// </summary>
public enum StorageKind
{
    /// <summary>
    /// Entity is persisted in a table.
    /// </summary>
    Table,

    /// <summary>
    /// Entity is retrieved from a stored procedure.
    /// </summary>
    StoredProcedure,

    /// <summary>
    /// Entity is retrieved from a function.
    /// </summary>
    Function,
}