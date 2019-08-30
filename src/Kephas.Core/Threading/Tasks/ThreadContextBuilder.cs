// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ThreadContextBuilder.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Provides methods to build <see cref="ThreadContext" /> instances.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Threading.Tasks
{
    using System;
    using System.Collections.Generic;

    using Kephas.Diagnostics.Contracts;
    using Kephas.Dynamic;

    /// <summary>
    /// Provides methods to configure and build <see cref="ThreadContext"/> instances.
    /// </summary>
    public class ThreadContextBuilder
    {
        private const string ThreadingContextStoreActionsKey = "__ThreadContextStoreActions";

        private const string ThreadingContextRestoreActionsKey = "__ThreadContextRestoreActions";

        private static readonly IExpando globalThreadContextPool = new Expando(isThreadSafe: true);

        private readonly IExpando threadContextPool;

        /// <summary>
        /// Initializes a new instance of the <see cref="ThreadContextBuilder"/> class.
        /// </summary>
        public ThreadContextBuilder()
            : this(globalThreadContextPool)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ThreadContextBuilder"/> class.
        /// </summary>
        /// <param name="threadContextPool">The thread context pool.</param>
        internal ThreadContextBuilder(IExpando threadContextPool)
        {
            Requires.NotNull(threadContextPool, nameof(threadContextPool));

            this.threadContextPool = threadContextPool;
        }

        /// <summary>
        /// Adds the store action to the ambient services. This method is not thread safe, use it with caution and only when initializing the server.
        /// </summary>
        /// <param name="storeAction">The store action.</param>
        /// <returns>
        /// This builder.
        /// </returns>
        public ThreadContextBuilder WithStoreAction(Action<ThreadContext> storeAction)
        {
            Requires.NotNull(storeAction, nameof(storeAction));

            var storeActions = this.GetOrCreateContextActions(ThreadingContextStoreActionsKey);
            storeActions.Add(storeAction);

            return this;
        }

        /// <summary>
        /// Adds the restore action to the ambient services. This method is not thread safe, use it with caution and only when initializing the server.
        /// </summary>
        /// <param name="restoreAction">The restore action.</param>
        /// <returns>
        /// This builder.
        /// </returns>
        public ThreadContextBuilder WithRestoreAction(Action<ThreadContext> restoreAction)
        {
            Requires.NotNull(restoreAction, nameof(restoreAction));

            var restoreActions = this.GetOrCreateContextActions(ThreadingContextRestoreActionsKey);
            restoreActions.Add(restoreAction);

            return this;
        }

        /// <summary>
        /// Creates a new <see cref="ThreadContext"/>.
        /// </summary>
        /// <returns>
        /// A <see cref="ThreadContext"/>.
        /// </returns>
        public ThreadContext CreateThreadContext()
        {
            var storeActions = this.GetContextActions(ThreadingContextStoreActionsKey);
            var restoreActions = this.GetContextActions(ThreadingContextRestoreActionsKey);

            return new ThreadContext(storeActions, restoreActions);
        }

        /// <summary>
        /// Gets the context actions list with the provided key and, if the list does not exist, creates it.
        /// </summary>
        /// <param name="actionsKey">The actions key.</param>
        /// <returns>
        /// The context actions list.
        /// </returns>
        private IList<Action<ThreadContext>> GetOrCreateContextActions(string actionsKey)
        {
            var actionsList = this.GetContextActions(actionsKey);
            if (actionsList == null)
            {
                actionsList = new List<Action<ThreadContext>>();
                if (this.threadContextPool != null)
                {
                    this.threadContextPool[actionsKey] = actionsList;
                }
            }

            return actionsList;
        }

        /// <summary>
        /// Gets the context actions list with the provided key.
        /// </summary>
        /// <param name="actionsKey">The actions key.</param>
        /// <returns>
        /// The context actions list.
        /// </returns>
        private IList<Action<ThreadContext>> GetContextActions(string actionsKey)
        {
            var rawActions = this.threadContextPool?[actionsKey];

            var actions = rawActions as IList<Action<ThreadContext>>;

            if (rawActions != null && actions == null)
            {
                throw new InvalidOperationException($"The thread context pool is corrupt. The {nameof(ThreadContext)} actions ({actionsKey}) cannot be converted to a list anymore.");
            }

            return actions;
        }
    }
}