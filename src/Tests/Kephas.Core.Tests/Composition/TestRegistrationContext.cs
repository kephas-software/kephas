// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TestRegistrationContext.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the test registration context class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Core.Tests.Composition
{
    using Kephas.Injection.Hosting;
    using NSubstitute;

    /// <summary>
    /// The test registration context.
    /// </summary>
    public class TestRegistrationContext : InjectionRegistrationContext
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TestRegistrationContext"/> class.
        /// </summary>
        /// <param name="ambientServices">The ambient services (optional).</param>
        public TestRegistrationContext(IAmbientServices? ambientServices = null)
            : base(ambientServices ?? Substitute.For<IAmbientServices>())
        {
        }
    }
}