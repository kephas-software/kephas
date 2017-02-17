// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DataStoreKind.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Implements the data store kind class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Data.Store
{
    /// <summary>
    /// Values that represent data store kinds.
    /// </summary>
    public enum DataStoreKind
    {
        /// <summary>
        /// In-Memory database.
        /// </summary>
        InMemory,

        /// <summary>
        /// MongoDB NoSQL database.
        /// </summary>
        MongoDB,

        /// <summary>
        /// DocumentDB NoSQL database.
        /// </summary>
        DocumentDB,

        /// <summary>
        /// Microsoft SQL Server database.
        /// </summary>
        SqlServer,

        /// <summary>
        /// An enum constant representing the oracle option.
        /// </summary>
        Oracle,

        /// <summary>
        /// An enum constant representing the postgre SQL option.
        /// </summary>
        PostgreSql,

        /// <summary>
        /// An enum constant representing my SQL option.
        /// </summary>
        MySql,

        /// <summary>
        /// An enum constant representing the maria database option.
        /// </summary>
        MariaDB,
    }
}