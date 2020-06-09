// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FuncJob.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the function job class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Scheduling.Jobs
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;

    using Kephas.Reflection;
    using Kephas.Runtime;
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
        private readonly IRuntimeTypeRegistry? typeRegistry;

        /// <summary>
        /// Initializes a new instance of the <see cref="FuncJob"/> class.
        /// </summary>
        /// <param name="operation">The operation.</param>
        /// <param name="typeRegistry">Optional. The type registry.</param>
        public FuncJob(Func<object?> operation, IRuntimeTypeRegistry? typeRegistry = null)
            : this(operation, () => new RuntimeFuncJobInfo(typeRegistry ?? RuntimeTypeRegistry.Instance, operation), typeRegistry)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FuncJob"/> class.
        /// </summary>
        /// <param name="asyncOperation">The asynchronous operation.</param>
        /// <param name="typeRegistry">Optional. The type registry.</param>
        public FuncJob(Func<CancellationToken, Task<object?>> asyncOperation, IRuntimeTypeRegistry? typeRegistry = null)
            : this(asyncOperation, () => new RuntimeFuncJobInfo(typeRegistry ?? RuntimeTypeRegistry.Instance, asyncOperation), typeRegistry)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FuncJob"/> class.
        /// </summary>
        /// <param name="operation">The operation.</param>
        /// <param name="typeRegistry">Optional. The type registry.</param>
        public FuncJob(Action operation, IRuntimeTypeRegistry? typeRegistry = null)
            : this(
                () =>
                    {
                        operation();
                        return (object?)null;
                    },
                typeRegistry)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FuncJob"/> class.
        /// </summary>
        /// <param name="operation">The operation.</param>
        /// <param name="typeInfoGetter">The type info getter.</param>
        /// <param name="typeRegistry">Optional. The type registry.</param>
        internal FuncJob(Func<object?> operation, Func<IJobInfo> typeInfoGetter, IRuntimeTypeRegistry? typeRegistry = null)
        {
            this.operation = operation;
            this.asyncOperation = _ => operation.AsAsync();
            this.lazyTypeInfo = new Lazy<IJobInfo>(typeInfoGetter);
            this.typeRegistry = typeRegistry;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FuncJob"/> class.
        /// </summary>
        /// <param name="asyncOperation">The asynchronous operation.</param>
        /// <param name="typeInfoGetter">The type info getter.</param>
        /// <param name="typeRegistry">Optional. The type registry.</param>
        internal FuncJob(Func<CancellationToken, Task<object?>> asyncOperation, Func<IJobInfo> typeInfoGetter, IRuntimeTypeRegistry? typeRegistry = null)
        {
            this.asyncOperation = asyncOperation;
            this.operation = () => asyncOperation(default).GetResultNonLocking();
            this.lazyTypeInfo = new Lazy<IJobInfo>(typeInfoGetter);
            this.typeRegistry = typeRegistry;
        }

        /// <summary>
        /// Gets the type registry.
        /// </summary>
        protected override IRuntimeTypeRegistry? TypeRegistry => this.typeRegistry;

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
        protected override ITypeInfo GetTypeInfoCore() => this.lazyTypeInfo.Value;
    }
}
