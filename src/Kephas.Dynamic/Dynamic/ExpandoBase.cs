// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ExpandoBase.cs" company="Kephas Software SRL">
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
    using System.Collections;
    using System.Collections.Generic;
    using System.Dynamic;
    using System.Reflection;
    using System.Runtime.CompilerServices;
    using Kephas.Dynamic.Resources;

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
    public abstract class ExpandoBase<T> : DynamicObject, IExpando, IExpandoMixin, IDynamic
    {
        private IDictionary<string, T>? innerDictionary;
        private WeakReference<object>? innerObjectRef;
        private bool ignoreCase;

        /// <summary>
        /// Initializes a new instance of the <see cref="ExpandoBase{T}"/> class.
        /// This constructor just works off the internal dictionary.
        /// </summary>
        /// <param name="innerDictionary">
        /// Optional. The inner dictionary for holding dynamic values.
        /// If not provided, a new dictionary will be created.
        /// </param>
        protected ExpandoBase(IDictionary<string, T>? innerDictionary = null)
        {
            // ReSharper disable once DoNotCallOverridableMethodsInConstructor
            this.InitializeExpando(null, innerDictionary);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ExpandoBase{T}"/> class. Allows passing in an
        /// existing instance variable to 'extend'.
        /// </summary>
        /// <param name="innerObject">The instance to be extended.</param>
        /// <param name="innerDictionary">
        /// Optional. The inner dictionary for holding dynamic values.
        /// If not provided, a new dictionary will be created.
        /// </param>
        protected ExpandoBase(object innerObject, IDictionary<string, T>? innerDictionary = null)
        {
            // ReSharper disable once DoNotCallOverridableMethodsInConstructor
            this.InitializeExpando(innerObject ?? throw new ArgumentNullException(nameof(innerObject)), innerDictionary);
        }

        /// <summary>
        /// Gets the inner dictionary.
        /// </summary>
        IDictionary<string, object?> IExpandoMixin.InnerDictionary =>
            this.innerDictionary as IDictionary<string, object?> ?? new ObjectDictionaryAdapter(this.innerDictionary!);

        /// <summary>
        /// Gets a weak reference to the inner object.
        /// </summary>
        object? IExpandoMixin.InnerObject => this.TryGetInnerObject();

        /// <summary>
        /// Gets the binders to use when retrieving the expando members.
        /// </summary>
        ExpandoMemberBinderKind IExpandoMixin.MemberBinders => this.MemberBinders;

        /// <summary>
        /// Gets a value indicating whether to ignore the case when identifying the members by name.
        /// </summary>
        bool IExpandoMixin.IgnoreCase => this.ignoreCase;

        /// <summary>
        /// Gets the inner dictionary.
        /// </summary>
        protected IDictionary<string, T> InnerDictionary => this.innerDictionary!;

        /// <summary>
        /// Gets or sets the binders to use when retrieving the expando members.
        /// </summary>
        protected ExpandoMemberBinderKind MemberBinders { get; set; } = ExpandoMemberBinderKind.All;

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
        public object? this[string key]
        {
            get => this.TryGetValue(key, out var value) ? value : null;
            set => this.TrySetValue(key, value);
        }

        /// <summary>
        /// Returns the enumeration of all dynamic member names.
        /// </summary>
        /// <returns>
        /// A sequence that contains dynamic member names.
        /// </returns>
        public override IEnumerable<string> GetDynamicMemberNames()
        {
            var binders = this.MemberBinders;
            var hashSet = this.ignoreCase ? new HashSet<string>(StringComparer.OrdinalIgnoreCase) : new HashSet<string>();

            // First check for public properties via reflection in this type
            var innerObject = this.TryGetInnerObject();
            if (this != innerObject
                && binders.HasFlag(ExpandoMemberBinderKind.This))
            {
                var type = this.GetType();
                foreach (var property in DynamicHelper.GetTypeProperties(type))
                {
                    var propName = property.Name;
                    if (!hashSet.Contains(propName))
                    {
                        hashSet.Add(propName);
                        yield return propName;
                    }
                }
            }

            // then, check for the properties in the inner object
            if (innerObject != null
                && binders.HasFlag(ExpandoMemberBinderKind.InnerObject))
            {
                var type = innerObject.GetType();
                foreach (var property in DynamicHelper.GetTypeProperties(type!))
                {
                    var propName = property.Name;
                    if (!hashSet.Contains(propName))
                    {
                        hashSet.Add(propName);
                        yield return propName;
                    }
                }
            }

            // last, check the dictionary for members.
            if (binders.HasFlag(ExpandoMemberBinderKind.InnerDictionary))
            {
                foreach (var propName in this.innerDictionary!.Keys)
                {
                    if (!hashSet.Contains(propName))
                    {
                        hashSet.Add(propName);
                        yield return propName;
                    }
                }
            }
        }

        /// <summary>
        /// Indicates whether the <paramref name="memberName"/> is defined in the expando.
        /// </summary>
        /// <param name="memberName">Name of the member.</param>
        /// <returns>
        /// True if defined, false if not.
        /// </returns>
        public virtual bool HasDynamicMember(string memberName)
        {
            return IExpandoMixin.HasDynamicMember(this, memberName);
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
        public override bool TryGetMember(GetMemberBinder binder, out object? result)
        {
            // if the member is not found, return null.
            this.TryGetValue(binder.Name, out result);
            return true;
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
        public override bool TrySetMember(SetMemberBinder binder, object? value)
        {
            if (this.TrySetValue(binder.Name, value))
            {
                return true;
            }

            var innerObject = this.TryGetInnerObject();
            throw new MemberAccessException(
                string.Format(
                    Strings.ExpandoBase_TrySetMember_Exception,
                    binder.Name,
                    innerObject != null ? innerObject.GetType() : this.GetType()));
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
        public override bool TryInvokeMember(InvokeMemberBinder binder, object?[]? args, out object? result)
        {
            var binders = this.MemberBinders;

            bool TryInvokeDelegateProperty(object? delegateValue, out object? res)
            {
                if (delegateValue == null)
                {
                    throw new NullReferenceException($"The property '{binder.Name}' is a null reference/not set.");
                }

                if (delegateValue is not Delegate invokable)
                {
                    throw new MemberAccessException(string.Format(
                        Strings.ExpandoBase_CannotInvokeNonDelegate_Exception,
                        binder.Name,
                        delegateValue?.GetType()));
                }

                res = invokable.DynamicInvoke(args);
                return true;
            }

            bool TryInvokeTypeMember(Type type, object instance, bool recurseDynamic, out object? res)
            {
                var methodInfo = this.TryGetMethodInfo(type, binder.Name, this.ignoreCase);
                if (methodInfo != null)
                {
                    res = methodInfo.Call(instance, args);
                    return true;
                }

                var propertyInfo = this.TryGetPropertyInfo(type, binder.Name, this.ignoreCase);
                if (propertyInfo != null)
                {
                    var delegateValue = propertyInfo.GetValue(instance);
                    return TryInvokeDelegateProperty(delegateValue, out res);
                }

                if (recurseDynamic && instance is DynamicObject dynInstance)
                {
                    if (dynInstance.TryInvokeMember(binder, args, out res))
                    {
                        return true;
                    }
                }

                res = null;
                return false;
            }

            var innerObject = this.TryGetInnerObject();
            if (this != innerObject
                && binders.HasFlag(ExpandoMemberBinderKind.This)
                && TryInvokeTypeMember(this.GetType(), this, false, out result))
            {
                return true;
            }

            if (innerObject != null
                && binders.HasFlag(ExpandoMemberBinderKind.InnerObject)
                && TryInvokeTypeMember(innerObject.GetType()!, innerObject, true, out result))
            {
                return true;
            }

            if (binders.HasFlag(ExpandoMemberBinderKind.InnerDictionary)
                && this.innerDictionary!.TryGetValue(binder.Name, out var method))
            {
                return TryInvokeDelegateProperty(method, out result);
            }

            result = null;
            return false;
        }

        /// <summary>
        /// Converts the expando to a dictionary having as keys the property names and as values the
        /// respective properties' values.
        /// </summary>
        /// <param name="keyFunc">Optional. The key transformation function.</param>
        /// <param name="valueFunc">Optional. The value transformation function.</param>
        /// <returns>
        /// A dictionary of property values with their associated names.
        /// </returns>
        public virtual IDictionary<string, object?> ToDictionary(
            Func<string, string>? keyFunc = null,
            Func<object?, object?>? valueFunc = null)
                => IExpandoMixin.ToDictionary(this, keyFunc, valueFunc);

        /// <summary>
        /// Attempts to get the dynamic value with the given key.
        /// </summary>
        /// <remarks>
        /// First of all, it is tried to get a property value from the inner object, if one is set.
        /// The next try is to retrieve the property value from the expando object itself.
        /// Lastly, if still a property by the provided name cannot be found, the inner dictionary is searched by the provided key.
        /// </remarks>
        /// <param name="key">The key.</param>
        /// <param name="value">The value to get.</param>
        /// <returns>
        /// <c>true</c> if a value is found, <c>false</c> otherwise.
        /// </returns>
        protected virtual bool TryGetValue(string key, out object? value)
            => IExpandoMixin.TryGetValue(this, key, out value);

        /// <summary>
        /// Attempts to set the value with the given key.
        /// </summary>
        /// <remarks>
        /// First of all, it is tried to set the property value to the inner object, if one is set.
        /// The next try is to set the property value to the expando object itself.
        /// Lastly, if still a property by the provided name cannot be found, the inner dictionary is used to set the value with the provided key.
        /// </remarks>
        /// <param name="key">The key.</param>
        /// <param name="value">The value to set.</param>
        /// <returns>
        /// <c>true</c> if the value could be set, <c>false</c> otherwise.
        /// </returns>
        protected virtual bool TrySetValue(string key, object? value)
            => IExpandoMixin.TrySetValue(this, key, value);

        /// <summary>
        /// Tries to get the MethodInfo for the provided type and key.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <param name="key">The key, typically the property name.</param>
        /// <param name="ignoreCase">
        /// Optional. Indicates whether case insensitive matching should be used.
        /// If not set, the default value inferred from the inner dictionary is used.
        /// </param>
        /// <returns>A MethodInfo or <c>null</c>.</returns>
        protected virtual MethodInfo? TryGetMethodInfo(Type type, string key, bool ignoreCase)
            => type.GetMethod(key, GetBindingFlags(ignoreCase));

        /// <summary>
        /// Tries to get the PropertyInfo for the provided type and key.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <param name="key">The key, typically the property name.</param>
        /// <param name="ignoreCase">
        /// Indicates whether case insensitive matching should be used.
        /// If not set, the default value inferred from the inner dictionary is used.
        /// </param>
        /// <returns>A PropertyInfo or <c>null</c>.</returns>
        protected virtual PropertyInfo? TryGetPropertyInfo(Type type, string key, bool ignoreCase)
            => type.GetProperty(key, GetBindingFlags(ignoreCase));

        /// <summary>
        /// Tries to get the inner/adapted object.
        /// </summary>
        /// <returns>The inner object, or <c>null</c>.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private object? TryGetInnerObject()
        {
            if (this.innerObjectRef == null)
            {
                return null;
            }

            return this.innerObjectRef.TryGetTarget(out var innerObject) ? innerObject : null;
        }

        /// <summary>
        /// Gets the binding flags for retrieving type members.
        /// </summary>
        /// <param name="ignoreCase">
        /// Indicates whether case insensitive matching should be used.
        /// If not set, the default value inferred from the inner dictionary is used.
        /// </param>
        /// <returns>The binding flags.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static BindingFlags GetBindingFlags(bool ignoreCase)
        {
            return IExpandoMixin.GetBindingFlags(ignoreCase);
        }

        /// <summary>
        /// Initializes the expando with the provided instance.
        /// </summary>
        /// <param name="instance">The instance.</param>
        /// <param name="dictionary">The inner dictionary.</param>
        private void InitializeExpando(object? instance, IDictionary<string, T>? dictionary)
        {
            this.innerObjectRef = instance == null ? null : new WeakReference<object>(instance);
            this.innerDictionary = dictionary ?? new Dictionary<string, T>();
            if (this.innerDictionary is Dictionary<string, object?> dict)
            {
                var comparer = dict.Comparer;
                this.ignoreCase = Equals(comparer, StringComparer.OrdinalIgnoreCase)
                                         || Equals(comparer, StringComparer.CurrentCultureIgnoreCase)
                                         || Equals(comparer, StringComparer.InvariantCultureIgnoreCase);
            }
        }

        private class ObjectDictionaryAdapter : IDictionary<string, object?>
        {
            private readonly IDictionary<string, T> dictionary;

            public ObjectDictionaryAdapter(IDictionary<string, T> dictionary)
            {
                this.dictionary = dictionary;
            }

            public int Count => this.dictionary.Count;

            public bool IsReadOnly => this.dictionary.IsReadOnly;

            public ICollection<string> Keys => this.dictionary.Keys;

            public ICollection<object?> Values => new ObjectCollectionAdapter(this.dictionary.Values);

            public object? this[string key]
            {
                get => this.dictionary[key];
                set => this.dictionary[key] = (T)value!;
            }

            public IEnumerator<KeyValuePair<string, object?>> GetEnumerator()
            {
                foreach (var (key, value) in this.dictionary)
                {
                    yield return new KeyValuePair<string, object?>(key, value);
                }
            }

            IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();

            public void Add(KeyValuePair<string, object?> item) =>
                this.dictionary.Add(new KeyValuePair<string, T>(item.Key, (T)item.Value!));

            public void Clear() => this.dictionary.Clear();

            public bool Contains(KeyValuePair<string, object?> item)
                => this.dictionary.Contains(new KeyValuePair<string, T>(item.Key, (T)item.Value!));

            public void CopyTo(KeyValuePair<string, object?>[] array, int arrayIndex)
            {
                throw new NotSupportedException();
            }

            public bool Remove(KeyValuePair<string, object?> item) =>
                this.dictionary.Remove(new KeyValuePair<string, T>(item.Key, (T)item.Value!));

            public void Add(string key, object? value) => this.dictionary.Add(key, (T)value!);

            public bool ContainsKey(string key) => this.dictionary.ContainsKey(key);

            public bool Remove(string key) => this.dictionary.Remove(key);

            public bool TryGetValue(string key, out object? value)
            {
                var result = this.dictionary.TryGetValue(key, out var typedValue);
                value = typedValue;
                return result;
            }
        }

        private class ObjectCollectionAdapter : ICollection<object?>
        {
            private readonly ICollection<T> collection;

            public ObjectCollectionAdapter(ICollection<T> collection)
            {
                this.collection = collection;
            }

            public int Count => this.collection.Count;

            public bool IsReadOnly => this.collection.IsReadOnly;

            public IEnumerator<object?> GetEnumerator() => this.collection.Cast<object?>().GetEnumerator();

            IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();

            public void Add(object? item) => this.collection.Add((T)item!);

            public void Clear() => this.collection.Clear();

            public bool Contains(object? item) => this.collection.Contains((T)item!);

            public void CopyTo(object?[] array, int arrayIndex)
            {
                throw new NotSupportedException();
            }

            public bool Remove(object? item) => this.collection.Remove((T)item!);
        }
    }
}