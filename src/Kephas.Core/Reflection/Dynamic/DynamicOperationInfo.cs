// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DynamicOperationInfo.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the dynamic operation information class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Reflection.Dynamic
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Information about the dynamic operation.
    /// </summary>
    public class DynamicOperationInfo : DynamicElementInfo, IOperationInfo
    {
        private readonly ICollection<IParameterInfo> parameters;
        private ITypeInfo? returnType;
        private string? returnTypeName;

        /// <summary>
        /// Initializes a new instance of the <see cref="DynamicOperationInfo"/> class.
        /// </summary>
        public DynamicOperationInfo()
        {
            this.parameters = new DynamicElementInfoCollection<IParameterInfo>(this);
        }

        /// <summary>
        /// Gets or sets the return type of the method.
        /// </summary>
        /// <value>
        /// The return type of the method.
        /// </value>
        public ITypeInfo? ReturnType
        {
            get => this.returnType ??= this.TryGetType(this.returnTypeName);
            set => this.returnType = value;
        }

        /// <summary>
        /// Gets or sets the name of the return type of the method.
        /// </summary>
        /// <value>
        /// The name of the return type of the method.
        /// </value>
        public string? ReturnTypeName
        {
            get => this.returnTypeName ?? this.returnType?.FullName;
            set
            {
                this.returnTypeName = value;
                this.returnType = null;
            }
        }

        /// <summary>
        /// Gets the method parameters.
        /// </summary>
        /// <value>
        /// The method parameters.
        /// </value>
        IEnumerable<IParameterInfo> IOperationInfo.Parameters => this.parameters;

        /// <summary>
        /// Gets the method parameters.
        /// </summary>
        /// <value>
        /// The method parameters.
        /// </value>
        public ICollection<IParameterInfo> Parameters => this.parameters;

        /// <summary>
        /// Invokes the specified method on the provided instance.
        /// </summary>
        /// <param name="instance">The instance.</param>
        /// <param name="args">The arguments.</param>
        /// <returns>The invocation result.</returns>
        public virtual object? Invoke(object? instance, IEnumerable<object?> args)
        {
            // TODO localization
            throw new NotSupportedException();
        }
    }
}