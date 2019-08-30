// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AmbientServicesLog4NetExtensions.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the log 4 net ambient services builder extensions class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Logging.Log4Net
{
    using Kephas.Diagnostics.Contracts;

    /// <summary>
    /// Extension methods for the <see cref="IAmbientServices"/>.
    /// </summary>
    public static class AmbientServicesLog4NetExtensions
    {
        /// <summary>
        /// Sets the NLog log manager to the ambient services.
        /// </summary>
        /// <param name="ambientServices">The ambient services.</param>
        /// <returns>
        /// The provided ambient services.
        /// </returns>
        public static IAmbientServices WithLog4NetManager(this IAmbientServices ambientServices)
        {
            Requires.NotNull(ambientServices, nameof(ambientServices));

            return ambientServices.WithLogManager(new Log4NetLogManager());
        }
    }
}