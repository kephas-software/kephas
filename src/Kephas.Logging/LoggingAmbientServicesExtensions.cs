// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LoggingAmbientServicesExtensions.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas
{
    using System;

    using Kephas.Logging;

    /// <summary>
    /// Logging extension methods for <see cref="IAmbientServices"/>.
    /// </summary>
    public static class LoggingAmbientServicesExtensions
    {
        /// <summary>
        /// Sets the log manager to the ambient services.
        /// </summary>
        /// <param name="ambientServices">The ambient services.</param>
        /// <param name="logManager">The log manager.</param>
        /// <param name="replaceDefault">Optional. True to replace the <see cref="LoggingHelper.DefaultLogManager"/>.</param>
        /// <returns>
        /// This <paramref name="ambientServices"/>.
        /// </returns>
        public static IAmbientServices WithLogManager(this IAmbientServices ambientServices, ILogManager logManager, bool replaceDefault = true)
        {
            ambientServices = ambientServices ?? throw new ArgumentNullException(nameof(ambientServices));
            logManager = logManager ?? throw new ArgumentNullException(nameof(logManager));

            if (replaceDefault)
            {
                LoggingHelper.DefaultLogManager = logManager;
            }

            ambientServices.Register<ILogManager>(logManager);

            return ambientServices;
        }
    }
}