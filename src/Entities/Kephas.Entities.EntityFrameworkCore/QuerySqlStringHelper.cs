// --------------------------------------------------------------------------------------------------------------------
// <copyright file="QuerySqlStringHelper.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Entities;

internal static class QuerySqlStringHelper
{
    internal static readonly IDictionary<StorageKind, (string open, string close)> Brackets =
        new Dictionary<StorageKind, (string open, string close)>
        {
            { StorageKind.Table, ("", "") },
            { StorageKind.StoredProcedure, (" ", "") },
            { StorageKind.Function, ("(", ")") },
        };
}