// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AppShutdownInstruction.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the application termination instruction class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Application
{
    /// <summary>
    /// Values that represent application shutdown instructions.
    /// </summary>
    /// <seealso/>
    public enum AppShutdownInstruction
    {
        /// <summary>
        /// The shutdown signal should be ignored.
        /// </summary>
        Ignore = 0,

        /// <summary>
        /// Continues the termination with the shutdown procedure.
        /// </summary>
        Shutdown = 1,
    }
}
