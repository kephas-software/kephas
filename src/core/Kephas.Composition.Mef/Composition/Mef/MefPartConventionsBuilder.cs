// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MefPartConventionsBuilder.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Conventions builder for a specific part.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Composition.Mef
{
    using System;
    using System.Composition.Convention;
    using System.Diagnostics.Contracts;

    using Kephas.Composition.Conventions;

    /// <summary>
    /// Conventions builder for a specific part.
    /// </summary>
    public class MefPartConventionsBuilder : IPartConventionsBuilder
    {
        /// <summary>
        /// The inner convention builder.
        /// </summary>
        private readonly PartConventionBuilder innerConventionBuilder;

        /// <summary>
        /// Initializes a new instance of the <see cref="MefPartConventionsBuilder"/> class.
        /// </summary>
        /// <param name="innerConventionBuilder">The inner convention builder.</param>
        internal MefPartConventionsBuilder(PartConventionBuilder innerConventionBuilder)
        {
            Contract.Requires(innerConventionBuilder != null);

            this.innerConventionBuilder = innerConventionBuilder;
        }
        
        /// <summary>
        /// Mark the part as being shared within the entire composition.
        /// </summary>
        /// <returns>A part builder allowing further configuration of the part.</returns>
        public IPartConventionsBuilder Shared()
        {
            this.innerConventionBuilder.Shared();
            return this;
        }

        /// <summary>
        /// Exports the part using the specified conventions builder.
        /// </summary>
        /// <param name="conventionsBuilder">The conventions builder.</param>
        /// <returns>A part builder allowing further configuration of the part.</returns>
        public IPartConventionsBuilder Export(Action<IExportConventionsBuilder> conventionsBuilder = null)
        {
            if (conventionsBuilder == null)
            {
                this.innerConventionBuilder.Export();
            }
            else
            {
                this.innerConventionBuilder.Export(b => conventionsBuilder(new MefExportConventionsBuilder(b)));
            }

            return this;
        }
    }
}