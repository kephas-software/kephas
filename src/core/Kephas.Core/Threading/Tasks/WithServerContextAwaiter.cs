// --------------------------------------------------------------------------------------------------------------------
// <copyright file="WithServerContextAwaiter.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Awaiter preserving the server context.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Threading.Tasks
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Diagnostics.Contracts;
    using System.Globalization;
    using System.Runtime.CompilerServices;
    using System.Threading.Tasks;

    /// <summary>
    /// Awaiter preserving the server context.
    /// </summary>
    /// <typeparam name="TResult">Type of the result.</typeparam>
    public class WithServerContextAwaiter<TResult> : INotifyCompletion, ICriticalNotifyCompletion
    {
        /// <summary>
        /// The awaiter.
        /// </summary>
        private readonly ConfiguredTaskAwaitable<TResult>.ConfiguredTaskAwaiter awaiter;

        /// <summary>
        /// The culture.
        /// </summary>
        private CultureInfo culture;

        /// <summary>
        /// The UI culture.
        /// </summary>
        private CultureInfo uiculture;

        /// <summary>
        /// Initializes a new instance of the <see cref="WithServerContextAwaiter{TResult}"/> class.
        /// </summary>
        /// <param name="task">The task.</param>
        public WithServerContextAwaiter(Task<TResult> task)
        {
            Contract.Requires(task != null);

            var configuredTaskAwaitable = task.ConfigureAwait(false);
            this.awaiter = configuredTaskAwaitable.GetAwaiter();
        }

        /// <summary>
        /// Gets a value indicating whether the awaiter is completed.
        /// </summary>
        /// <value>
        /// <c>true</c> if this object is completed, <c>false</c> if not.
        /// </value>
        public bool IsCompleted => this.awaiter.IsCompleted;

        /// <summary>
        /// Schedules the continuation action that's invoked when the instance completes.
        /// </summary>
        /// <param name="continuation">The action to invoke when the operation completes.</param><exception cref="T:System.ArgumentNullException">The <paramref name="continuation"/> argument is null (Nothing in Visual Basic).</exception>
        public void OnCompleted(Action continuation)
        {
            this.culture = CultureInfo.CurrentCulture;
            this.uiculture = CultureInfo.CurrentUICulture;
            // ReSharper disable once ImpureMethodCallOnReadonlyValueField
            this.awaiter.OnCompleted(continuation);
        }

        /// <summary>
        /// Schedules the continuation action that's invoked when the instance completes.
        /// </summary>
        /// <param name="continuation">The action to invoke when the operation completes.</param><exception cref="T:System.ArgumentNullException">The <paramref name="continuation"/> argument is null (Nothing in Visual Basic).</exception>
        public void UnsafeOnCompleted(Action continuation)
        {
            this.culture = CultureInfo.CurrentCulture;
            this.uiculture = CultureInfo.CurrentUICulture;
            // ReSharper disable once ImpureMethodCallOnReadonlyValueField
            this.awaiter.UnsafeOnCompleted(continuation);
        }

        /// <summary>
        /// Gets this awaiter.
        /// </summary>
        /// <returns>
        /// The awaiter.
        /// </returns>
        public WithServerContextAwaiter<TResult> GetAwaiter() => this;

        /// <summary>
        /// Notifies the awaiter to get the result.
        /// </summary>
        /// <returns>
        /// The result returned by the task.
        /// </returns>
        public TResult GetResult()
        {
            if (this.culture != null)
            {
                // TODO support this
                ////CultureInfo.CurrentCulture = this.culture;
            }

            if (this.uiculture != null)
            {
                // TODO support this
                ////CultureInfo.CurrentUICulture = this.uiculture;
            }

            // ReSharper disable once ImpureMethodCallOnReadonlyValueField
            return this.awaiter.GetResult();
        }
    }

    /// <summary>
    /// Awaiter preserving the server context.
    /// </summary>
    [SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1402:FileMayOnlyContainASingleClass", Justification = "Reviewed. Suppression is OK here.")]
    public class WithServerContextAwaiter : INotifyCompletion, ICriticalNotifyCompletion
    {
        /// <summary>
        /// The awaiter.
        /// </summary>
        private readonly ConfiguredTaskAwaitable.ConfiguredTaskAwaiter awaiter;

        /// <summary>
        /// The culture.
        /// </summary>
        private CultureInfo culture;

        /// <summary>
        /// The UI culture.
        /// </summary>
        private CultureInfo uiculture;

        /// <summary>
        /// Initializes a new instance of the <see cref="WithServerContextAwaiter"/> class.
        /// </summary>
        /// <param name="task">The task.</param>
        public WithServerContextAwaiter(Task task)
        {
            Contract.Requires(task != null);

            var configuredTaskAwaitable = task.ConfigureAwait(false);
            this.awaiter = configuredTaskAwaitable.GetAwaiter();
        }

        /// <summary>
        /// Gets a value indicating whether the awaiter is completed.
        /// </summary>
        /// <value>
        /// <c>true</c> if this object is completed, <c>false</c> if not.
        /// </value>
        public bool IsCompleted => this.awaiter.IsCompleted;

        /// <summary>
        /// Schedules the continuation action that's invoked when the instance completes.
        /// </summary>
        /// <param name="continuation">The action to invoke when the operation completes.</param><exception cref="T:System.ArgumentNullException">The <paramref name="continuation"/> argument is null (Nothing in Visual Basic).</exception>
        public void OnCompleted(Action continuation)
        {
            this.culture = CultureInfo.CurrentCulture;
            this.uiculture = CultureInfo.CurrentUICulture;
            // ReSharper disable once ImpureMethodCallOnReadonlyValueField
            this.awaiter.OnCompleted(continuation);
        }

        /// <summary>
        /// Schedules the continuation action that's invoked when the instance completes.
        /// </summary>
        /// <param name="continuation">The action to invoke when the operation completes.</param><exception cref="T:System.ArgumentNullException">The <paramref name="continuation"/> argument is null (Nothing in Visual Basic).</exception>
        public void UnsafeOnCompleted(Action continuation)
        {
            this.culture = CultureInfo.CurrentCulture;
            this.uiculture = CultureInfo.CurrentUICulture;
            // ReSharper disable once ImpureMethodCallOnReadonlyValueField
            this.awaiter.UnsafeOnCompleted(continuation);
        }

        /// <summary>
        /// Gets this awaiter.
        /// </summary>
        /// <returns>
        /// The awaiter.
        /// </returns>
        public WithServerContextAwaiter GetAwaiter() => this;

        /// <summary>
        /// Notifies the awaiter to get the result.
        /// </summary>
        public void GetResult()
        {
            if (this.culture != null)
            {
                // TODO support this
                ////CultureInfo.CurrentCulture = this.culture;
            }

            if (this.uiculture != null)
            {
                // TODO support this
                ////CultureInfo.CurrentUICulture = this.uiculture;
            }

            // ReSharper disable once ImpureMethodCallOnReadonlyValueField
            this.awaiter.GetResult();
        }
    }
}
