// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CompletedTask.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   A completed task.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Threading.Tasks
{
    using System.Diagnostics.CodeAnalysis;
    using System.Threading.Tasks;

    /// <summary>
    /// A completed task returning a result of type <typeparamref name="TResult"/>.
    /// </summary>
    /// <typeparam name="TResult">Type of the result.</typeparam>
    public class CompletedTask<TResult>
    {
        /// <summary>
        /// Initializes static members of the <see cref="CompletedTask{TResult}"/> class.
        /// </summary>
        static CompletedTask()
        {
            Value = Task.FromResult(default(TResult));
        }

        /// <summary>
        /// Prevents a default instance of the <see cref="CompletedTask{TResult}"/> class from being created.
        /// </summary>
        private CompletedTask()
        {
        }

        /// <summary>
        /// Gets a completed task returning the default value of <typeparamref name="TResult"/>.
        /// </summary>
        public static Task<TResult> Value { get; }
    }

    /// <summary>
    /// A completed task.
    /// </summary>
    [SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1402:FileMayOnlyContainASingleClass", Justification = "Reviewed. Suppression is OK here.")]
    public class CompletedTask
    {
        /// <summary>
        /// Initializes static members of the <see cref="CompletedTask"/> class.
        /// </summary>
        static CompletedTask()
        {
            Value = Task.FromResult(0);
        }

        /// <summary>
        /// Prevents a default instance of the <see cref="CompletedTask"/> class from being created.
        /// </summary>
        private CompletedTask()
        {
        }

        /// <summary>
        /// Gets a completed task.
        /// </summary>
        public static Task Value { get; }
    }
}