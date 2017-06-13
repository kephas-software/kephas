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
    using Kephas.Threading.Tasks;

    using global::MongoDB.Bson;

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
            var originalIsTemporary = Id.IsTemporary;
            Id.IsTemporary = value =>
                {
                    if (value is ObjectId)
                    {
                        return (ObjectId)value < ObjectId.Empty;
                    }

                    return originalIsTemporary(value);
                };

            Id.AddEmptyValue(ObjectId.Empty);

            return TaskHelper.CompletedTask;
        }
    }
}