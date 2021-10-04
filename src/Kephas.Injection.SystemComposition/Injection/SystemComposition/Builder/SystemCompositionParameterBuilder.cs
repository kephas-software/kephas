﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SystemCompositionParameterBuilder.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   The MEF implementation of the conventions builder.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Injection.SystemComposition.Builder
{
    using System.Composition.Convention;
    using Kephas.Injection.Builder;

    /// <summary>
    /// The MEF implementation of the conventions builder.
    /// </summary>
    public class SystemCompositionParameterBuilder : IParameterBuilder
    {
        /// <summary>
        /// The inner builder.
        /// </summary>
        private readonly ImportConventionBuilder innerBuilder;

        /// <summary>
        /// Initializes a new instance of the <see cref="SystemCompositionParameterBuilder"/> class.
        /// </summary>
        /// <param name="innerBuilder">
        /// The inner builder.
        /// </param>
        internal SystemCompositionParameterBuilder(ImportConventionBuilder innerBuilder)
        {
            this.innerBuilder = innerBuilder;
        }
    }
}