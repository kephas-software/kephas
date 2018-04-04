// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MongoFeatureManager.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
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
    [FeatureInfo(FeatureName, isRequired: true)]
    public class MongoFeatureManager : FeatureManagerBase
    {
        /// <summary>
        /// Name of the feature.
        /// </summary>
        public const string FeatureName = "MongoDB";

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
                    if (value is ObjectId id)
                    {
                        return id < ObjectId.Empty;
                    }

                    return originalIsTemporary(value);
                };

            Id.AddEmptyValue(ObjectId.Empty);

            return TaskHelper.CompletedTask;
        }
    }
}