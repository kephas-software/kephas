// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Program.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Implements the program class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace RoleGame.CustomizeMef
{
    using System.Threading.Tasks;
    using Kephas;
    using Kephas.Application;
    using RoleGame.Composition;

    class Program
    {
        static Task Main(string[] args)
        {
            return new App(ambientServices =>
                ambientServices
                    .WithNLogManager()
                    .WithDynamicAppRuntime(an => an.Name.StartsWith("Kephas") || an.Name.StartsWith("RoleGame"))
                    .BuildWithSystemComposition(
                        cfg => cfg.WithConventionsRegistrar(new GameConventionRegistrar())))
                .BootstrapAsync(new AppArgs(args));
        }
    }
}
