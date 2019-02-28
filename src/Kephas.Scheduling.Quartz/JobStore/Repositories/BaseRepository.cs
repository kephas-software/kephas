// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BaseRepository.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the base repository class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Scheduling.Quartz.JobStore.Repositories
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using Kephas.Data;
    using Kephas.Logging;

    internal abstract class BaseRepository<TDocument>
    {
        private static readonly HashSet<string> InitializedCollections = new HashSet<string>();

        protected BaseRepository(IDataContext dataContext, string instanceName)
        {
            this.DataContext = dataContext;
            this.InstanceName = instanceName;
        }

        /// <summary>
        /// Gets or sets the logger.
        /// </summary>
        /// <value>
        /// The logger.
        /// </value>
        public ILogger<BaseRepository<TDocument>> Log { get; set; }

        public IDataContext DataContext { get; }

        protected string InstanceName { get; }

        /* TODO

        protected IMongoCollection<TDocument> Collection { get; }

        protected FilterDefinitionBuilder<TDocument> FilterBuilder => Builders<TDocument>.Filter;

        protected UpdateDefinitionBuilder<TDocument> UpdateBuilder => Builders<TDocument>.Update;

        protected SortDefinitionBuilder<TDocument> SortBuilder => Builders<TDocument>.Sort;

        protected ProjectionDefinitionBuilder<TDocument> ProjectionBuilder => Builders<TDocument>.Projection;

        protected IndexKeysDefinitionBuilder<TDocument> IndexBuilder => Builders<TDocument>.IndexKeys;

        */

        public virtual Task EnsureIndex()
        {
            return Task.FromResult(0);
        }
    }
}