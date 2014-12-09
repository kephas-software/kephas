using System;
using System.Collections.Generic;

namespace Kephas.Model.Elements
{
    /// <summary>
    /// Base abstract class for model elements.
    /// </summary>
    public abstract class ModelElementBase : NamedElementBase, IModelElement
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ModelElementBase"/> class.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="projection">The projection.</param>
        protected ModelElementBase(string name, IModelProjection projection = null) 
            : base(name)
        {
            this.Projection = projection;
        }

        /// <summary>
        /// Gets the projection where the model element is defined.
        /// </summary>
        /// <value>
        /// The projection.
        /// </value>
        public IModelProjection Projection { get; private set; }

        /// <summary>
        /// Gets the members of this model element.
        /// </summary>
        /// <value>
        /// The model element members.
        /// </value>
        public IEnumerable<IModelElement> Members
        {
            get { throw new NotImplementedException(); }
        }

        /// <summary>
        /// Gets the attributes of this model element.
        /// </summary>
        /// <value>
        /// The model element attributes.
        /// </value>
        public IEnumerable<IAttribute> Attributes
        {
            get { throw new NotImplementedException(); }
        }
    }
}