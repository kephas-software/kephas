// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TestBuildContext.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the test registration context class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Core.Tests.Injection
{
    using System.Reflection;

    using Kephas.Injection.Builder;

    /// <summary>
    /// The test registration context.
    /// </summary>
    public class TestBuildContext : InjectionBuildContext
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TestBuildContext"/> class.
        /// </summary>
        /// <param name="ambientServices">The ambient services (optional).</param>
        public TestBuildContext(IAmbientServices? ambientServices = null)
            : base(ambientServices?.GetAppRuntime().GetAppAssemblies() ?? new List<Assembly>())
        {
        }
    }
}