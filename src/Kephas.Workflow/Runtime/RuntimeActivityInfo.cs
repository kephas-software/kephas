// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RuntimeActivityInfo.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the runtime activity information class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Workflow.Runtime
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;

    using Kephas.Dynamic;
    using Kephas.Reflection;
    using Kephas.Runtime;
    using Kephas.Workflow.Reflection;

    /// <summary>
    /// Information about the runtime activity.
    /// </summary>
    public class RuntimeActivityInfo : RuntimeTypeInfo, IActivityInfo
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RuntimeActivityInfo"/> class.
        /// </summary>
        /// <param name="type">The type.</param>
        protected internal RuntimeActivityInfo(Type type)
            : base(type)
        {
        }

        /// <summary>
        /// Gets the return type of the method.
        /// </summary>
        /// <value>
        /// The return type of the method.
        /// </value>
        public ITypeInfo ReturnType { get; }

        /// <summary>
        /// Gets the method parameters.
        /// </summary>
        /// <value>
        /// The method parameters.
        /// </value>
        public IEnumerable<IParameterInfo> Parameters { get; }

        /// <summary>
        /// Executes the activity asynchronously.
        /// </summary>
        /// <param name="activity">The activity to execute.</param>
        /// <param name="target">The activity target.</param>
        /// <param name="arguments">The execution arguments.</param>
        /// <param name="context">The execution context.</param>
        /// <param name="cancellationToken">Optional. The cancellation token.</param>
        /// <returns>
        /// An asynchronous result that yields the output.
        /// </returns>
        public virtual async Task<object> ExecuteAsync(
            IActivity activity,
            object target,
            IExpando arguments,
            IActivityContext context,
            CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }
    }
}