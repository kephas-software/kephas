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
        /// Registers the provided service using a registration builder.
        /// </summary>
        /// <param name="contractDeclarationType">The contract declaration type.</param>
        /// <param name="instancingStrategy">The instancing strategy.</param>
        /// <param name="builder">The builder.</param>
        /// <returns>
        /// This <see cref="IAmbientServices"/>.
        /// </returns>
        IAmbientServices IAmbientServices.RegisterService(Type contractDeclarationType, object instancingStrategy, Action<IRegistrationBuilder>? builder)
        {
            contractDeclarationType = contractDeclarationType ?? throw new ArgumentNullException(nameof(contractDeclarationType));
            instancingStrategy = instancingStrategy ?? throw new ArgumentNullException(nameof(instancingStrategy));

            var serviceBuilder = new ServiceRegistrationBuilder(this.ServiceRegistry, contractDeclarationType, instancingStrategy);
            builder?.Invoke(serviceBuilder);
            this.ServiceRegistry.RegisterSource(serviceBuilder.Build());

            return this;
        }
    }
}