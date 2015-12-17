// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MefImportConventionsBuilder.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   The MEF implementation of the conventions builder.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Composition.Mef.Conventions
{
    using System.Composition.Convention;

    using Kephas.Composition.Conventions;

    /// <summary>
    /// The MEF implementation of the conventions builder.
    /// </summary>
    public class MefImportConventionsBuilder : IImportConventionsBuilder
    {
        /// <summary>
        /// The inner builder.
        /// </summary>
        private readonly ImportConventionBuilder innerBuilder;

        /// <summary>
        /// Initializes a new instance of the <see cref="MefImportConventionsBuilder"/> class.
        /// </summary>
        /// <param name="innerBuilder">
        /// The inner builder.
        /// </param>
        internal MefImportConventionsBuilder(ImportConventionBuilder innerBuilder)
        {
            this.innerBuilder = innerBuilder;
        }
    }
}