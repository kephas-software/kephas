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
        protected override void BuildServicesContainer(IAmbientServices ambientServices)
        {
            ambientServices
                .WithNLogManager()
                .WithDynamicAppRuntime(an => an.Name.StartsWith("Kephas") || an.Name.StartsWith("RoleGame"))
                .BuildWithSystemComposition();
        }
    }
}