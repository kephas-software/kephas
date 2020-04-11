// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FuncJob.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the function job class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using Kephas.Reflection;

namespace Kephas.Scheduling.Jobs
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;

    using Kephas.Scheduling.Reflection;
    using Kephas.Scheduling.Runtime;
    using Kephas.Services;
    using Kephas.Threading.Tasks;

    /// <summary>
    /// A job based on a function.
    /// </summary>
    public class FuncJob : JobBase, IFuncJob
    {
        private readonly Lazy<IJobInfo> lazyTypeInfo;
        private readonly Func<object?> operation;
        private readonly Func<CancellationToken, Task<object?>> asyncOperation;

        /// <summary>
        /// Initializes a new instance of the <see cref="FuncJob"/> class.
        /// </summary>
        /// <param name="operation">The operation.</param>
        public FuncJob(Func<object?> operation)
            : this(operation, () => new RuntimeFuncJobInfo(operation))
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FuncJob"/> class.
        /// </summary>
        /// <param name="asyncOperation">The asynchronous operation.</param>
        public FuncJob(Func<CancellationToken, Task<object?>> asyncOperation)
            : this(asyncOperation, () => new RuntimeFuncJobInfo(asyncOperation))
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FuncJob"/> class.
        /// </summary>
        /// <param name="operation">The operation.</param>
        public FuncJob(Action operation)
            : this(() => { operation(); return (object?)null; })
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FuncJob"/> class.
        /// </summary>
        /// <param name="operation">The operation.</param>
        /// <param name="typeInfoGetter">The type info getter.</param>
        internal FuncJob(Func<object?> operation, Func<IJobInfo> typeInfoGetter)
        {
            this.operation = operation;
            this.asyncOperation = _ => operation.AsAsync();
            this.lazyTypeInfo = new Lazy<IJobInfo>(typeInfoGetter);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FuncJob"/> class.
        /// </summary>
        /// <param name="asyncOperation">The asynchronous operation.</param>
        /// <param name="typeInfoGetter">The type info getter.</param>
        internal FuncJob(Func<CancellationToken, Task<object?>> asyncOperation, Func<IJobInfo> typeInfoGetter)
        {
            this.asyncOperation = asyncOperation;
            this.operation = () => asyncOperation(default).GetResultNonLocking();
            this.lazyTypeInfo = new Lazy<IJobInfo>(typeInfoGetter);
        }

        /// <summary>
        /// Executes the operation in the given context.
        /// </summary>
        /// <param name="context">Optional. The context.</param>
        /// <returns>
        /// An object.
        /// </returns>
        public object? Execute(IContext? context = null)
            => this.operation();

        /// <summary>
        /// Executes the operation asynchronously in the given context.
        /// </summary>
        /// <param name="context">Optional. The context.</param>
        /// <param name="cancellationToken">Optional. A token that allows processing to be cancelled.</param>
        /// <returns>
        /// An object.
        /// </returns>
        public Task<object?> ExecuteAsync(IContext? context = null, CancellationToken cancellationToken = default)
            => this.asyncOperation(cancellationToken);

        /// <summary>
        /// Gets the type information.
        /// </summary>
        /// <returns>The type information.</returns>
        protected override ITypeInfo GetTypeInfoBase() => this.lazyTypeInfo.Value;
    }
}
