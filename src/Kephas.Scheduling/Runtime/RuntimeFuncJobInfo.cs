// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RuntimeFuncJobInfo.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the runtime func job information class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Scheduling.Runtime
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;

    using Kephas.Scheduling.Jobs;

    /// <summary>
    /// Information about the runtime job based on a function/delegate.
    /// </summary>
    public class RuntimeFuncJobInfo : RuntimeJobInfo
    {
        private readonly Func<CancellationToken, Task<object?>>? asyncOperation;
        private readonly Func<object?>? operation;

        /// <summary>
        /// Initializes a new instance of the <see cref="RuntimeFuncJobInfo"/> class.
        /// </summary>
        /// <param name="operation">The operation.</param>
        /// <param name="friendlyName">Optional. The friendly name.</param>
        protected internal RuntimeFuncJobInfo(Action operation, string? friendlyName = null)
            : this(
                () =>
                    {
                        operation();
                        return null;
                    },
                friendlyName)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RuntimeFuncJobInfo"/> class.
        /// </summary>
        /// <param name="operation">The operation.</param>
        /// <param name="friendlyName">Optional. The friendly name.</param>
        protected internal RuntimeFuncJobInfo(Func<object?> operation, string? friendlyName = null)
            : base(typeof(FuncJob))
        {
            this.operation = operation;
            this.FriendlyName = friendlyName;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RuntimeFuncJobInfo"/> class.
        /// </summary>
        /// <param name="asyncOperation">The async operation.</param>
        /// <param name="friendlyName">Optional. The friendly name.</param>
        protected internal RuntimeFuncJobInfo(Func<CancellationToken, Task<object?>> asyncOperation, string? friendlyName = null)
            : base(typeof(FuncJob))
        {
            this.asyncOperation = asyncOperation;
            this.FriendlyName = friendlyName;
        }

        /// <summary>
        /// Gets the friendly name.
        /// </summary>
        public string? FriendlyName { get; }

        /// <summary>
        /// Creates an instance with the provided arguments (if any).
        /// </summary>
        /// <param name="args">The arguments.</param>
        /// <returns>
        /// The new instance.
        /// </returns>
        public override object CreateInstance(IEnumerable<object?>? args = null)
        {
            if (args == null)
            {
                return this.asyncOperation != null
                    ? new FuncJob(this.asyncOperation, () => this)
                    : new FuncJob(this.operation!, () => this);
            }

            return base.CreateInstance(args);
        }

        /// <summary>
        /// Returns a string that represents the current object.
        /// </summary>
        /// <returns>
        /// A string that represents the current object.
        /// </returns>
        public override string ToString()
        {
            return this.FriendlyName ?? base.ToString();
        }
    }
}