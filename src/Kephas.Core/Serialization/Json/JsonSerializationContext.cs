// --------------------------------------------------------------------------------------------------------------------
// <copyright file="JsonSerializationContext.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Implements the JSON serialization context class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Serialization.Json
{
    /// <summary>
    /// A JSON serialization context.
    /// </summary>
    public class JsonSerializationContext : SerializationContext<JsonFormat>
    {
    }

    /// <summary>
    /// A JSON serialization context.
    /// </summary>
    /// <typeparam name="TRootObject">Type of the root object.</typeparam>
    public class JsonSerializationContext<TRootObject> : JsonSerializationContext
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="JsonSerializationContext{TRootObject} "/> class.
        /// </summary>
        public JsonSerializationContext()
        {
            this.RootObjectType = typeof(TRootObject);
        }
    }
}