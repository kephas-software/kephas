// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ActivityBase.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the activity base class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Workflow
{
    using System;

    using Kephas.Dynamic;
    using Kephas.Operations;
    using Kephas.Reflection;
    using Kephas.Runtime;
    using Kephas.Workflow.Reflection;

    /// <summary>
    /// Base implementation of <see cref="IActivity"/>.
    /// </summary>
    public abstract class ActivityBase : OperationResult, IActivity
    {
        /// <summary>
        /// Gets or sets the target against which the activity is executed.
        /// </summary>
        /// <remarks>
        /// The target is typically the activity's container instance.
        /// For example, a user entity may contain a ChangePassword activity,
        /// in which case the target is the user.
        /// </remarks>
        /// <value>
        /// The target.
        /// </value>
        public object? Target { get; set; }

        /// <summary>
        /// Gets or sets the arguments for the execution.
        /// </summary>
        /// <value>
        /// The arguments.
        /// </value>
        public IExpando? Arguments { get; set; }

        /// <summary>
        /// Gets or sets the execution context.
        /// </summary>
        /// <value>
        /// The execution context.
        /// </value>
        public IActivityContext? Context { get; set; }

        /// <summary>
        /// Gets or sets the identifier for this instance.
        /// </summary>
        /// <value>
        /// The identifier.
        /// </value>
        public object Id { get; protected set; } = Guid.NewGuid();

        /// <summary>
        /// Gets the type information for this instance.
        /// </summary>
        /// <returns>
        /// The type information.
        /// </returns>
        public virtual IActivityInfo GetTypeInfo() => (IActivityInfo)this.GetTypeInfoBase();

        /// <summary>
        /// Gets the type information for this instance.
        /// </summary>
        /// <returns>
        /// The type information.
        /// </returns>
        ITypeInfo IInstance.GetTypeInfo() => this.GetTypeInfoBase();

        /// <summary>
        /// Gets the type information (overridable implementation).
        /// </summary>
        /// <returns>The type information.</returns>
        protected virtual ITypeInfo GetTypeInfoBase() => this.GetRuntimeTypeInfo(this.GetTypeRegistry())!;
    }
}