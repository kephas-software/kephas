// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IAmbientServicesMixin.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas
{
    using System;

    using Kephas.Injection.Builder;
    using Kephas.Injection.Lite.Builder;
    using Kephas.Services;

    /// <summary>
    /// Mixin for implementing the <see cref="IAmbientServices"/>.
    /// </summary>
    public interface IAmbientServicesMixin : IAmbientServices
    {
        /// <summary>
        /// Gets the service registry.
        /// </summary>
        protected internal IAppServiceRegistry ServiceRegistry { get; }

        /// <summary>
        /// Gets a value indicating whether the service with the provided contract is registered.
        /// </summary>
        /// <param name="contractType">Type of the service contract.</param>
        /// <returns>
        /// <c>true</c> if the service is registered, <c>false</c> if not.
        /// </returns>
        bool IAmbientServices.IsRegistered(Type contractType) =>
            this.ServiceRegistry.IsRegistered(contractType);

        /// <summary>
        /// Registers the provided service using a registration builder.
        /// </summary>
        /// <param name="contractDeclarationType">The contract declaration type.</param>
        /// <param name="instancingStrategy">The instancing strategy.</param>
        /// <param name="builder">The builder.</param>
        /// <returns>
        /// This <see cref="IAmbientServices"/>.
        /// </returns>
        IAmbientServices IAmbientServices.RegisterService(Type contractDeclarationType, object instancingStrategy, Action<IRegistrationBuilder>? builder = null)
        {
            contractDeclarationType = contractDeclarationType ?? throw new ArgumentNullException(nameof(contractDeclarationType));
            instancingStrategy = instancingStrategy ?? throw new ArgumentNullException(nameof(instancingStrategy));

            var serviceBuilder = new ServiceRegistrationBuilder(this.ServiceRegistry, contractDeclarationType, instancingStrategy);
            builder?.Invoke(serviceBuilder);
            this.ServiceRegistry.RegisterSource(serviceBuilder.Build());

            return this;
        }

        /// <summary>
        /// Gets the service object of the specified type.
        /// </summary>
        /// <returns>
        /// A service object of type <paramref name="contractType"/>.-or- null if there is no service object of type <paramref name="contractType"/>.
        /// </returns>
        /// <param name="contractType">The contract type of service to get. </param>
        object? IServiceProvider.GetService(Type contractType) =>
            this.ServiceRegistry.GetService(contractType);
    }
}