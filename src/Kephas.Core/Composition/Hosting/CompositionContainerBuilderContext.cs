// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CompositionContainerBuilderContext.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Implements the composition container builder context class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Composition.Hosting
{
    using System.Diagnostics.Contracts;

    using Kephas.Diagnostics.Contracts;
    using Kephas.Services;

    /// <summary>
    /// A composition container builder context.
    /// </summary>
    public class CompositionContainerBuilderContext : ContextBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CompositionContainerBuilderContext"/> class.
        /// </summary>
        /// <param name="ambientServices">The ambient services.</param>
        public CompositionContainerBuilderContext(IAmbientServices ambientServices)
            : base(ambientServices)
        {
            Requires.NotNull(ambientServices, nameof(ambientServices));
        }
    }
}