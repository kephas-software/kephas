// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Transition.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Workflow.Model.Elements
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Kephas.Model;
    using Kephas.Model.Construction;
    using Kephas.Model.Elements;
    using Kephas.Reflection;

    /// <summary>
    /// Definition class for transitions.
    /// </summary>
    public class Transition : ModelElementBase<ITransition>, ITransition
    {
        private ITypeInfo? returnType;

        /// <summary>
        /// Initializes a new instance of the <see cref="Transition"/> class.
        /// </summary>
        /// <param name="constructionContext">Context for the construction.</param>
        /// <param name="name">The name.</param>
        public Transition(IModelConstructionContext constructionContext, string name)
            : base(constructionContext, name)
        {
        }

        /// <summary>
        /// Gets the return type of the method.
        /// </summary>
        /// <value>
        /// The return type of the method.
        /// </value>
        public ITypeInfo? ReturnType
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
        public IEnumerable<IParameterInfo> Parameters => throw new NotImplementedException();

        /// <summary>
        /// Gets or sets the states from which the transitions starts.
        /// </summary>
        /// <value>
        /// An enumeration of states.
        /// </value>
        public IEnumerable<object> From { get; protected internal set; }

        /// <summary>
        /// Gets or sets the state to which the state machine is transitioned.
        /// </summary>
        /// <value>
        /// An enumeration of states.
        /// </value>
        public object To { get; protected internal set; }

        /// <summary>
        /// Gets the element annotations.
        /// </summary>
        /// <value>
        /// The element annotations.
        /// </value>
        IEnumerable<object> IElementInfo.Annotations => this.Annotations;

        /// <summary>
        /// Invokes the specified method on the provided instance.
        /// </summary>
        /// <param name="instance">The instance.</param>
        /// <param name="args">The arguments.</param>
        /// <returns>The invocation result.</returns>
        public object? Invoke(object? instance, IEnumerable<object?> args)
        {
            // TODO Check implementation
            throw new NotSupportedException();
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
            var firstPart = this.TryGetFirstPart<IParameterInfo>();
            if (firstPart == null)
            {
                // TODO localization
                throw new ModelException("Missing parts to compute property type.");
            }

            return this.ModelSpace.TryGetClassifier(firstPart.ValueType) ?? firstPart.ValueType;
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