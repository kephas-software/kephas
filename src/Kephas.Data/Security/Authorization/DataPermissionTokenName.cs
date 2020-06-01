// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DataPermissionTokenName.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Data.Security.Authorization
{
    /// <summary>
    /// Provides data permission names.
    /// </summary>
    public static class DataPermissionTokenName
    {
        /// <summary>
        /// The 'query' permission token name.
        /// </summary>
        public const string Query = "query";

        /// <summary>
        /// The 'create' permission token name.
        /// </summary>
        public const string Create = "create";

        /// <summary>
        /// The 'read' permission token name.
        /// </summary>
        public const string Read = "read";

        /// <summary>
        /// The 'update' permission token name.
        /// </summary>
        public const string Update = "update";

        /// <summary>
        /// The 'delete' permission token name.
        /// </summary>
        public const string Delete = "delete";

        /// <summary>
        /// The 'use' permission token name.
        /// </summary>
        public const string Use = "use";
    }
}