// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Method.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the method class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

#nullable enable

namespace Kephas.Model.Elements
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Kephas.Model.Construction;
    using Kephas.Model.Resources;
    using Kephas.Reflection;
    using Kephas.Runtime;

    /// <summary>
    /// Definition class for methods.
    /// </summary>
    public class Method : ModelElementBase<IMethod>, IMethod
    {
        /// <summary>
        /// Type of the return value.
        /// </summary>
        private ITypeInfo? returnType;

        /// <summary>
        /// The runtime method info.
        /// </summary>
        private IRuntimeMethodInfo? runtimeMethodInfo;

        /// <summary>
        /// Initializes a new instance of the <see cref="Method"/> class.
        /// </summary>
        /// <param name="constructionContext">Context for the construction.</param>
        /// <param name="name">The name.</param>
        public Method(IModelConstructionContext constructionContext, string name)
            : base(constructionContext, name)
        {
        }

        /// <summary>
        /// Gets or sets the return type of the method.
        /// </summary>
        /// <value>
        /// The return type of the method.
        /// </value>
        public ITypeInfo ReturnType
        {
            get => this.returnType ??= this.ComputeReturnType();
            protected internal set => this.returnType = value;
        }

        /// <summary>
        /// Gets the method parameters.
        /// </summary>
        /// <value>
        /// The method parameters.
        /// </value>
        public IEnumerable<IParameterInfo> Parameters => this.Members.OfType<IParameter>();

        /// <summary>
        /// Invokes the specified method on the provided instance.
        /// </summary>
        /// <param name="instance">The instance.</param>
        /// <param name="args">The arguments.</param>
        /// <returns>The invocation result.</returns>
        public object? Invoke(object? instance, IEnumerable<object?> args)
        {
            var runtimeMethod = this.TryGetRuntimeMethodInfo();
            if (runtimeMethod == null)
            {
                // TODO improve
                throw new NotSupportedException();
            }

            return runtimeMethod.Invoke(instance, args);
        }

        /// <summary>
        /// Calculates the property type.
        /// </summary>
        /// <exception cref="ModelException">Thrown when the property has no parts which can be used to get the classifier.</exception>
        /// <returns>
        /// The calculated property type.
        /// </returns>
        protected virtual ITypeInfo ComputeReturnType()
        {
            var firstPart = this.TryGetFirstPart<IOperationInfo>();
            if (firstPart == null)
            {
                throw new ModelException(string.Format(Strings.Property_MissingPartsToComputePropertyType_Exception, this.Name, this.DeclaringContainer));
            }

            return this.ModelSpace.TryGetClassifier(firstPart.ReturnType) ?? firstPart.ReturnType;
        }

        /// <summary>
        /// Tries to get the runtime method information for this method.
        /// </summary>
        /// <returns>
        /// A <see cref="IRuntimeMethodInfo"/> or <c>null</c>.
        /// </returns>
        protected virtual IRuntimeMethodInfo TryGetRuntimeMethodInfo()
        {
            // TODO improve implementation.
            if (this.runtimeMethodInfo != null)
            {
                return this.runtimeMethodInfo;
            }

            this.runtimeMethodInfo = this.TryGetFirstPart<IRuntimeMethodInfo>();
            return this.runtimeMethodInfo;
        }

        /// <summary>
        /// Tries to get a part of the provided generic type.
        /// </summary>
        /// <typeparam name="T">The part type.</typeparam>
        /// <returns>
        /// The part of the provided type.
        /// </returns>
        private T TryGetFirstPart<T>()
        {
            return this.Parts.OfType<T>().FirstOrDefault();
        }
    }
}