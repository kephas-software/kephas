// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TestRegistrationContext.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Implements the test registration context class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Core.Tests.Composition
{
    using Kephas.Composition.Hosting;

    using NSubstitute;

    /// <summary>
    /// The test registration context.
    /// </summary>
    public class TestRegistrationContext : CompositionRegistrationContext
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TestRegistrationContext"/> class.
        /// </summary>
        /// <param name="ambientServices">The ambient services (optional).</param>
        public TestRegistrationContext(IAmbientServices ambientServices = null)
            : base(ambientServices ?? Substitute.For<IAmbientServices>())
        {
        }
    }
}