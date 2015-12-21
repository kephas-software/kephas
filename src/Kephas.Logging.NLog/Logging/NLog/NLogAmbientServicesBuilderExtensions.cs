// --------------------------------------------------------------------------------------------------------------------
// <copyright file="NLogAmbientServicesBuilderExtensions.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Extension methods for the AmbientServicesBuilder.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Logging.NLog
{
    using System.Diagnostics.Contracts;

    /// <summary>
    /// Extension methods for the <see cref="AmbientServicesBuilder"/>.
    /// </summary>
    public static class NLogAmbientServicesBuilderExtensions
    {
        /// <summary>
        /// Sets the NLog log manager to the ambient services.
        /// </summary>
        /// <param name="ambientServicesBuilder">The ambient services builder.</param>
        /// <returns>
        /// The provided ambient services builder.
        /// </returns>
        public static AmbientServicesBuilder WithNLogManager(this AmbientServicesBuilder ambientServicesBuilder)
        {
            Contract.Requires(ambientServicesBuilder != null);

            return ambientServicesBuilder.WithLogManager(new NLogManager());
        }
    }
}