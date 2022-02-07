// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RedisFindCommand.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Data.Redis.Commands;

using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using Kephas.Data.Commands;
using Kephas.Data.Redis;
using Kephas.Serialization;
using Kephas.Threading.Tasks;

/// <summary>
/// The redis find command.
/// </summary>
[DataContextType(typeof(RedisDataContext))]
public class RedisFindCommand : FindCommand
{
    /// <summary>
    /// The serialization service.
    /// </summary>
    private readonly ISerializationService serializationService;

    /// <summary>
    /// Initializes a new instance of the <see cref="RedisFindCommand"/> class.
    /// </summary>
    /// <param name="serializationService">The serialization service.</param>
    public RedisFindCommand(ISerializationService serializationService)
    {
        this.serializationService = serializationService;
    }

    /// <summary>
    /// Searches for the first entity matching the provided criteria and returns it asynchronously.
    /// </summary>
    /// <typeparam name="T">The type of the entity.</typeparam>
    /// <param name="findContext">The find context.</param>
    /// <param name="cancellationToken">The cancellation token (optional).</param>
    /// <returns>A promise of the found entity.</returns>
    protected override async Task<IFindResult> FindAsync<T>(IFindContext findContext, CancellationToken cancellationToken)
    {
        var dataContext = (RedisDataContext)findContext.DataContext;
        var criteria = this.GetFindCriteria<T>(findContext);

        var localCacheQuery = this.TryGetLocalCacheQuery<T>(findContext);
        if (localCacheQuery != null)
        {
            var result = localCacheQuery
                .Where(criteria.Compile())
                .Take(2)
                .ToList();
            if (result.Count > 0)
            {
                return this.GetFindResult(findContext, result, criteria);
            }
        }

        var entityHash = dataContext.GetEntityHash(findContext.EntityType);
        var id = findContext.Id?.ToString();

        if (!entityHash.TryGetValue(id, out var entityString))
        {
            return this.GetFindResult(findContext, Array.Empty<T>(), criteria);
        }

        var entity = await this.serializationService.JsonDeserializeAsync<T>(entityString, cancellationToken: cancellationToken).PreserveThreadContext();
        var entityInfo = dataContext.Attach(entity);
        return this.GetFindResult(findContext, new T[] { entity }, criteria);
    }
}