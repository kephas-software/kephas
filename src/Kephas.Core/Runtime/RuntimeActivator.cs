// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RuntimeActivator.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the runtime activator class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Runtime
{
    using Kephas.Activation;

    /// <summary>
    /// Activator based on the runtime type information.
    /// </summary>
    public class RuntimeActivator : ActivatorBase
    {
        /// <summary>
        /// The static instance of the runtime activator.
        /// </summary>
        public static readonly RuntimeActivator Instance = new RuntimeActivator();
    }
}