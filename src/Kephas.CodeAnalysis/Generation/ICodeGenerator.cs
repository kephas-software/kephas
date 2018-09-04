// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ICodeGenerator.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Declares the ICodeGenerator interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Generation
{
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;

    using Kephas.Reflection;

    /// <summary>
    /// Interface for code generator.
    /// </summary>
    public interface ICodeGenerator
    {
        /// <summary>
        /// Writes the generated code to the given text writer.
        /// </summary>
        /// <param name="codeElements">The <see cref="IElementInfo"/>s to be generated.</param>
        /// <param name="codeGenerationContext">Context for the code generation.</param>
        /// <param name="cancellationToken">The cancellation token (optional).</param>
        /// <returns>
        /// A promise of an enumeration of <see cref="ICodeGenerationUnit"/>s.
        /// </returns>
        Task<IEnumerable<ICodeGenerationUnit>> GenerateCodeAsync(
            IEnumerable<IElementInfo> codeElements,
            ICodeGenerationContext codeGenerationContext,
            CancellationToken cancellationToken = default);
    }
}