// --------------------------------------------------------------------------------------------------------------------
// <copyright file="XmlSerializationContext.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Implements the XML serialization context class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Serialization.Xml
{
    /// <summary>
    /// An XML serialization context.
    /// </summary>
    public class XmlSerializationContext : SerializationContext<XmlFormat>
    {
    }

    /// <summary>
    /// An XML serialization context.
    /// </summary>
    /// <typeparam name="TRootObject">Type of the root object.</typeparam>
    public class XmlSerializationContext<TRootObject> : XmlSerializationContext
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="XmlSerializationContext{TRootObject} "/> class.
        /// </summary>
        public XmlSerializationContext()
        {
            this.RootObjectType = typeof(TRootObject);
        }
    }
}