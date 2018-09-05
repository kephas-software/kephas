// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DebugLogAmbientServicesBuilderExtensions.cs" company="Kephas Software SRL">
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
    /// Extension methods for the <see cref="AmbientServicesBuilder"/>.
    /// </summary>
    public static class DebugLogAmbientServicesBuilderExtensions
    {
        /// <summary>
        /// Sets the debug log manager to the ambient services.
        /// </summary>
        /// <param name="ambientServicesBuilder">The ambient services builder.</param>
        /// <param name="logCallback">The log callback (optional).</param>
        /// <returns>
        /// The provided ambient services builder.
        /// </returns>
        public static AmbientServicesBuilder WithDebugLogManager(this AmbientServicesBuilder ambientServicesBuilder, Action<string, string, object, Exception> logCallback = null)
        {
            Requires.NotNull(ambientServicesBuilder, nameof(ambientServicesBuilder));

            return ambientServicesBuilder.WithLogManager(new DebugLogManager(logCallback));
        }
    }
}