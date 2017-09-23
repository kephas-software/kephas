// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DebugLogAmbientServicesBuilderExtensions.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Extension methods for the AmbientServicesBuilder.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Diagnostics.Logging
{
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
        /// <returns>
        /// The provided ambient services builder.
        /// </returns>
        public static AmbientServicesBuilder WithDebugLogManager(this AmbientServicesBuilder ambientServicesBuilder)
        {
            Requires.NotNull(ambientServicesBuilder, nameof(ambientServicesBuilder));

            return ambientServicesBuilder.WithLogManager(new DebugLogManager());
        }
    }
}