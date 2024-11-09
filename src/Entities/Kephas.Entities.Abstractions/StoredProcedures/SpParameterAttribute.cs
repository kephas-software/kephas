// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SpParameterAttribute.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System.Data;

namespace Kephas.Entities.StoredProcedures;

/// <summary>
/// Marks a property as an output argument in a stored procedure.
/// </summary>
[AttributeUsage(AttributeTargets.Property | AttributeTargets.Parameter, Inherited = true, AllowMultiple = false)]
public sealed class SpParameterAttribute : Attribute, ISpParameter
{
    /// <summary>
    /// Initializes a new instance of the <see cref="SpParameterAttribute"/> class
    /// using the inferred values from the property type.
    /// </summary>
    public SpParameterAttribute()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="SpParameterAttribute"/> class
    /// using the provided <see cref="DbType"/>.
    /// </summary>
    public SpParameterAttribute(DbType dbType)
    {
        DbType = dbType;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="SpParameterAttribute"/> class
    /// using the provided <see cref="SqlDbType"/>.
    /// </summary>
    public SpParameterAttribute(SqlDbType sqlDbType)
    {
        SqlDbType = sqlDbType;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="SpParameterAttribute"/> class
    /// using the provided <see cref="DbType"/> and <see cref="SqlDbType"/>.
    /// </summary>
    public SpParameterAttribute(DbType dbType, SqlDbType sqlDbType)
    {
        DbType = dbType;
        SqlDbType = sqlDbType;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="SpParameterAttribute"/> class
    /// using the provided <see cref="DbType"/> and <see cref="SqlDbType"/>.
    /// </summary>
    public SpParameterAttribute(DbType dbType, SqlDbType sqlDbType, int size)
        : this(dbType, sqlDbType)
    {
        Size = size;
    }

    /// <summary>
    /// Gets or sets the parameter direction.
    /// </summary>
    public ParameterDirection Direction { get; set; } = ParameterDirection.Input;

    /// <summary>
    /// Gets the DB type.
    /// </summary>
    public DbType? DbType { get; }

    /// <summary>
    /// Gets the native DB type.
    /// </summary>
    public SqlDbType? SqlDbType { get; }

    /// <summary>
    /// Gets or sets the native type name.
    /// </summary>
    public string? TypeName { get; set; }

    /// <summary>
    /// Gets the size.
    /// </summary>
    public int? Size { get; }
}