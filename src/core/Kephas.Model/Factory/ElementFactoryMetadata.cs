using System;
using System.Collections.Generic;
using Kephas.Reflection;
using Kephas.Services;

namespace Kephas.Model.Factory
{
    /// <summary>
    /// Metadata for element factories.
    /// </summary>
    public class ElementFactoryMetadata : AppServiceMetadata
    {
        /// <summary>
        /// The element type metadata key.
        /// </summary>
        public static readonly string ElementTypeKey = ReflectionHelper.GetPropertyName<ElementFactoryMetadata>(m => m.ElementType);

        /// <summary>
        /// Initializes a new instance of the <see cref="ElementFactoryMetadata"/> class.
        /// </summary>
        /// <param name="metadata">The metadata.</param>
        public ElementFactoryMetadata(IDictionary<string, object> metadata) 
            : base(metadata)
        {
            object value;
            if (metadata.TryGetValue(ElementTypeKey, out value))
            {
                this.ElementType = (Type)value;
            }
        }

        /// <summary>
        /// Gets the type of the element.
        /// </summary>
        /// <value>
        /// The type of the element.
        /// </value>
        public Type ElementType { get; private set; }
    }
}