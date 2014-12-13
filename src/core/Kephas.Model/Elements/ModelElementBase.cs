// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ModelElementBase.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Base abstract class for model elements.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Model.Elements
{
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// Base abstract class for model elements.
    /// </summary>
    /// <typeparam name="TModelContract">The type of the model contract.</typeparam>
    public abstract class ModelElementBase<TModelContract> : NamedElementBase<TModelContract>, IModelElement
    {
        /// <summary>
        /// The members.
        /// </summary>
        private readonly IDictionary<string, INamedElement> members = new Dictionary<string, INamedElement>();

        /// <summary>
        /// Initializes a new instance of the <see cref="ModelElementBase{TModelContract}"/> class.
        /// </summary>
        /// <param name="modelSpace">The model space.</param>
        /// <param name="name">The name.</param>
        protected ModelElementBase(IModelSpace modelSpace, string name)
            : base(modelSpace, name)
        {
        }

        /// <summary>
        /// Gets the members of this model element.
        /// </summary>
        /// <value>
        /// The model element members.
        /// </value>
        public IEnumerable<INamedElement> Members
        {
            get
            {
                return this.members.Values;
            }
        }

        /// <summary>
        /// Gets the attributes of this model element.
        /// </summary>
        /// <value>
        /// The model element attributes.
        /// </value>
        public IEnumerable<IModelAttribute> Attributes
        {
            get { return this.members.OfType<IModelAttribute>(); }
        }

        /// <summary>
        /// Gets the base model element.
        /// </summary>
        /// <value>
        /// The base model element.
        /// </value>
        public IModelElement Base { get; internal set; }

        /// <summary>
        /// Gets the member with the specified qualified name.
        /// </summary>
        /// <param name="qualifiedName">The qualified name of the member.</param>
        /// <param name="throwOnNotFound">If set to <c>true</c> and the member is not found, an exception occurs; otherwise <c>null</c> is returned if the member is not found.</param>
        /// <returns>
        /// The member with the provided qualified name or <c>null</c>.
        /// </returns>
        public INamedElement GetMember(string qualifiedName, bool throwOnNotFound = true)
        {
            INamedElement element;
            if (this.members.TryGetValue(qualifiedName, out element))
            {
                return element;
            }

            if (throwOnNotFound)
            {
                throw new ElementNotFoundException(qualifiedName, this.Name);
            }

            return null;
        }
    }
}