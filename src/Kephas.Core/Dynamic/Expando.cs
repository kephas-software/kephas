// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Expando.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
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
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Diagnostics.Contracts;

    using Kephas.Diagnostics.Contracts;

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
    public class Expando : ExpandoBase
    {
        /// <summary>
        /// The properties.
        /// </summary>
        private IDictionary<string, object> properties;

        /// <summary>
        /// Initializes a new instance of the <see cref="Expando"/> class. This constructor just works
        /// off the internal dictionary and any public properties of this object.
        /// </summary>
        /// <param name="isThreadSafe"><c>true</c> if this object is thread safe when working with the internal dictionary, <c>false</c> otherwise.</param>
        public Expando(bool isThreadSafe = false)
        {
            this.InitializeDictionary(isThreadSafe);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Expando"/> class. Allows passing in an existing
        /// instance variable to 'extend'.
        /// </summary>
        /// <remarks>
        /// You can pass in null here if you don't want to check native properties and only check the
        /// Dictionary!.
        /// </remarks>
        /// <param name="instance">The instance which sould be extended.</param>
        /// <param name="isThreadSafe"><c>true</c> if this object is thread safe when working with the internal dictionary, <c>false</c> otherwise.</param>
        public Expando(object instance, bool isThreadSafe = false)
            : base(instance)
        {
            Requires.NotNull(instance, nameof(instance));

            this.InitializeDictionary(isThreadSafe);
        }

        /// <summary>
        /// Converts this expando object to a dictionary.
        /// </summary>
        /// <returns>
        /// This object as an <see cref="IDictionary{TKey,TValue}"/>.
        /// </returns>
        public IDictionary<string, object> ToDictionary()
        {
            return new Dictionary<string, object>(this.properties);
        }

        /// <summary>
        /// Attempts to get dictionary value from the given data.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="value">The value to get from the dictionary.</param>
        /// <returns>
        /// <c>true</c> if a value is found, <c>false</c> otherwise.
        /// </returns>
        protected override bool TryGetDictionaryValue(string key, out object value)
        {
            return this.properties.TryGetValue(key, out value);
        }

        /// <summary>
        /// Attempts to set the gived data in the dictionary.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="value">The value to set into the dictionary.</param>
        /// <param name="addIfNonExisting"><c>true</c> to add the item if it is not existing.</param>
        /// <returns>
        /// <c>true</c> if the value could be set, <c>false</c> otherwise.
        /// </returns>
        protected override bool TrySetDictionaryValue(string key, object value, bool addIfNonExisting)
        {
            if (!addIfNonExisting && !this.properties.ContainsKey(key))
            {
                return false;
            }

            this.properties[key] = value;
            return true;
        }

        /// <summary>
        /// Gets the dictionary entries.
        /// </summary>
        /// <returns>
        /// An enumerator that allows foreach to be used to process the dictionary entries in this
        /// collection.
        /// </returns>
        protected override IEnumerable<KeyValuePair<string, object>> GetDictionaryEntries()
        {
            return this.properties;
        }

        /// <summary>
        /// Initializes the dictionary.
        /// </summary>
        /// <param name="isThreadSafe"><c>true</c> if this object is thread safe when working with the internal dictionary, <c>false</c> otherwise.</param>
        private void InitializeDictionary(bool isThreadSafe)
        {
            this.properties = isThreadSafe
                                  ? (IDictionary<string, object>)new ConcurrentDictionary<string, object>()
                                  : new Dictionary<string, object>();
        }
    }
}
