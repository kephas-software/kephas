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

    /// <summary>
    /// Contract interface for ambient services.
    /// </summary>
    public interface IAmbientServices : IExpando, IServiceProvider, IDisposable
    {
        private static readonly List<Action<IAmbientServices>> Initializers = new ();

        /// <summary>
        /// Gets the injector.
        /// </summary>
        /// <value>
        /// The injector.
        /// </value>
        public IInjector Injector => this.GetRequiredService<IInjector>();

        /// <summary>
        /// Gets the log manager.
        /// </summary>
        /// <value>
        /// The log manager.
        /// </value>
        public ILogManager LogManager => this.GetRequiredService<ILogManager>();

        /// <summary>
        /// Gets a value indicating whether the service with the provided contract is registered.
        /// </summary>
        /// <param name="contractType">Type of the service contract.</param>
        /// <returns>
        /// <c>true</c> if the service is registered, <c>false</c> if not.
        /// </returns>
        public bool IsRegistered(Type contractType);

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
        /// Adds an initializer for <see cref="IAmbientServices"/>.
        /// </summary>
        /// <param name="initializer">The initializer.</param>
        public static void RegisterInitializer(Action<IAmbientServices> initializer)
        {
            lock (Initializers)
            {
                Initializers.Add(initializer ?? throw new ArgumentNullException(nameof(initializer)));
            }
        }

        /// <summary>
        /// Initializes the ambient services with the registered initializers.
        /// </summary>
        /// <param name="ambientServices">The ambient services.</param>
        public static void Initialize(IAmbientServices ambientServices)
        {
            lock (Initializers)
            {
                foreach (var initializer in Initializers)
                {
                    initializer(ambientServices);
                }
            }
        }
    }
}