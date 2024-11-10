// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ISpParameter.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System.Data;

namespace Kephas.Entities.StoredProcedures;

/// <summary>
/// Provides information about a stored procedure parameter.
/// </summary>
public interface ISpParameter
{
    /// <summary>
    /// Gets the parameter direction.
    /// </summary>
    ParameterDirection Direction { get; }

    /// <summary>
    /// Gets the DB type.
    /// </summary>
    DbType? DbType { get; }

    /// <summary>
    /// Gets the native DB type.
    /// </summary>
    SqlDbType? SqlDbType { get; }

    /// <summary>
    /// Gets the native type name.
    /// </summary>
    string? TypeName { get; }

    /// <summary>
    /// Gets the size of the value.
    /// </summary>
    int? Size { get; }
}
