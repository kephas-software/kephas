// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CodeConversionContext.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the code conversion context class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.CodeAnalysis.Conversion
{
    using System;

    using Kephas.Injection;
    using Kephas.Services;

    /// <summary>
    /// A code conversion context.
    /// </summary>
    public class CodeConversionContext : Context, ICodeConversionContext
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CodeConversionContext"/> class.
        /// </summary>
        /// <param name="injector">The injector.</param>
        /// <param name="codeConverter">The code converter.</param>
        public CodeConversionContext(
            IInjector injector,
            ICodeConverter codeConverter)
            : base(injector)
        {
            codeConverter = codeConverter ?? throw new ArgumentNullException(nameof(codeConverter));

            this.CodeConverter = codeConverter;
        }

        /// <summary>
        /// Gets the code converter.
        /// </summary>
        /// <value>
        /// The code converter.
        /// </value>
        public ICodeConverter CodeConverter { get; }
    }
}