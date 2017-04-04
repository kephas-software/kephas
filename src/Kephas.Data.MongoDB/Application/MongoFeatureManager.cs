// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MongoFeatureManager.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Implements the mongo application initializer class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Data.MongoDB.Application
{
    using System.Threading;
    using System.Threading.Tasks;

    using Kephas.Application;
    using Kephas.Data.MongoDB.Serialization;

    using global::MongoDB.Bson.Serialization;

    /// <summary>
    /// Feature manager for MongoDB.
    /// </summary>
    public class MongoFeatureManager : FeatureManagerBase
    {
        /// <summary>
        /// Initializes the MongoDB infrastructure asynchronously.
        /// </summary>
        /// <param name="appContext">Context for the application.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>
        /// A Task.
        /// </returns>
        protected override Task InitializeCoreAsync(IAppContext appContext, CancellationToken cancellationToken)
        {
            BsonSerializer.RegisterSerializer(typeof(Id), new IdBsonSerializer());

            return Task.CompletedTask;
        }
    }
}