// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Program.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Implements the program class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace RoleGame
{
    using System.Threading.Tasks;

    using RoleGame.Application;

    class Program
    {
        static Task Main(string[] args)
        {
            return new RoleGameShell().BootstrapAsync(args);
        }
    }
}
