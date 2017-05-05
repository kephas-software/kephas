﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LocalizationExtensions.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Implements the localization extensions class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Reflection
{
    using System;
    using System.Reflection;

    using Kephas.Diagnostics.Contracts;
    using Kephas.Reflection.Localization;

    /// <summary>
    /// Localization extensions for reflection.
    /// </summary>
    public static class LocalizationExtensions
    {
        /// <summary>
        /// Gets the dynamic localization property name.
        /// </summary>
        /// <value>
        /// The dynamic localization property name.
        /// </value>
        private const string LocalizationPropertyName = "Kephas-Localization";

        /// <summary>
        /// Gets the localization for a <see cref="Type"/>.
        /// </summary>
        /// <param name="type">The type to act on.</param>
        /// <returns>
        /// The type localization.
        /// </returns>
        public static ITypeInfoLocalization GetLocalization(this Type type)
        {
            Requires.NotNull(type, nameof(type));

            return GetLocalization(type.AsRuntimeTypeInfo());
        }

        /// <summary>
        /// Gets the localization for a <see cref="TypeInfo"/>.
        /// </summary>
        /// <param name="typeInfo">The type information to act on.</param>
        /// <returns>
        /// The type localization.
        /// </returns>
        public static ITypeInfoLocalization GetLocalization(this TypeInfo typeInfo)
        {
            Requires.NotNull(typeInfo, nameof(typeInfo));

            return GetLocalization(typeInfo.AsRuntimeTypeInfo());
        }

        /// <summary>
        /// Gets the localization for a <see cref="ITypeInfo"/>.
        /// </summary>
        /// <param name="typeInfo">The type information to act on.</param>
        /// <returns>
        /// The type localization.
        /// </returns>
        public static ITypeInfoLocalization GetLocalization(this ITypeInfo typeInfo)
        {
            Requires.NotNull(typeInfo, nameof(typeInfo));

            var localization = typeInfo[LocalizationPropertyName] as ITypeInfoLocalization;
            if (localization == null && !typeInfo.HasMember(LocalizationPropertyName))
            {
                localization = new TypeInfoLocalization(typeInfo);
                typeInfo[LocalizationPropertyName] = localization;
            }

            return localization;
        }

        /// <summary>
        /// Gets the localization for a <see cref="PropertyInfo"/>.
        /// </summary>
        /// <param name="propertyInfo">The property information to act on.</param>
        /// <returns>
        /// The property localization.
        /// </returns>
        public static IPropertyInfoLocalization GetLocalization(this PropertyInfo propertyInfo)
        {
            Requires.NotNull(propertyInfo, nameof(propertyInfo));

            var runtimeTypeInfo = propertyInfo.DeclaringType.AsRuntimeTypeInfo();
            var runtimePropertyInfo = runtimeTypeInfo.Properties[propertyInfo.Name];
            return GetLocalization(runtimePropertyInfo);
        }

        /// <summary>
        /// Gets the localization for a <see cref="IPropertyInfo"/>.
        /// </summary>
        /// <param name="propertyInfo">The property information to act on.</param>
        /// <returns>
        /// The property localization.
        /// </returns>
        public static IPropertyInfoLocalization GetLocalization(this IPropertyInfo propertyInfo)
        {
            Requires.NotNull(propertyInfo, nameof(propertyInfo));

            var localization = propertyInfo[LocalizationPropertyName] as IPropertyInfoLocalization;
            if (localization == null && !propertyInfo.HasMember(LocalizationPropertyName))
            {
                var typeInfo = propertyInfo.DeclaringContainer as ITypeInfo;
                var typeInfoLocalization = GetLocalization(typeInfo);
                localization = typeInfoLocalization.Properties[propertyInfo.Name];
                propertyInfo[LocalizationPropertyName] = localization;
            }

            return localization;
        }
    }
}