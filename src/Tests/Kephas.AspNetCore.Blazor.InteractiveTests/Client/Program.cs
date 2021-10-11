// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Program.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.AspNetCore.Blazor.InteractiveTests.Client
{
    using System.Threading.Tasks;

    using Kephas.Application;
    using Kephas.AspNetCore.Blazor.InteractiveTests.Client.Application;

    public class Program
    {
        public static async Task Main(string[] args)
        {
            var clientApp = new ClientApp<App>(new AppArgs(args), a => a.BuildWithAutofac());

            await clientApp.BootstrapAsync();
        }
    }
}
