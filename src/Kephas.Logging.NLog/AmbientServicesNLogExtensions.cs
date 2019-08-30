// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AmbientServicesNLogExtensions.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Extension methods for the AmbientServicesBuilder.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Logging.NLog
{
    using Kephas.Diagnostics.Contracts;

    /// <summary>
    /// Extension methods for the <see cref="IAmbientServices"/>.
    /// </summary>
    public static class AmbientServicesNLogExtensions
    {
        /// <summary>
        /// Sets the NLog log manager to the ambient services.
        /// </summary>
        /// <param name="ambientServices">The ambient services builder.</param>
        /// <returns>
        /// The provided ambient services builder.
        /// </returns>
        public static IAmbientServices WithNLogManager(this IAmbientServices ambientServices)
        {
            Requires.NotNull(ambientServices, nameof(ambientServices));

            return ambientServices.WithLogManager(new NLogManager());
        }
    }
}