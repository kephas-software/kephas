// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TestRegistrationContext.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Implements the test registration context class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Testing.Core.Services.Composition
{
    using Kephas.Services;

    /// <summary>
    /// The test registration context.
    /// </summary>
    public class TestRegistrationContext : ContextBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TestRegistrationContext"/> class.
        /// </summary>
        /// <param name="ambientServices">The ambient services (optional).</param>
        public TestRegistrationContext(IAmbientServices ambientServices = null)
            : base(ambientServices)
        {
        }
    }
}