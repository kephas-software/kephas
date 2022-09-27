// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LoggingExtensions.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the logging extensions class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Logging
{
    using System;

    using Kephas.Injection;
    using Kephas.Services;

    /// <summary>
    /// Logging extensions.
    /// </summary>
    public static class LoggingExtensions
    {
        /// <summary>
        /// A Type extension method that gets a logger.
        /// </summary>
        /// <param name="type">The type to act on.</param>
        /// <param name="context">Optional. The context used to get the log manager.</param>
        /// <returns>
        /// The logger.
        /// </returns>
        public static ILogger GetLogger(this Type type, IContext? context = null)
        {
            type = type ?? throw new ArgumentNullException(nameof(type));

            var logManager = context?.ServiceProvider?.Resolve<ILogManager>() ?? LoggingHelper.DefaultLogManager;
            return logManager.GetLogger(type);
        }

        /// <summary>
        /// An Object extension method that gets a logger.
        /// </summary>
        /// <remarks>
        /// Use this method only if the object cannot get a logger through dependency injection.
        /// </remarks>
        /// <param name="obj">The object to act on.</param>
        /// <param name="context">The context used to get the log manager (optional).</param>
        /// <returns>
        /// The logger.
        /// </returns>
        public static ILogger GetLogger(this object obj, IContext? context = null)
        {
            obj = obj ?? throw new ArgumentNullException(nameof(obj));

            var objType = obj as Type ?? obj.GetType();
            if (obj is IAmbientServices ambientServices)
            {
                return ambientServices.GetServiceInstance<ILogManager>().GetLogger(objType);
            }

            if (obj is IServiceProvider injector)
            {
                return injector.GetLogger(objType);
            }

            if (obj is IContext contextObj)
            {
                injector = contextObj.ServiceProvider;
                if (injector != null)
                {
                    return injector.GetLogger(objType);
                }
            }

            var logManager = context?.ServiceProvider.Resolve<ILogManager>() ?? LoggingHelper.DefaultLogManager;
            return logManager.GetLogger(objType);
        }
    }
}