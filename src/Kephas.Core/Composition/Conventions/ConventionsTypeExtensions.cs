// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ConventionsTypeExtensions.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the conventions type extensions class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Composition.Conventions
{
    using System;
    using System.Reflection;
    using System.Runtime.CompilerServices;

    using Kephas.Composition.AttributedModel;

    /// <summary>
    /// Type extensions for composition conventions.
    /// </summary>
    public static class ConventionsTypeExtensions
    {
        /// <summary>
        /// Information describing the convention registrar contract type.
        /// </summary>
        private static readonly TypeInfo ConventionRegistrarContractTypeInfo = typeof(IConventionsRegistrar).GetTypeInfo();

        /// <summary>
        /// Information describing the application service information provider contract type.
        /// </summary>
        private static readonly TypeInfo AppServiceInfoProviderContractTypeInfo = typeof(IAppServiceInfoProvider).GetTypeInfo();

        /// <summary>
        /// Indicates whether the provided type is an instantiable <see cref="IConventionsRegistrar"/>
        /// type.
        /// </summary>
        /// <param name="conventionsType">The conventions type to act on.</param>
        /// <returns>
        /// True if the type is an instantiable <see cref="IConventionsRegistrar"/> type, false if not.
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsInstantiableConventionsRegistrarType(this Type conventionsType)
        {
            return IsInstantiableConventionsType(conventionsType, ConventionRegistrarContractTypeInfo);
        }

        /// <summary>
        /// Indicates whether the provided type is an instantiable <see cref="IConventionsRegistrar"/>
        /// type.
        /// </summary>
        /// <param name="conventionsType">The conventions type to act on.</param>
        /// <returns>
        /// True if the type is an instantiable <see cref="IConventionsRegistrar"/> type, false if not.
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsInstantiableConventionsRegistrarType(this TypeInfo conventionsType)
        {
            return IsInstantiableConventionsType(conventionsType, ConventionRegistrarContractTypeInfo);
        }

        /// <summary>
        /// Indicates whether the provided type is an instantiable <see cref="IAppServiceInfoProvider"/>
        /// type.
        /// </summary>
        /// <param name="conventionsType">The conventions type to act on.</param>
        /// <returns>
        /// True if the type is an instantiable <see cref="IAppServiceInfoProvider"/> type, false if not.
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsInstantiableAppServiceInfoProviderType(this Type conventionsType)
        {
            return IsInstantiableConventionsType(conventionsType, AppServiceInfoProviderContractTypeInfo);
        }

        /// <summary>
        /// Indicates whether the provided type is an instantiable <see cref="IAppServiceInfoProvider"/>
        /// type.
        /// </summary>
        /// <param name="conventionsType">The conventions type to act on.</param>
        /// <returns>
        /// True if the type is an instantiable <see cref="IAppServiceInfoProvider"/> type, false if not.
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsInstantiableAppServiceInfoProviderType(this TypeInfo conventionsType)
        {
            return IsInstantiableConventionsType(conventionsType, AppServiceInfoProviderContractTypeInfo);
        }

        /// <summary>
        /// Indicates whether the provided type is an instantiable type implementing the provided
        /// contract type.
        /// </summary>
        /// <param name="conventionsType">The conventions type to act on.</param>
        /// <param name="conventionsContractType">Type of the conventions contract.</param>
        /// <returns>
        /// True if the type is an instantiable type implementing the provided contract type, false if
        /// not.
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsInstantiableConventionsType(this Type conventionsType, TypeInfo conventionsContractType)
        {
            return IsInstantiableConventionsType(conventionsType.GetTypeInfo(), conventionsContractType);
        }

        /// <summary>
        /// Indicates whether the provided type is an instantiable type implementing the provided
        /// contract type.
        /// </summary>
        /// <param name="conventionsType">The conventions type to act on.</param>
        /// <param name="conventionsContractType">Type of the conventions contract.</param>
        /// <returns>
        /// True if the type is an instantiable type implementing the provided contract type, false if
        /// not.
        /// </returns>
        public static bool IsInstantiableConventionsType(this TypeInfo conventionsType, TypeInfo conventionsContractType)
        {
            return conventionsType.IsClass
                   && !conventionsType.IsAbstract
                   && conventionsContractType.IsAssignableFrom(conventionsType)
                   && conventionsType.GetCustomAttribute<ExcludeFromCompositionAttribute>() == null;
        }
    }
}