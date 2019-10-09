// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RoleGameShell.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Implements the role game shell class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace RoleGame.Application
{
    using Kephas;
    using Kephas.Application;
    using Kephas.Logging.NLog;

    public class RoleGameShell : AppBase
    {
        /// <summary>
        /// Configures the ambient services asynchronously.
        /// </summary>
        /// <remarks>
        /// Override this method to initialize the startup services, like log manager and configuration
        /// manager.
        /// </remarks>
        /// <param name="ambientServices">The ambient services.</param>
        protected override void ConfigureAmbientServices(IAmbientServices ambientServices)
        {
            ambientServices
                .WithNLogManager()
                .WithDynamicAppRuntime()
                .WithMefCompositionContainer();
        }
    }
}