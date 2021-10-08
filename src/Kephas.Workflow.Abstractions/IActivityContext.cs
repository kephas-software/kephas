// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IActivityContext.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Declares the IActivityContext interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Workflow
{
    using System;

    using Kephas.Diagnostics.Contracts;
    using Kephas.Services;

    /// <summary>
    /// Interface for workflow execution context.
    /// </summary>
    public interface IActivityContext : IContext
    {
        /// <summary>
        /// Gets the workflow processor.
        /// </summary>
        /// <value>
        /// The workflow processor.
        /// </value>
        IWorkflowProcessor WorkflowProcessor { get; }

        /// <summary>
        /// Gets or sets the activity being executed.
        /// </summary>
        /// <remarks>
        /// The activity's input parameters are contained
        /// in the activity itself as members.
        /// </remarks>
        /// <value>
        /// The activity being executed.
        /// </value>
        IActivity? Activity { get; set; }

        /// <summary>
        /// Gets or sets the execution result.
        /// </summary>
        /// <remarks>
        /// The result contains as values the activity's output parameters.
        /// </remarks>
        /// <value>
        /// The execution result.
        /// </value>
        object? Result { get; set; }

        /// <summary>
        /// Gets or sets the execution exception.
        /// </summary>
        /// <value>
        /// The execution exception.
        /// </value>
        Exception? Exception { get; set; }

        /// <summary>
        /// Gets or sets the execution timeout.
        /// </summary>
        /// <value>
        /// The execution timeout.
        /// </value>
        TimeSpan? Timeout { get; set; }
    }

    /// <summary>
    /// Extension methods for <see cref="IActivityContext"/>.
    /// </summary>
    public static class ActivityContextExtensions
    {
        /// <summary>
        /// Sets the processing activity.
        /// </summary>
        /// <typeparam name="TContext">Type of the context.</typeparam>
        /// <param name="context">The activity context.</param>
        /// <param name="activity">The activity.</param>
        /// <returns>
        /// This <paramref name="context"/>.
        /// </returns>
        public static TContext Activity<TContext>(this TContext context, IActivity activity)
            where TContext : class, IActivityContext
        {
            context = context ?? throw new ArgumentNullException(nameof(context));
            Requires.NotNull(activity, nameof(activity));

            context.Activity = activity;

            return context;
        }

        /// <summary>
        /// Sets the processing result.
        /// </summary>
        /// <typeparam name="TContext">Type of the context.</typeparam>
        /// <param name="context">The activity context.</param>
        /// <param name="result">The processing result.</param>
        /// <returns>
        /// This <paramref name="context"/>.
        /// </returns>
        public static TContext Result<TContext>(this TContext context, object? result)
            where TContext : class, IActivityContext
        {
            context = context ?? throw new ArgumentNullException(nameof(context));

            context.Result = result;

            return context;
        }

        /// <summary>
        /// Sets the processing exception.
        /// </summary>
        /// <typeparam name="TContext">Type of the context.</typeparam>
        /// <param name="context">The activity context.</param>
        /// <param name="exception">The processing exception.</param>
        /// <returns>
        /// This <paramref name="context"/>.
        /// </returns>
        public static TContext Result<TContext>(this TContext context, Exception exception)
            where TContext : class, IActivityContext
        {
            context = context ?? throw new ArgumentNullException(nameof(context));

            context.Exception = exception;

            return context;
        }

        /// <summary>
        /// Sets the processing activity.
        /// </summary>
        /// <typeparam name="TContext">Type of the context.</typeparam>
        /// <param name="context">The activity context.</param>
        /// <param name="timeout">The timeout.</param>
        /// <returns>
        /// This <paramref name="context"/>.
        /// </returns>
        public static TContext Timeout<TContext>(this TContext context, TimeSpan? timeout)
            where TContext : class, IActivityContext
        {
            context = context ?? throw new ArgumentNullException(nameof(context));

            context.Timeout = timeout;

            return context;
        }
    }
}