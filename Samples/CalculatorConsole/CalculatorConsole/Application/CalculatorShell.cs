// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CalculatorShell.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Implements the calculator shell class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace CalculatorConsole.Application
{
    using Kephas;
    using Kephas.Application;
    using Kephas.Logging.NLog;

    public class CalculatorShell : AppBase
    {
        protected override void ConfigureAmbientServices(IAmbientServices ambientServices)
        {
            ambientServices
                .WithNLogManager()
                .WithStaticAppRuntime()
                .WithMefCompositionContainer();
        }
    }
}