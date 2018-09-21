// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MongoAppLifecycleBehavior.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
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
    /// A MongoDB application lifecycle behavior.
    /// </summary>
    /// <remarks>
    /// Adds the <see cref="Id"/> temporary and empty values the ObjectId correspondents.
    /// </remarks>
    public class MongoAppLifecycleBehavior : AppLifecycleBehaviorBase
    {
        /// <summary>
        /// Interceptor called before the application starts its asynchronous initialization.
        /// </summary>
        /// <remarks>
        /// To interrupt the application initialization, simply throw an appropriate exception.
        /// </remarks>
        /// <param name="appContext">Context for the application.</param>
        /// <param name="cancellationToken">The cancellation token (optional).</param>
        /// <returns>
        /// A Task.
        /// </returns>
        public override Task BeforeAppInitializeAsync(IAppContext appContext, CancellationToken cancellationToken = default)
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