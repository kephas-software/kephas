// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AmbientServicesDebugLogExtensions.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Extension methods for the AmbientServicesBuilder.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Diagnostics.Logging
{
    using System;

    using Kephas.Diagnostics.Contracts;

    /// <summary>
    /// Extension methods for the <see cref="IAmbientServices"/>.
    /// </summary>
    public static class AmbientServicesDebugLogExtensions
    {
        /// <summary>
        /// Sets the debug log manager to the ambient services.
        /// </summary>
        /// <param name="ambientServices">The ambient services.</param>
        /// <param name="logCallback">Optional. The log callback.</param>
        /// <returns>
        /// The provided ambient services builder.
        /// </returns>
        public static IAmbientServices WithDebugLogManager(this IAmbientServices ambientServices, Action<string, string, object, Exception> logCallback = null)
        {
            Requires.NotNull(ambientServices, nameof(ambientServices));

            return ambientServices.WithLogManager(new DebugLogManager(logCallback));
        }
    }
}