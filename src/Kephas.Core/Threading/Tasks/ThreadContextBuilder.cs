// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ThreadContextBuilder.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Provides methods to build <see cref="ThreadContext" /> instances.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Threading.Tasks
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.Contracts;

    /// <summary>
    /// Provides methods to configure and build <see cref="ThreadContext"/> instances.
    /// </summary>
    public class ThreadContextBuilder
    {
        /// <summary>
        /// The server thread context store actions key.
        /// </summary>
        private const string ThreadingContextStoreActionsKey = "Kephas_ThreadContextStoreActions";

        /// <summary>
        /// The server thread context restore actions key.
        /// </summary>
        private const string ThreadingContextRestoreActionsKey = "Kephas_ThreadContextRestoreActions";

        /// <summary>
        /// The ambient services.
        /// </summary>
        private readonly IAmbientServices ambientServices;

        /// <summary>
        /// Initializes a new instance of the <see cref="ThreadContextBuilder"/> class.
        /// </summary>
        /// <param name="ambientServices">
        /// The ambient services.
        /// </param>
        public ThreadContextBuilder(IAmbientServices ambientServices)
        {
            Contract.Requires(ambientServices != null);

            this.ambientServices = ambientServices;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ThreadContextBuilder"/> class.
        /// </summary>
        public ThreadContextBuilder()
            : this(AmbientServices.Instance)
        {
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
            Contract.Requires(storeAction != null);

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
            Contract.Requires(restoreAction != null);

            var restoreActions = this.GetOrCreateContextActions(ThreadingContextRestoreActionsKey);
            restoreActions.Add(restoreAction);

            return this;
        }

        /// <summary>
        /// Converts this object to a <see cref="ThreadContext"/>.
        /// </summary>
        /// <returns>
        /// A <see cref="ThreadContext"/>.
        /// </returns>
        public ThreadContext AsThreadContext()
        {
            var storeActions = this.GetContextActions(ThreadingContextStoreActionsKey);
            var restoreActions = this.GetContextActions(ThreadingContextRestoreActionsKey);

            return new ThreadContext(this.ambientServices, storeActions, restoreActions);
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
                if (this.ambientServices != null)
                {
                    this.ambientServices[actionsKey] = actionsList;
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
            var rawActions = this.ambientServices?[actionsKey];

            var actions = rawActions as IList<Action<ThreadContext>>;

            if (rawActions != Undefined.Value && rawActions != null && actions == null)
            {
                throw new InvalidOperationException($"The ambient services are corrupt. The {nameof(ThreadContext)} actions ({actionsKey}) cannot be converted to a list anymore.");
            }

            return actions;
        }
    }
}