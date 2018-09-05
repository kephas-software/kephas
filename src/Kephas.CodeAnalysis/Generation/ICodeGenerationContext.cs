// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ICodeGenerationContext.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Declares the ICodeGenerationContext interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Generation
{
    using Kephas.Services;

    /// <summary>
    /// Interface for code generation context.
    /// </summary>
    public interface ICodeGenerationContext : IContext
    {
        /// <summary>
        /// Gets the code generator.
        /// </summary>
        /// <value>
        /// The code generator.
        /// </value>
        ICodeGenerator CodeGenerator { get; }

        /// <summary>
        /// Gets the code formatter.
        /// </summary>
        /// <value>
        /// The code formatter.
        /// </value>
        ICodeFormatter CodeFormatter { get; }
    }
}