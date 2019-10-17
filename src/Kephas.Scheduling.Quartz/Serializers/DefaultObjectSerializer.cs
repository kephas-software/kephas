// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DefaultObjectSerializer.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the default object serializer class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Quartz.Simpl
{
    using System.IO;
    using System.Runtime.Serialization.Formatters.Binary;

    using Quartz.Spi;

    /// <summary>
    /// Default object serialization strategy that uses <see cref="BinaryFormatter" /> 
    /// under the hood.
    /// </summary>
    public class DefaultObjectSerializer : IObjectSerializer
    {
        /// <summary>
        /// Initializes this object.
        /// </summary>
        public void Initialize()
        {
        }

        /// <summary>
        /// Serializes given object as bytes that can be stored to permanent stores.
        /// </summary>
        /// <typeparam name="T">The object type.</typeparam>
        /// <param name="obj">Object to serialize.</param>
        /// <returns>
        /// A byte[].
        /// </returns>
        public byte[] Serialize<T>(T obj)
            where T : class
        {
            using (MemoryStream ms = new MemoryStream())
            {
                var bf = new BinaryFormatter();
                bf.Serialize(ms, obj);
                return ms.ToArray();
            }
        }

        /// <summary>
        /// Deserializes object from byte array presentation.
        /// </summary>
        /// <typeparam name="T">Generic type parameter.</typeparam>
        /// <param name="data">Data to deserialize object from.</param>
        /// <returns>
        /// A deserialized object.
        /// </returns>
        public T DeSerialize<T>(byte[] data)
            where T : class
        {
            using (MemoryStream ms = new MemoryStream(data))
            {
                var bf = new BinaryFormatter();
                return (T)bf.Deserialize(ms);
            }
        }
    }
}
