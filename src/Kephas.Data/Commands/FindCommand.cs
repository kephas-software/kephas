﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FindCommand.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the find command base class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Data.Commands
{
    using System;
    using System.Linq.Expressions;
    using System.Threading;
    using System.Threading.Tasks;

    using Kephas.Data.Resources;
    using Kephas.Logging;

    /// <summary>
    /// Base class for find commands.
    /// </summary>
    [DataContextType(typeof(DataContextBase))]
    public class FindCommand : FindCommandBase<IFindContext>, IFindCommand
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FindCommand"/> class.
        /// </summary>
        /// <param name="logManager">Optional. Manager for log.</param>
        public FindCommand(ILogManager? logManager = null)
            : base(logManager)
        {
        }

        /// <summary>
        /// Executes the data command asynchronously.
        /// </summary>
        /// <param name="operationContext">The operation context.</param>
        /// <param name="cancellationToken">The cancellation token (optional).</param>
        /// <returns>
        /// A promise of a <see cref="IOperationResult"/>.
        /// </returns>
        public override Task<IFindResult> ExecuteAsync(IFindContext operationContext, CancellationToken cancellationToken = default)
        {
            if (Id.IsEmpty(operationContext.Id))
            {
                throw new ArgumentException(Strings.DataContext_FindAsync_IdEmpty_Exception, nameof(operationContext.Id));
            }

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
        protected override Expression<Func<T, bool>> GetFindCriteria<T>(IFindContext findContext)
        {
            return findContext.DataContext.GetIdEqualityExpression<T>(findContext.Id);
        }

        /// <summary>
        /// Gets the criteria string for exception display.
        /// </summary>
        /// <typeparam name="T">The entity type.</typeparam>
        /// <param name="findContext">The find context.</param>
        /// <param name="criteria">The criteria.</param>
        /// <returns>
        /// The criteria string.
        /// </returns>
        protected override string GetCriteriaString<T>(IFindContext findContext, Expression<Func<T, bool>> criteria)
        {
            return $"{nameof(IIdentifiable.Id)} == {findContext.Id}";
        }
    }
}