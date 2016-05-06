// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Property.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
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
            get { return this.propertyType ?? (this.propertyType = this.ComputePropertyType()); }
            protected internal set { this.propertyType = value; }
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
        /// Calculates the property type.
        /// </summary>
        /// <exception cref="ModelException">Thrown when the property has no parts which can be used to get the classifier.</exception>
        /// <returns>
        /// The calculated property type.
        /// </returns>
        protected virtual ITypeInfo ComputePropertyType()
        {
            var firstPart = ((IAggregatedElementInfo)this).Parts.OfType<IPropertyInfo>().FirstOrDefault();
            if (firstPart == null)
            {
                throw new ModelException(string.Format(Strings.Property_MissingPartsToComputePropertyType_Exception, this.Name, this.Container));
            }

            return this.ModelSpace.TryGetClassifier(firstPart.PropertyType) ?? firstPart.PropertyType;
        }
    }
}