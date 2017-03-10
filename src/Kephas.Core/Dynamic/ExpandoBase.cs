// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ExpandoBase.cs" company="Quartz Software SRL">
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
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.Contracts;
    using System.Dynamic;

    using Kephas.Diagnostics.Contracts;
    using Kephas.Reflection;
    using Kephas.Runtime;

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
    public abstract class ExpandoBase : DynamicObject, IExpando
    {
        /// <summary>
        /// Value of object passed in.
        /// </summary>
        private object wrappedInstance;

        /// <summary>
        /// Cached dynamic type of the instance.
        /// </summary>
        /// <remarks>
        /// Do not use directly this field, instead use the <see cref="GetRuntimeTypeInfo"/> method
        /// which knows how to late-initialize it.
        /// </remarks>
        private IRuntimeTypeInfo dynamicTypeInfo;

        /// <summary>
        /// Initializes a new instance of the <see cref="ExpandoBase"/> class.
        /// This constructor just works off the internal dictionary and any 
        /// public properties of this object.
        /// </summary>
        protected ExpandoBase()
        {
            // ReSharper disable once DoNotCallOverridableMethodsInConstructor
            this.InitializeExpando(null);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ExpandoBase"/> class.
        /// Allows passing in an existing instance variable to 'extend'.
        /// </summary>
        /// <param name="instance">
        /// The instance which sould be extended.
        /// </param>
        /// <remarks>
        /// You can pass in null here if you don't want to
        /// check native properties and only check the Dictionary!.
        /// </remarks>
        protected ExpandoBase(object instance)
        {
            Requires.NotNull(instance, nameof(instance));

            // ReSharper disable once DoNotCallOverridableMethodsInConstructor
            this.InitializeExpando(instance);
        }

        /// <summary>
        /// Convenience method that provides a string Indexer to the Properties collection AND the
        /// strongly typed properties of the object by name. // dynamic exp["Address"] = "112 nowhere
        /// lane";
        /// // strong var name = exp["StronglyTypedProperty"] as string;.
        /// </summary>
        /// <remarks>
        /// The getter checks the Properties dictionary first then looks in PropertyInfo for properties.
        /// The setter checks the instance properties before checking the Properties dictionary.
        /// </remarks>
        /// <param name="key">The key.</param>
        /// <returns>The <see cref="object" />.</returns>
        public object this[string key]
        {
            get
            {
                if (this.TryGetDictionaryValue(key, out object value))
                {
                    return value;
                }

                // try reflection on instanceType
                var instance = this.wrappedInstance ?? this;
                return this.GetRuntimeTypeInfo().TryGetValue(instance, key);
            }

            set
            {
                if (this.TrySetDictionaryValue(key, value, addIfNonExisting: false))
                {
                    return;
                }

                // check instance for existance of type first
                var instance = this.wrappedInstance ?? this;
                if (!this.GetRuntimeTypeInfo().TrySetValue(instance, key, value))
                {
                    this.TrySetDictionaryValue(key, value, addIfNonExisting: true);
                }
            }
        }

        /// <summary>
        /// Try to retrieve a member by name first from instance properties
        /// followed by the collection entries.
        /// </summary>
        /// <param name="binder">Provides information about the object that called the dynamic operation. The binder.Name property provides the name of the member on which the dynamic operation is performed. For example, for the Console.WriteLine(sampleObject.SampleProperty) statement, where sampleObject is an instance of the class derived from the <see cref="T:System.Dynamic.DynamicObject" /> class, binder.Name returns "SampleProperty". The binder.IgnoreCase property specifies whether the member name is case-sensitive.</param>
        /// <param name="result">The result of the get operation. For example, if the method is called for a property, you can assign the property value to <paramref name="result" />.</param>
        /// <returns>
        /// The <see cref="bool" />.
        /// </returns>
        public override bool TryGetMember(GetMemberBinder binder, out object result)
        {
            // first check the Properties collection for member
            if (this.TryGetDictionaryValue(binder.Name, out result))
            {
                return true;
            }

            // Next check for Public properties via Reflection
            var instance = this.wrappedInstance ?? this;
            result = this.GetRuntimeTypeInfo().TryGetValue(instance, binder.Name);

            return result != Undefined.Value;
        }

        /// <summary>
        /// Property setter implementation tries to retrieve value from instance
        /// first then into this object.
        /// </summary>
        /// <param name="binder">Provides information about the object that called the dynamic operation. The binder.Name property provides the name of the member to which the value is being assigned. For example, for the statement sampleObject.SampleProperty = "Test", where sampleObject is an instance of the class derived from the <see cref="T:System.Dynamic.DynamicObject" /> class, binder.Name returns "SampleProperty". The binder.IgnoreCase property specifies whether the member name is case-sensitive.</param>
        /// <param name="value">The value to set to the member. For example, for sampleObject.SampleProperty = "Test", where sampleObject is an instance of the class derived from the <see cref="T:System.Dynamic.DynamicObject" /> class, the <paramref name="value" /> is "Test".</param>
        /// <returns>
        /// The <see cref="bool" />.
        /// </returns>
        public override bool TrySetMember(SetMemberBinder binder, object value)
        {
            // first check to see if there's a native property to set
            var instance = this.wrappedInstance ?? this;
            if (this.GetRuntimeTypeInfo().TrySetValue(instance, binder.Name, value))
            {
                return true;
            }

            // no match - set or add to dictionary
            return this.TrySetDictionaryValue(binder.Name, value, addIfNonExisting: true);
        }

        /// <summary>
        /// Dynamic invocation method. Currently allows only for Reflection based
        /// operation (no ability to add methods dynamically).
        /// </summary>
        /// <param name="binder">Provides information about the dynamic operation. The binder.Name property provides the name of the member on which the dynamic operation is performed. For example, for the statement sampleObject.SampleMethod(100), where sampleObject is an instance of the class derived from the <see cref="T:System.Dynamic.DynamicObject" /> class, binder.Name returns "SampleMethod". The binder.IgnoreCase property specifies whether the member name is case-sensitive.</param>
        /// <param name="args">The arguments that are passed to the object member during the invoke operation. For example, for the statement sampleObject.SampleMethod(100), where sampleObject is derived from the <see cref="T:System.Dynamic.DynamicObject" /> class, <paramref name="args" />[0] is equal to 100.</param>
        /// <param name="result">The result of the member invocation.</param>
        /// <returns>
        /// The <see cref="bool" />.
        /// </returns>
        public override bool TryInvokeMember(InvokeMemberBinder binder, object[] args, out object result)
        {
            if (this.TryGetDictionaryValue(binder.Name, out object method))
            {
                var delegateProperty = (Delegate)method;
                result = delegateProperty.DynamicInvoke(args);
                return true;
            }

            var instance = this.wrappedInstance ?? this;
            result = this.GetRuntimeTypeInfo().TryInvoke(instance, binder.Name, args);
            return result != Undefined.Value;
        }

        /// <summary>
        /// Gets the <see cref="IRuntimeTypeInfo"/> used by the expando in the dynamic behavior.
        /// </summary>
        /// <returns>
        /// The dynamic type.
        /// </returns>
        protected virtual IRuntimeTypeInfo GetRuntimeTypeInfo()
        {
            Contract.Ensures(Contract.Result<IRuntimeTypeInfo>() != null);

            return this.dynamicTypeInfo ?? (this.dynamicTypeInfo = (this.wrappedInstance ?? this).GetType().AsRuntimeTypeInfo());
        }

        /// <summary>
        /// Initializes the expando with the provided instance.
        /// </summary>
        /// <param name="instance">The instance.</param>
        protected virtual void InitializeExpando(object instance)
        {
            this.wrappedInstance = instance;
        }

        /// <summary>
        /// Returns and the properties of.
        /// </summary>
        /// <param name="includeInstanceProperties">If set to <c>true</c> the instance properties are
        ///                                         also returned.</param>
        /// <returns>
        /// An enumeration of property (name, value) pairs.
        /// </returns>
        protected IEnumerable<KeyValuePair<string, object>> GetProperties(bool includeInstanceProperties = false)
        {
            if (includeInstanceProperties)
            {
                foreach (var prop in this.GetRuntimeTypeInfo().Properties)
                {
                    yield return new KeyValuePair<string, object>(prop.Key, prop.Value.GetValue(this.wrappedInstance));
                }
            }

            foreach (var item in this.GetDictionaryEntries())
            {
                yield return item;
            }
        }

        /// <summary>
        /// Attempts to get the dictionary value from the given data.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="value">The value to get from the dictionary.</param>
        /// <returns>
        /// <c>true</c> if a value is found, <c>false</c> otherwise.
        /// </returns>
        protected abstract bool TryGetDictionaryValue(string key, out object value);

        /// <summary>
        /// Attempts to set the gived data in the dictionary.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="value">The value to set into the dictionary.</param>
        /// <param name="addIfNonExisting"><c>true</c> to add the item if it is not existing.</param>
        /// <returns>
        /// <c>true</c> if the value could be set, <c>false</c> otherwise.
        /// </returns>
        protected abstract bool TrySetDictionaryValue(string key, object value, bool addIfNonExisting);

        /// <summary>
        /// Gets the dictionary entries.
        /// </summary>
        /// <returns>
        /// An enumerator that allows foreach to be used to process the dictionary entries in this
        /// collection.
        /// </returns>
        protected abstract IEnumerable<KeyValuePair<string, object>> GetDictionaryEntries();
    }
}
