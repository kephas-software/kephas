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
        internal const string RegistrationOrderMetadataKey = "__RegistrationOrder";

        internal static long GetRegistrationOrder(this IComponentRegistration registration)
        {
            object value;
            return registration.Metadata.TryGetValue(RegistrationOrderMetadataKey, out value) ? (long)value : long.MaxValue;
        }

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