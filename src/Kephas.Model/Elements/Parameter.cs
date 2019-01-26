// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Parameter.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the parameter class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Model.Elements
{
    using System;
    using System.Linq;

    using Kephas.Model.Construction;
    using Kephas.Model.Resources;
    using Kephas.Reflection;

    /// <summary>
    /// Definition class for parameters.
    /// </summary>
    public class Parameter : ModelElementBase<IParameter>, IParameter
    {
        /// <summary>
        /// Type of the property.
        /// </summary>
        private ITypeInfo valueType;

        /// <summary>
        /// Initializes a new instance of the <see cref="Parameter"/> class.
        /// </summary>
        /// <param name="constructionContext">Context for the construction.</param>
        /// <param name="name">The name.</param>
        public Parameter(IModelConstructionContext constructionContext, string name)
            : base(constructionContext, name)
        {
        }

        /// <summary>
        /// Gets or sets the type of the element's value.
        /// </summary>
        /// <value>
        /// The type of the element's value.
        /// </value>
        public ITypeInfo ValueType
        {
            get => this.valueType ?? (this.valueType = this.ComputeValueType());
            protected internal set => this.valueType = value;
        }

        /// <summary>
        /// Gets or sets the position in the parameter's list.
        /// </summary>
        /// <value>
        /// The position in the parameter's list.
        /// </value>
        public int Position { get; protected internal set; }

        /// <summary>
        /// Gets or sets a value indicating whether this parameter is optional.
        /// </summary>
        /// <value>
        /// <c>true</c> if the parameter is optional, <c>false</c> otherwise.
        /// </value>
        public bool IsOptional { get; protected internal set; }

        /// <summary>
        /// Gets or sets a value indicating whether the parameter is for input.
        /// </summary>
        /// <value>
        /// True if this parameter is for input, false if not.
        /// </value>
        public bool IsIn { get; protected internal set; }

        /// <summary>
        /// Gets or sets a value indicating whether the parameter is for output.
        /// </summary>
        /// <value>
        /// True if this parameter is for output, false if not.
        /// </value>
        public bool IsOut { get; protected internal set; }

        /// <summary>
        /// Sets the specified value.
        /// </summary>
        /// <param name="obj">The object.</param>
        /// <param name="value">The value.</param>
        public void SetValue(object obj, object value)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// Gets the value from the specified object.
        /// </summary>
        /// <param name="obj">The object.</param>
        /// <returns>
        /// The value.
        /// </returns>
        public object GetValue(object obj)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// Calculates the property type.
        /// </summary>
        /// <exception cref="ModelException">Thrown when the property has no parts which can be used to get the classifier.</exception>
        /// <returns>
        /// The calculated property type.
        /// </returns>
        protected virtual ITypeInfo ComputeValueType()
        {
            var firstPart = this.TryGetFirstPart<IParameterInfo>();
            if (firstPart == null)
            {
                throw new ModelException(string.Format(Strings.Property_MissingPartsToComputePropertyType_Exception, this.Name, this.DeclaringContainer));
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