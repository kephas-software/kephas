// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RefProperty.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Data.Model.Elements
{
    using Kephas.Data.Reflection;
    using Kephas.Model;
    using Kephas.Model.Construction;
    using Kephas.Model.Elements;
    using Kephas.Model.Resources;
    using Kephas.Reflection;

    /// <summary>
    /// Definition class for reference properties.
    /// </summary>
    public class RefProperty : Property, IRefPropertyInfo
    {
        private ITypeInfo? refType;

        /// <summary>
        /// Initializes a new instance of the <see cref="RefProperty"/> class.
        /// </summary>
        /// <param name="constructionContext">Context for the construction.</param>
        /// <param name="name">The name.</param>
        public RefProperty(IModelConstructionContext constructionContext, string name)
            : base(constructionContext, name)
        {
        }

        /// <summary>
        /// Gets or sets the reference type.
        /// </summary>
        public ITypeInfo RefType
        {
            get => this.refType ??= this.ComputeRefType();
            protected internal set => this.refType = value;
        }

        /// <summary>
        /// Calculates the reference type.
        /// </summary>
        /// <exception cref="ModelException">Thrown when the property has no parts which can be used to get the classifier.</exception>
        /// <returns>
        /// The calculated reference type.
        /// </returns>
        protected virtual ITypeInfo ComputeRefType()
        {
            var firstPart = this.TryGetFirstPart<IRefPropertyInfo>();
            if (firstPart == null)
            {
                throw new ModelException(string.Format(Strings.Property_MissingPartsToComputeRefType_Exception, this.Name, this.DeclaringContainer));
            }

            return this.ModelSpace.TryGetClassifier(firstPart.RefType) ?? firstPart.ValueType;
        }
    }
}