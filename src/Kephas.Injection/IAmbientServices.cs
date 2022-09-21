// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IAmbientServices.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Contract interface for ambient services.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas
{
    using System;
    using System.Collections.Generic;

    using Kephas.Dynamic;
    using Kephas.Injection;
    using Kephas.Injection.Builder;
    using Kephas.Logging;
    using Kephas.Services.Reflection;

    /// <summary>
    /// Contract interface for ambient services.
    /// </summary>
    public interface IAmbientServices : IExpando, IEnumerable<IAppServiceInfo>
    {
        private static readonly List<Action<IAmbientServices>> Collectors = new ();

        /// <summary>
        /// Gets a value indicating whether the service with the provided contract is registered.
        /// </summary>
        /// <param name="contractType">Type of the service contract.</param>
        /// <returns>
        /// <c>true</c> if the service is registered, <c>false</c> if not.
        /// </returns>
        public bool IsRegistered(Type contractType)
            => this.Any(r => r.ContractType == contractType);

        /// <summary>
        /// Registers the provided service using a registration builder.
        /// </summary>
        /// <param name="contractDeclarationType">The contract declaration type.</param>
        /// <param name="instancingStrategy">The instancing strategy.</param>
        /// <param name="builder">The builder.</param>
        /// <returns>
        /// This <see cref="IAmbientServices"/>.
        /// </returns>
        public IAmbientServices RegisterService(Type contractDeclarationType, object instancingStrategy, Action<IRegistrationBuilder>? builder = null);

        /// <summary>
        /// Adds an collector for <see cref="IAmbientServices"/>.
        /// </summary>
        /// <param name="collect">The collect callback function.</param>
        public static void RegisterCollector(Action<IAmbientServices> collect)
        {
            lock (Collectors)
            {
                Collectors.Add(collect ?? throw new ArgumentNullException(nameof(collect)));
            }
        }

        /// <summary>
        /// Initializes the ambient services with the registered initializers.
        /// </summary>
        /// <param name="ambientServices">The ambient services.</param>
        public static void Initialize(IAmbientServices ambientServices)
        {
            lock (Collectors)
            {
                foreach (var initializer in Collectors)
                {
                    initializer(ambientServices);
                }
            }
        }
    }
}