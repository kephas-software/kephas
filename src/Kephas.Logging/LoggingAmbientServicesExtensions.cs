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
        /// Gets the logger with the provided name.
        /// </summary>
        /// <param name="ambientServices">The ambient services.</param>
        /// <param name="loggerName">Name of the logger.</param>
        /// <returns>
        /// A logger for the provided name.
        /// </returns>
        public static ILogger GetLogger(this IAmbientServices ambientServices, string loggerName)
        {
            ambientServices = ambientServices ?? throw new ArgumentNullException(nameof(ambientServices));
            loggerName = loggerName ?? throw new ArgumentNullException(nameof(loggerName));

            return ambientServices.LogManager.GetLogger(loggerName);
        }

        /// <summary>
        /// Gets the logger for the provided type.
        /// </summary>
        /// <param name="ambientServices">The ambient services.</param>
        /// <param name="type">The type.</param>
        /// <returns>
        /// A logger for the provided type.
        /// </returns>
        public static ILogger GetLogger(this IAmbientServices ambientServices, Type type)
        {
            ambientServices = ambientServices ?? throw new ArgumentNullException(nameof(ambientServices));
            type = type ?? throw new ArgumentNullException(nameof(type));

            return ambientServices.LogManager.GetLogger(type);
        }

        /// <summary>
        /// Gets the logger for the provided type.
        /// </summary>
        /// <typeparam name="T">The type for which a logger should be created.</typeparam>
        /// <param name="ambientServices">The ambient services.</param>
        /// <returns>
        /// A logger for the provided type.
        /// </returns>
        public static ILogger GetLogger<T>(this IAmbientServices ambientServices)
        {
            ambientServices = ambientServices ?? throw new ArgumentNullException(nameof(ambientServices));

            return ambientServices.LogManager.GetLogger(typeof(T));
        }

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