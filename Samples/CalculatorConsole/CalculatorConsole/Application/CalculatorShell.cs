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
    using System;
    using System.Threading;

    public class CalculatorShell : AppBase
    {
        public CalculatorShell()
            : base(containerBuilder: ambientServices => ambientServices
                .WithNLogManager()
                .WithStaticAppRuntime()
                .BuildWithLite())
        {
        }
    }
}