// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RegistrationOrderExtensions.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the registration order extensions class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Composition.Autofac
{
    using global::Autofac.Builder;
    using global::Autofac.Core;

    /// <summary>
    /// A registration order extensions.
    /// </summary>
    internal static class RegistrationOrderExtensions
    {
        /// <summary>
        /// The registration order metadata key.
        /// </summary>
        internal const string RegistrationOrderMetadataKey = "__RegistrationOrder";

        /// <summary>
        /// Gets the registration order.
        /// </summary>
        /// <param name="registration">The registration.</param>
        /// <returns>The registration order.</returns>
        internal static long GetRegistrationOrder(this IComponentRegistration registration)
        {
            return registration.Metadata.TryGetValue(RegistrationOrderMetadataKey, out var value) ? (long)value! : long.MaxValue;
        }

        /// <summary>
        /// Inherits the registration order from the source.
        /// </summary>
        /// <param name="registration">The registration builder.</param>
        /// <param name="source">The source.</param>
        /// <typeparam name="TLimit">The limit type.</typeparam>
        /// <typeparam name="TActivatorData">The activator data type.</typeparam>
        /// <typeparam name="TSingleRegistrationStyle">The single registration style type.</typeparam>
        /// <returns>The provided registration builder.</returns>
        internal static IRegistrationBuilder<TLimit, TActivatorData, TSingleRegistrationStyle> InheritRegistrationOrderFrom<TLimit, TActivatorData, TSingleRegistrationStyle>(
            this IRegistrationBuilder<TLimit, TActivatorData, TSingleRegistrationStyle> registration,
            IComponentRegistration source)
            where TSingleRegistrationStyle : SingleRegistrationStyle
        {
            var sourceRegistrationOrder = source.GetRegistrationOrder();
            registration.RegistrationData.Metadata[RegistrationOrderMetadataKey] = sourceRegistrationOrder;

            return registration;
        }
    }
}