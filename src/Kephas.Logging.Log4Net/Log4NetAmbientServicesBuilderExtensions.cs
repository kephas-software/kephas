// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Log4NetAmbientServicesBuilderExtensions.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
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
    /// Extension methods for the <see cref="AmbientServicesBuilder"/>.
    /// </summary>
    public static class Log4NetAmbientServicesBuilderExtensions
    {
        /// <summary>
        /// Sets the NLog log manager to the ambient services.
        /// </summary>
        /// <param name="ambientServicesBuilder">The ambient services builder.</param>
        /// <returns>
        /// The provided ambient services builder.
        /// </returns>
        public static AmbientServicesBuilder WithLog4NetManager(this AmbientServicesBuilder ambientServicesBuilder)
        {
            Requires.NotNull(ambientServicesBuilder, nameof(ambientServicesBuilder));

            return ambientServicesBuilder.WithLogManager(new Log4NetLogManager());
        }
    }
}