// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RefBase.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Implements the reference base class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Data
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;

    using Kephas.Data.Capabilities;
    using Kephas.Data.Commands;
    using Kephas.Diagnostics.Contracts;

    public abstract class RefBase<T> : IRef<T>
        where T : class
    {
        /// <summary>
        /// The entity information provider.
        /// </summary>
        private readonly IEntityInfoAware entityInfoProvider;

        /// <summary>
        /// Name of the reference identifier.
        /// </summary>
        private readonly string refIdName;

        /// <summary>
        /// Initializes a new instance of the <see cref="RefBase{T}"/> class.
        /// </summary>
        /// <param name="entityInfoProvider">The entity information provider.</param>
        /// <param name="refIdName">Name of the reference identifier.</param>
        protected RefBase(IEntityInfoAware entityInfoProvider, string refIdName)
        {
            Requires.NotNull(entityInfoProvider, nameof(entityInfoProvider));
            Requires.NotNullOrEmpty(refIdName, nameof(refIdName));

            this.entityInfoProvider = entityInfoProvider;
            this.refIdName = refIdName;
            this.EntityType = typeof(T);
        }

        /// <summary>
        /// Gets the identifier for this instance.
        /// </summary>
        /// <value>
        /// The identifier.
        /// </value>
        object IIdentifiable.Id => this.Id;

        /// <summary>
        /// Gets the type of the referenced entity.
        /// </summary>
        /// <value>
        /// The type of the referenced entity.
        /// </value>
        public Type EntityType { get; }

        /// <summary>
        /// Gets the referenced entity asynchronously.
        /// </summary>
        /// <param name="operationContext">The operationContext for finding the entity (optional).</param>
        /// <param name="cancellationToken">The cancellation token (optional).</param>
        /// <returns>
        /// A task promising the referenced entity.
        /// </returns>
        public Task<T> GetAsync(IFindContext<T> operationContext = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            var dataContext = this.entityInfoProvider.GetEntityInfo()?.DataContext;
            if (dataContext == null)
            {
                dataContext = operationContext?.DataContext;
            }

            if (dataContext == null)
            {
                // TODO localization
                throw new InvalidOperationException("Cannot retrieve a data context object.");
            }

            return dataContext.FindAsync<T>(operationContext, cancellationToken);
        }

        /// <summary>
        /// Gets the referenced entity asynchronously.
        /// </summary>
        /// <param name="operationContext">The context for finding the entity (optional).</param>
        /// <param name="cancellationToken">The cancellation token (optional).</param>
        /// <returns>
        /// A task promising the referenced entity.
        /// </returns>
        Task<object> IRef.GetAsync(IFindContext operationContext, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Gets or sets the identifier of the referenced entity.
        /// </summary>
        /// <value>
        /// The identifier of the referenced entity.
        /// </value>
        public object Id
        {
            get => throw new NotImplementedException();
            set => throw new NotImplementedException();
        }
    }
}