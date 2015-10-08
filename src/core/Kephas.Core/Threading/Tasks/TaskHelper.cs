// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TaskHelper.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Helper class for tasks.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Threading.Tasks
{
    using System.Diagnostics.Contracts;
    using System.Threading.Tasks;

    /// <summary>
    /// Helper class for tasks.
    /// </summary>
    public static class TaskHelper
    {
        /// <summary>
        /// The default value of milliseconds to wait when simulating synchronous calls.
        /// </summary>
        public const int DefaultWaitMilliseconds = 20;

        /// <summary>
        /// Gets a resolved task returning the default value of <typeparamref name="T"/>.
        /// </summary>
        /// <typeparam name="T">The result type.</typeparam>
        public static Task<T> EmptyTask<T>() => Task.FromResult(default(T));

        /// <summary>
        /// Gets a task awaiter preserving the current server context upon continuation.
        /// </summary>
        /// <remarks>
        /// ConfigureAwait(false) is called and the current culture and current UI culture are preserved.
        /// </remarks>
        /// <typeparam name="TResult">Type of the result.</typeparam>
        /// <param name="task">The task.</param>
        /// <returns>
        /// A <see cref="WithServerContextAwaiter{TResult}"/>.
        /// </returns>
        public static WithServerContextAwaiter<TResult> WithServerContext<TResult>(this Task<TResult> task)
        {
            Contract.Requires(task != null);

            return new WithServerContextAwaiter<TResult>(task);
        }

        /// <summary>
        /// Gets a task awaiter preserving the current culture upon continuation.
        /// </summary>
        /// <remarks>
        /// ConfigureAwait(false) is called and the current culture and current UI culture are preserved.
        /// </remarks>
        /// <param name="task">The task.</param>
        /// <returns>
        /// A <see cref="WithServerContextAwaiter"/>.
        /// </returns>
        public static WithServerContextAwaiter WithServerContext(this Task task)
        {
            Contract.Requires(task != null);

            return new WithServerContextAwaiter(task);
        }
    }
}