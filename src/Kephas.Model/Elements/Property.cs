// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Property.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Definition class for properties.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Model.Elements
{
    using System.Linq;

    using Kephas.Model.Construction;
    using Kephas.Model.Resources;
    using Kephas.Reflection;
    using Kephas.Runtime;

    /// <summary>
    /// Definition class for properties.
    /// </summary>
    public class Property : ModelElementBase<IProperty>, IProperty
    {
        /// <summary>
        /// Type of the property.
        /// </summary>
        private ITypeInfo propertyType;

        /// <summary>
        /// The runtime property info.
        /// </summary>
        private IRuntimePropertyInfo runtimePropertyInfo;

        /// <summary>
        /// Initializes a new instance of the <see cref="Property"/> class.
        /// </summary>
        /// <param name="constructionContext">Context for the construction.</param>
        /// <param name="name">The name.</param>
        public Property(IModelConstructionContext constructionContext, string name)
            : base(constructionContext, name)
        {
        }

        /// <summary>
        /// Gets or sets the type of the property.
        /// </summary>
        /// <value>
        /// The type of the property.
        /// </value>
        public ITypeInfo PropertyType
        {
            get => this.propertyType ?? (this.propertyType = this.ComputePropertyType());
            protected internal set => this.propertyType = value;
        } 

        /// <summary>
        /// Gets or sets a value indicating whether the property can be written to.
        /// </summary>
        /// <value>
        /// <c>true</c> if the property can be written to; otherwise, <c>false</c>.
        /// </value>
        public virtual bool CanWrite { get; protected internal set; }

        /// <summary>
        /// Gets or sets a value indicating whether the property value can be read.
        /// </summary>
        /// <value>
        /// <c>true</c> if the property value can be read; otherwise <c>false</c>.
        /// </value>
        public virtual bool CanRead { get; protected internal set; }

        /// <summary>
        /// Sets the specified value.
        /// </summary>
        /// <param name="obj">The object.</param>
        /// <param name="value">The value.</param>
        public void SetValue(object obj, object value)
        {
            var runtimeProperty = this.TryGetRuntimePropertyInfo();
            if (runtimeProperty == null)
            {
                obj.SetPropertyValue(this.Name, value);
            }
            else
            {
                runtimeProperty.SetValue(obj, value);
            }
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
            var runtimeProperty = this.TryGetRuntimePropertyInfo();
            if (runtimeProperty == null)
            {
                return obj.GetPropertyValue(this.Name);
            }
            else
            {
                return runtimeProperty.GetValue(obj);
            }
        }

        /// <summary>
        /// Tries to get the runtime property information for this property.
        /// </summary>
        /// <returns>
        /// A <see cref="IRuntimePropertyInfo"/> or <c>null</c>.
        /// </returns>
        protected virtual IRuntimePropertyInfo TryGetRuntimePropertyInfo()
        {
            // TODO improve implementation.
            if (this.runtimePropertyInfo != null)
            {
                return this.runtimePropertyInfo;
            }

            this.runtimePropertyInfo = this.TryGetFirstPart<IRuntimePropertyInfo>();
            return this.runtimePropertyInfo;
        }

        /// <summary>
        /// Calculates the property type.
        /// </summary>
        /// <exception cref="ModelException">Thrown when the property has no parts which can be used to get the classifier.</exception>
        /// <returns>
        /// The calculated property type.
        /// </returns>
        protected virtual ITypeInfo ComputePropertyType()
        {
            var firstPart = this.TryGetFirstPart<IPropertyInfo>();
            if (firstPart == null)
            {
                throw new ModelException(string.Format(Strings.Property_MissingPartsToComputePropertyType_Exception, this.Name, this.DeclaringContainer));
            }

            return this.ModelSpace.TryGetClassifier(firstPart.PropertyType) ?? firstPart.PropertyType;
        }

        /// <summary>
        /// Called when the construction is complete.
        /// </summary>
        /// <param name="constructionContext">Context for the construction.</param>
        protected override void OnCompleteConstruction(IModelConstructionContext constructionContext)
        {
            // TODO add members from multiple parts
            // TODO validate the property types of multiple parts
            base.OnCompleteConstruction(constructionContext);
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