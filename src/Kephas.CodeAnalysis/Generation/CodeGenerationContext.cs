// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CodeGenerationContext.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the code generation context class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.CodeAnalysis.Generation
{
    using System;

    using Kephas.Services;
    using Kephas.Services;

    /// <summary>
    /// A code generation context.
    /// </summary>
    public class CodeGenerationContext : Context, ICodeGenerationContext
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CodeGenerationContext"/> class.
        /// </summary>
        /// <param name="serviceProvider">The injector.</param>
        /// <param name="codeGenerator">The code generator.</param>
        /// <param name="codeFormatter">Optional. The code formatter.</param>
        public CodeGenerationContext(
            IServiceProvider serviceProvider,
            ICodeGenerator codeGenerator,
            ICodeFormatter? codeFormatter = null)
            : base(serviceProvider)
        {
            codeGenerator = codeGenerator ?? throw new ArgumentNullException(nameof(codeGenerator));

            this.CodeGenerator = codeGenerator;
            this.CodeFormatter = codeFormatter ?? new CodeFormatter();
        }

        /// <summary>
        /// Gets the code generator.
        /// </summary>
        /// <value>
        /// The code generator.
        /// </value>
        public ICodeGenerator CodeGenerator { get; }

        /// <summary>
        /// Gets the code formatter.
        /// </summary>
        /// <value>
        /// The code formatter.
        /// </value>
        public ICodeFormatter CodeFormatter { get; }
    }
}