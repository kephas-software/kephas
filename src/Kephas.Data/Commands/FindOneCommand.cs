// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FindOneCommand.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the find one command class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Data.Commands
{
    using System;
    using System.Linq.Expressions;
    using System.Threading;
    using System.Threading.Tasks;

    using Kephas.Diagnostics.Contracts;

    /// <summary>
    /// Base class for find commands retrieving one result.
    /// </summary>
    [DataContextType(typeof(DataContextBase))]
    public class FindOneCommand : FindCommandBase<IFindOneContext>, IFindOneCommand
    {
        /// <summary>
        /// Executes the data command asynchronously.
        /// </summary>
        /// <param name="operationContext">The operation context.</param>
        /// <param name="cancellationToken">The cancellation token (optional).</param>
        /// <returns>
        /// A promise of a <see cref="IDataCommandResult"/>.
        /// </returns>
        public override Task<IFindResult> ExecuteAsync(IFindOneContext operationContext, CancellationToken cancellationToken = default)
        {
            Requires.NotNull(operationContext.Criteria, nameof(operationContext.Criteria));

            return base.ExecuteAsync(operationContext, cancellationToken);
        }

        /// <summary>
        /// Gets the find criteria.
        /// </summary>
        /// <typeparam name="T">Generic type parameter.</typeparam>
        /// <param name="findContext">The find context.</param>
        /// <returns>
        /// The find criteria.
        /// </returns>
        protected override Expression<Func<T, bool>> GetFindCriteria<T>(IFindOneContext findContext)
        {
            return (Expression<Func<T, bool>>)findContext.Criteria;
        }
    }
}