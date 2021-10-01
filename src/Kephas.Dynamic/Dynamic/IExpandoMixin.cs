// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IExpandoMixin.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Dynamic
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Runtime.CompilerServices;

    using Kephas.Runtime;

    /// <summary>
    /// Mixin for providing a default implementation of an <see cref="IExpandoBase"/>.
    /// </summary>
    public interface IExpandoMixin : IExpandoBase
    {
        /// <summary>
        /// Gets the inner dictionary.
        /// </summary>
        protected IDictionary<string, object?> InnerDictionary { get; }

        /// <summary>
        /// Gets a weak reference to the inner object.
        /// </summary>
        protected object? InnerObject => this;

        /// <summary>
        /// Gets the binders to use when retrieving the expando members.
        /// </summary>
        protected ExpandoMemberBinderKind MemberBinders => ExpandoMemberBinderKind.All;

        /// <summary>
        /// Gets a value indicating whether to ignore the case when identifying the members by name.
        /// </summary>
        protected bool IgnoreCase => false;

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
        object? IDynamic.this[string key]
        {
            get => TryGetValue(this, key, out var value) ? value : null;
            set => TrySetValue(this, key, value);
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
        IDictionary<string, object?> IExpandoBase.ToDictionary(
            Func<string, string>? keyFunc,
            Func<object?, object?>? valueFunc)
            => ToDictionary(this, keyFunc, valueFunc);

        /// <summary>
        /// Indicates whether the <paramref name="memberName"/> is defined in the expando.
        /// </summary>
        /// <param name="memberName">Name of the member.</param>
        /// <returns>
        /// True if defined, false if not.
        /// </returns>
        bool IExpandoBase.HasDynamicMember(string memberName)
            => HasDynamicMember(this, memberName);

        /// <summary>
        /// Indicates whether the <paramref name="memberName"/> is defined in the expando.
        /// </summary>
        /// <param name="self">The expando.</param>
        /// <param name="memberName">Name of the member.</param>
        /// <returns>
        /// True if defined, false if not.
        /// </returns>
        protected internal static bool HasDynamicMember(IExpandoMixin self, string memberName)
        {
            var binders = self.MemberBinders;

            // First check for public properties over this instance
            var innerObject = self.InnerObject;
            if (self != innerObject
                && binders.HasFlag(ExpandoMemberBinderKind.This)
                && TryGetPropertyInfo(self.GetType(), memberName, self.IgnoreCase) != null)
            {
                return true;
            }

            // then check for public properties in the inner object
            if (innerObject != null
                && binders.HasFlag(ExpandoMemberBinderKind.InnerObject)
                && TryGetPropertyInfo(innerObject.GetType()!, memberName, self.IgnoreCase) != null)
            {
                return true;
            }

            // last, check the dictionary for member
            if (binders.HasFlag(ExpandoMemberBinderKind.InnerDictionary))
            {
                return self.InnerDictionary.ContainsKey(memberName);
            }

            return false;
        }

        /// <summary>
        /// Converts the expando to a dictionary having as keys the property names and as values the
        /// respective properties' values.
        /// </summary>
        /// <param name="self">The </param>
        /// <param name="keyFunc">Optional. The key transformation function.</param>
        /// <param name="valueFunc">Optional. The value transformation function.</param>
        /// <returns>
        /// A dictionary of property values with their associated names.
        /// </returns>
        protected internal static IDictionary<string, object?> ToDictionary(
            IExpandoMixin self,
            Func<string, string>? keyFunc,
            Func<object?, object?>? valueFunc)
        {
            var binders = self.MemberBinders;
            var innerDictionary = self.InnerDictionary;

            // add the properties in their overwrite order:
            // first, the values in the dictionary
            var dictionary = binders.HasFlag(ExpandoMemberBinderKind.InnerDictionary)
                ? keyFunc == null && valueFunc == null
                    ? new Dictionary<string, object?>(innerDictionary)
                    : innerDictionary.ToDictionary(
                        kv => keyFunc == null ? kv.Key : keyFunc(kv.Key),
                        kv => valueFunc == null ? kv.Value : valueFunc(kv.Value))
                : new Dictionary<string, object?>();

            // second, the values in the inner object
            var innerObject = self.InnerObject;
            if (innerObject != null && binders.HasFlag(ExpandoMemberBinderKind.InnerObject))
            {
                var innerObjectType = innerObject.GetType();
                foreach (var prop in RuntimeHelper.GetTypeProperties(innerObjectType))
                {
                    var propName = prop.Name;
                    var value = prop.GetValue(innerObject);
                    dictionary[keyFunc == null ? propName : keyFunc(propName)] =
                        valueFunc == null ? value : valueFunc(value);
                }
            }

            // last, the values in this instance's properties
            if (self != innerObject && binders.HasFlag(ExpandoMemberBinderKind.This))
            {
                var thisType = self.GetType();
                foreach (var prop in RuntimeHelper.GetTypeProperties(thisType))
                {
                    var propName = prop.Name;
                    var value = prop.GetValue(self);
                    dictionary[keyFunc == null ? propName : keyFunc(propName)] =
                        valueFunc == null ? value : valueFunc(value);
                }
            }

            return dictionary;
        }

        /// <summary>
        /// Attempts to get the dynamic value with the given key.
        /// </summary>
        /// <remarks>
        /// First of all, it is tried to get a property value from the inner object, if one is set.
        /// The next try is to retrieve the property value from the expando object itself.
        /// Lastly, if still a property by the provided name cannot be found, the inner dictionary is searched by the provided key.
        /// </remarks>
        /// <param name="self">The expando.</param>
        /// <param name="key">The key.</param>
        /// <param name="value">The value to get.</param>
        /// <returns>
        /// <c>true</c> if a value is found, <c>false</c> otherwise.
        /// </returns>
        protected internal static bool TryGetValue(IExpandoMixin self, string key, out object? value)
        {
            var binders = self.MemberBinders;
            var innerDictionary = self.InnerDictionary;

            bool? TryGetPropertyValue(Type type, object instance, out object? val)
            {
                var propInfo = TryGetPropertyInfo(type, key, self.IgnoreCase);
                if (propInfo == null)
                {
                    val = null;
                    return null;
                }

                if (propInfo.CanRead)
                {
                    val = propInfo.GetValue(instance);
                    return true;
                }

                val = null;
                return false;
            }

            // first, check the properties in this object
            var innerObject = self.InnerObject;
            if (self != innerObject && binders.HasFlag(ExpandoMemberBinderKind.This))
            {
                var thisType = self.GetType();
                var canRead = TryGetPropertyValue(thisType, self, out value);
                if (canRead != null)
                {
                    return canRead.Value;
                }
            }

            // then, check the inner object
            if (innerObject != null && binders.HasFlag(ExpandoMemberBinderKind.InnerObject))
            {
                var innerObjectType = innerObject.GetType();
                var canRead = TryGetPropertyValue(innerObjectType, innerObject, out value);
                if (canRead != null)
                {
                    return canRead.Value;
                }
            }

            // last, check the dictionary for member
            if (binders.HasFlag(ExpandoMemberBinderKind.InnerDictionary))
            {
                return innerDictionary.TryGetValue(key, out value);
            }

            value = null;
            return false;
        }

        /// <summary>
        /// Attempts to set the value with the given key.
        /// </summary>
        /// <remarks>
        /// First of all, it is tried to set the property value to the inner object, if one is set.
        /// The next try is to set the property value to the expando object itself.
        /// Lastly, if still a property by the provided name cannot be found, the inner dictionary is used to set the value with the provided key.
        /// </remarks>
        /// <param name="self">The expando.</param>
        /// <param name="key">The key.</param>
        /// <param name="value">The value to set.</param>
        /// <returns>
        /// <c>true</c> if the value could be set, <c>false</c> otherwise.
        /// </returns>
        protected internal static bool TrySetValue(IExpandoMixin self, string key, object? value)
        {
            var binders = self.MemberBinders;
            var innerDictionary = self.InnerDictionary;

            bool? TrySetPropertyValue(Type type, object instance)
            {
                var propInfo = TryGetPropertyInfo(type, key, self.IgnoreCase);
                if (propInfo == null)
                {
                    return null;
                }

                if (propInfo.CanWrite)
                {
                    propInfo.SetValue(instance, value);
                    return true;
                }

                return false;
            }

            // first, check the properties in this object
            var innerObject = self.InnerObject;
            if (self != innerObject && binders.HasFlag(ExpandoMemberBinderKind.This))
            {
                var thisType = self.GetType();
                var canSet = TrySetPropertyValue(thisType, self);
                if (canSet != null)
                {
                    return canSet.Value;
                }
            }

            // then check the inner object
            if (innerObject != null && binders.HasFlag(ExpandoMemberBinderKind.InnerObject))
            {
                var innerObjectType = innerObject.GetType();
                var canSet = TrySetPropertyValue(innerObjectType, innerObject);
                if (canSet != null)
                {
                    return canSet.Value;
                }
            }

            // last, check the dictionary for member
            if (binders.HasFlag(ExpandoMemberBinderKind.InnerDictionary))
            {
                if (value == null)
                {
                    innerDictionary.Remove(key);
                }
                else
                {
                    innerDictionary[key] = value;
                }

                return true;
            }

            return false;
        }

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
        protected internal static PropertyInfo? TryGetPropertyInfo(Type type, string key, bool ignoreCase)
            => type.GetProperty(key, GetBindingFlags(ignoreCase));

        /// <summary>
        /// Gets the binding flags for retrieving type members.
        /// </summary>
        /// <param name="ignoreCase">
        /// Indicates whether case insensitive matching should be used.
        /// If not set, the default value inferred from the inner dictionary is used.
        /// </param>
        /// <returns>The binding flags.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected internal static BindingFlags GetBindingFlags(bool ignoreCase)
        {
            var flags = BindingFlags.Instance | BindingFlags.Public;
            if (ignoreCase)
            {
                flags |= BindingFlags.IgnoreCase;
            }

            return flags;
        }
    }
}