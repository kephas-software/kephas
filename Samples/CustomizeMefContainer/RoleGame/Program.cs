// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Program.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Implements the program class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace RoleGame
{
    using System.Threading.Tasks;
    using Kephas.Application;
    using RoleGame.Application;

    class Program
    {
        static Task Main(string[] args)
        {
            return new RoleGameShell().BootstrapAsync(new AppArgs(args));
        }
    }
}
