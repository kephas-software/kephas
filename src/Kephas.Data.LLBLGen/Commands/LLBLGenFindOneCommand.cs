// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LLBLGenFindOneCommand.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Implements the llbl generate find one command class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Data.LLBLGen.Commands
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;

    using Kephas.Activation;
    using Kephas.Data.Commands;
    using Kephas.Data.Linq;
    using Kephas.Data.LLBLGen.Entities;
    using Kephas.Logging;
    using Kephas.Reflection;
    using Kephas.Runtime;
    using Kephas.Threading.Tasks;

    /// <summary>
    /// A llbl generate find one command.
    /// </summary>
    [DataContextType(typeof(LLBLGenDataContext))]
    public class LLBLGenFindOneCommand : FindOneCommand
    {
        /// <summary>
        /// The entity activator.
        /// </summary>
        private readonly IActivator entityActivator;

        /// <summary>
        /// Initializes a new instance of the <see cref="LLBLGenFindOneCommand"/> class.
        /// </summary>
        /// <param name="entityActivator">The entity activator.</param>
        /// <param name="logManager">Optional. Manager for log.</param>
        public LLBLGenFindOneCommand(IEntityActivator entityActivator, ILogManager logManager = null)
            : base(logManager)
        {
            this.entityActivator = entityActivator;
        }

        /// <summary>
        /// Searches for the first entity matching the provided criteria and returns it asynchronously.
        /// </summary>
        /// <typeparam name="T">The type of the entity.</typeparam>
        /// <param name="findContext">The find context.</param>
        /// <param name="cancellationToken">The cancellation token (optional).</param>
        /// <returns>
        /// A promise of the found entity.
        /// </returns>
        protected override async Task<IFindResult> FindAsync<T>(
            IFindOneContext findContext,
            CancellationToken cancellationToken)
        {
            var dataContext = findContext.DataContext;
            var criteria = this.GetFindCriteria<T>(findContext);
            IList<T> result;

            var localCache = (LLBLGenCache)this.TryGetLocalCache(dataContext);
            if (localCache != null)
            {
                var runtimeEntityType = findContext.EntityType.AsRuntimeTypeInfo(findContext?.AmbientServices?.TypeRegistry);
                var underlyingEntityType = (IRuntimeTypeInfo)this.entityActivator.GetImplementationType(runtimeEntityType);
                result = localCache.GetAll(underlyingEntityType.Type).Cast<T>()
                    .Where(criteria.Compile())
                    .Take(2)
                    .ToList();
                if (result.Count > 0)
                {
                    return this.GetFindResult(findContext, result, criteria);
                }

                result = localCache.NewEntities.Values.OfType<T>()
                    .Where(criteria.Compile())
                    .Take(2)
                    .ToList();
                if (result.Count > 0)
                {
                    return this.GetFindResult(findContext, result, criteria);
                }
            }

            var query = dataContext.Query<T>().Where(criteria).Take(2);
            result = await query.ToListAsync(cancellationToken).PreserveThreadContext();
            return this.GetFindResult(findContext, result, criteria);
        }
    }
}