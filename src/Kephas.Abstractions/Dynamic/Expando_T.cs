// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Expando_T.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Class that provides extensible properties and methods. This
//   dynamic object stores 'extra' properties in a dictionary or
//   checks the actual properties of the instance.
//   This means you can subclass this expando and retrieve either
//   native properties or properties from values in the dictionary.
//   This type allows you three ways to access its properties:
//   Directly: any explicitly declared properties are accessible
//   Dynamic: dynamic cast allows access to dictionary and native properties/methods
//   Dictionary: Any of the extended properties are accessible via IDictionary interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Dynamic
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;

    /// <summary>
    /// <para>
    /// Class that provides extensible properties and methods. This
    /// dynamic object stores 'extra' properties in a dictionary or
    /// checks the actual properties of the instance.
    /// This means you can subclass this expando and retrieve either
    /// native properties or properties from values in the dictionary.
    /// </para>
    /// <para>
    /// This type allows you three ways to access its properties:
    /// <list type="bullet">
    /// <item>
    /// <term>Directly</term>
    /// <description>any explicitly declared properties are accessible</description>
    /// </item>
    /// <item>
    /// <term>Dynamic</term>
    /// <description>dynamic cast allows access to dictionary and native properties/methods</description>
    /// </item>
    /// <item>
    /// <term>Dictionary</term>
    /// <description>Any of the extended properties are accessible via IDictionary interface</description>
    /// </item>
    /// </list>
    /// </para>
    /// </summary>
    /// <typeparam name="T">The inner dictionary item type.</typeparam>
    public class Expando<T> : ExpandoBase<T>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Expando{T}"/> class. This constructor just works
        /// off the internal dictionary and any public properties of this object.
        /// </summary>
        public Expando()
            : this(isThreadSafe: false)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Expando{T}"/> class. This constructor just works
        /// off the internal dictionary and any public properties of this object.
        /// </summary>
        /// <param name="isThreadSafe"><c>true</c> if this object is thread safe when working with the internal dictionary, <c>false</c> otherwise.</param>
        public Expando(bool isThreadSafe)
            : base(GetDictionary(null, isThreadSafe))
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Expando{T}"/> class. This constructor just works
        /// off the internal dictionary and any public properties of this object.
        /// </summary>
        /// <param name="dictionary">The properties.</param>
        protected internal Expando(IDictionary<string, T> dictionary)
            : base(dictionary)
        {
            dictionary = dictionary ?? throw new ArgumentNullException(nameof(dictionary));
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Expando{T}"/> class. Allows passing in an existing
        /// instance variable to 'extend'.
        /// </summary>
        /// <remarks>
        /// You can pass in null here if you don't want to check native properties and only check the
        /// Dictionary!.
        /// </remarks>
        /// <param name="innerObject">The instance to be extended.</param>
        /// <param name="isThreadSafe"><c>true</c> if this object is thread safe when working with the internal dictionary, <c>false</c> otherwise.</param>
        protected internal Expando(object innerObject, bool isThreadSafe = false)
            : base(innerObject, GetDictionary(innerObject, isThreadSafe))
        {
            innerObject = innerObject ?? throw new ArgumentNullException(nameof(innerObject));
        }

        /// <summary>
        /// Gets a dictionary based on the <paramref name="isThreadSafe"/> flag.
        /// </summary>
        /// <param name="innerObject">The instance to be extended.</param>
        /// <param name="isThreadSafe"><c>true</c> if the internal dictionary should be thread safe,
        ///                            <c>false</c> otherwise.</param>
        /// <returns>
        /// The dictionary.
        /// </returns>
        protected internal static IDictionary<string, T> GetDictionary(object? innerObject, bool isThreadSafe)
        {
            if (innerObject is IDictionary<string, T> innerDictionary)
            {
                return innerDictionary;
            }

            return isThreadSafe
                       ? new ConcurrentDictionary<string, T>()
                       : new Dictionary<string, T>();
        }
    }
}
