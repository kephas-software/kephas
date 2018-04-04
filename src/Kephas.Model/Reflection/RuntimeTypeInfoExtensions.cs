// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RuntimeTypeInfoExtensions.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the runtime type information extensions class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Model.Reflection
{
    using System;
    using System.Linq;
    using System.Reflection;

    using Kephas.Model.AttributedModel;
    using Kephas.Reflection;
    using Kephas.Runtime;

    /// <summary>
    /// Extension methods for <see cref="IRuntimeTypeInfo"/>.
    /// </summary>
    public static class RuntimeTypeInfoExtensions
    {
        /// <summary>
        /// Gets the classifier kind for a <see cref="IRuntimeTypeInfo"/>.
        /// </summary>
        /// <param name="runtimeTypeInfo">The runtimeTypeInfo to act on.</param>
        /// <returns>
        /// The classifier kind.
        /// </returns>
        public static Type GetClassifierKind(this IRuntimeTypeInfo runtimeTypeInfo)
        {
            if (runtimeTypeInfo == null)
            {
                return null;
            }

            var attr = runtimeTypeInfo.Annotations.OfType<ClassifierKindAttribute>().FirstOrDefault();
            if (attr != null)
            {
                return attr.ClassifierType;
            }

            return runtimeTypeInfo["__ClassifierKind"] as Type;
        }

        /// <summary>
        /// Gets a value indicating whether the type is excluded from model.
        /// </summary>
        /// <param name="typeInfo">The type to act on.</param>
        /// <returns>
        /// True if the type is excluded from model, false if not.
        /// </returns>
        public static bool IsExcludedFromModel(this Type typeInfo)
        {
            return typeInfo == null || typeInfo.AsRuntimeTypeInfo().GetAttribute<ExcludeFromModelAttribute>() != null;
        }

        /// <summary>
        /// Gets a value indicating whether the type is excluded from model.
        /// </summary>
        /// <param name="typeInfo">The type to act on.</param>
        /// <returns>
        /// True if the type is excluded from model, false if not.
        /// </returns>
        public static bool IsExcludedFromModel(this TypeInfo typeInfo)
        {
            return typeInfo == null || typeInfo.GetCustomAttribute<ExcludeFromModelAttribute>() != null;
        }

        /// <summary>
        /// Gets a value indicating whether the type or member is excluded from model.
        /// </summary>
        /// <param name="memberInfo">The member to act on.</param>
        /// <returns>
        /// True if the type or member is excluded from model, false if not.
        /// </returns>
        public static bool IsExcludedFromModel(this MemberInfo memberInfo)
        {
            return memberInfo == null || memberInfo.GetCustomAttribute<ExcludeFromModelAttribute>() != null;
        }

        /// <summary>
        /// Gets a value indicating whether the type is excluded from model.
        /// </summary>
        /// <param name="memberInfo">The member to act on.</param>
        /// <returns>
        /// True if the type is excluded from model, false if not.
        /// </returns>
        public static bool IsExcludedFromModel(this IAttributeProvider memberInfo)
        {
            return memberInfo == null || memberInfo.GetAttribute<ExcludeFromModelAttribute>() != null;
        }

        /// <summary>
        /// Gets a value indicating whether the <paramref name="runtimeTypeInfo"/> is a model type.
        /// </summary>
        /// <param name="runtimeTypeInfo">The runtimeTypeInfo to act on.</param>
        /// <returns>
        /// <c>true</c> if the runtime type is a model type, <c>false</c> if not.
        /// </returns>
        public static bool IsModelType(this IRuntimeTypeInfo runtimeTypeInfo)
        {
            if (runtimeTypeInfo == null)
            {
                return false;
            }

            return (bool)(runtimeTypeInfo["__IsModelType"] ?? false);
        }

        /// <summary>
        /// Sets the classifier kind for a <see cref="IRuntimeTypeInfo"/>.
        /// </summary>
        /// <param name="runtimeTypeInfo">The runtimeTypeInfo to act on.</param>
        /// <param name="classifierKind">The classifier kind.</param>
        internal static void SetClassifierKind(this IRuntimeTypeInfo runtimeTypeInfo, Type classifierKind)
        {
            if (runtimeTypeInfo == null)
            {
                return;
            }

            var attr = runtimeTypeInfo.Annotations.OfType<ClassifierKindAttribute>().FirstOrDefault();
            if (attr != null)
            {
                return;
            }

            runtimeTypeInfo["__ClassifierKind"] = classifierKind;
        }

        /// <summary>
        /// Sets a value to indicate whether the <paramref name="runtimeTypeInfo"/> is a model type.
        /// </summary>
        /// <param name="runtimeTypeInfo">The runtimeTypeInfo to act on.</param>
        /// <param name="value"><c>true</c> if the runtime type is a model type, otherwise <c>false</c>.</param>
        internal static void SetIsModelType(this IRuntimeTypeInfo runtimeTypeInfo, bool value)
        {
            if (runtimeTypeInfo == null)
            {
                return;
            }

            runtimeTypeInfo["__IsModelType"] = value;
        }
    }
}