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
    using System.Text;

    using Kephas.Diagnostics.Contracts;
    using Kephas.Logging;

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
        /// <param name="replaceDefault">Optional. True to replace <see cref="LoggingHelper.DefaultLogManager"/>.</param>
        /// <returns>
        /// This <paramref name="ambientServices"/>.
        /// </returns>
        public static IAmbientServices WithDebugLogManager(this IAmbientServices ambientServices, Action<string, string, object, object?[], Exception?>? logCallback = null, bool replaceDefault = true)
        {
            ambientServices = ambientServices ?? throw new ArgumentNullException(nameof(ambientServices));

            return ambientServices.WithLogManager(new DebugLogManager(logCallback), replaceDefault);
        }

        /// <summary>
        /// Sets the debug log manager to the ambient services.
        /// </summary>
        /// <param name="ambientServices">The ambient services.</param>
        /// <param name="stringBuilder">The string builder.</param>
        /// <param name="replaceDefault">Optional. True to replace <see cref="LoggingHelper.DefaultLogManager"/>.</param>
        /// <returns>
        /// This <paramref name="ambientServices"/>.
        /// </returns>
        public static IAmbientServices WithDebugLogManager(this IAmbientServices ambientServices, StringBuilder stringBuilder, bool replaceDefault = true)
        {
            ambientServices = ambientServices ?? throw new ArgumentNullException(nameof(ambientServices));

            return ambientServices.WithLogManager(new DebugLogManager(stringBuilder), replaceDefault);
        }
    }
}