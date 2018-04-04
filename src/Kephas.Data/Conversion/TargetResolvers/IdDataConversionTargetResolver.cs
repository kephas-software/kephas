﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IdDataConversionTargetResolver.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the identifier data conversion target resolver class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Data.Conversion.TargetResolvers
{
    using System.Reflection;
    using System.Threading;
    using System.Threading.Tasks;

    using Kephas.Data.Capabilities;
    using Kephas.Data.Commands;
    using Kephas.Services;
    using Kephas.Threading.Tasks;

    /// <summary>
    /// An identifier data conversion target resolver.
    /// </summary>
    [ProcessingPriority(Priority.High)]
    public class IdDataConversionTargetResolver : IDataConversionTargetResolver<object, object>
    {
        /// <summary>
        /// Tries to resolve the target entity asynchronously.
        /// </summary>
        /// <param name="targetDataContext">Context for the target data.</param>
        /// <param name="targetType">The type of the target object.</param>
        /// <param name="sourceEntity"></param>
        /// <param name="sourceEntityInfo">The source entity information.</param>
        /// <param name="cancellationToken">The cancellation token (optional).</param>
        /// <returns>
        /// A promise of the target entity.
        /// </returns>
        public async Task<object> TryResolveTargetEntityAsync(IDataContext targetDataContext, TypeInfo targetType, object sourceEntity, IEntityInfo sourceEntityInfo, CancellationToken cancellationToken = default)
        {
            var sourceId = sourceEntityInfo?.EntityId ?? sourceEntity.ToExpando()[nameof(IIdentifiable.Id)];
            if (!Id.IsEmpty(sourceId))
            {
                var target = await targetDataContext.FindAsync(
                                 new FindContext(targetDataContext, targetType.AsType(), sourceId, throwIfNotFound: false),
                                 cancellationToken).PreserveThreadContext();
                return target;
            }

            return null;
        }
    }
}