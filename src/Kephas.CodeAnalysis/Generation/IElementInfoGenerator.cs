// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IElementInfoGenerator.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Declares the IElementInfoGenerator interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Generation
{
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;

    using Kephas.Reflection;
    using Kephas.Services;

    /// <summary>
    /// Interface for element information generator.
    /// </summary>
    [SingletonAppServiceContract]
    public interface IElementInfoGenerator
    {
        /// <summary>
        /// Determines whether the generator can handle the provided <see cref="IElementInfo"/>.
        /// </summary>
        /// <param name="elementInfo">The <see cref="IElementInfo"/> to be generated.</param>
        /// <param name="codeGenerationContext">Context for the code generation.</param>
        /// <returns>
        /// <c>true</c> if the generator can handle the <see cref="IElementInfo"/>, <c>false</c> if not.
        /// </returns>
        bool CanHandle(IElementInfo elementInfo, ICodeGenerationContext codeGenerationContext);

        /// <summary>
        /// Writes the generated code to the given text writer.
        /// </summary>
        /// <param name="text">The text builder.</param>
        /// <param name="elementInfo">The <see cref="IElementInfo"/> to be generated.</param>
        /// <param name="codeGenerationContext">Context for the code generation.</param>
        /// <param name="cancellationToken">The cancellation token (optional).</param>
        /// <returns>
        /// A promise of an enumeration of <see cref="ICodeGenerationUnit"/>s.
        /// </returns>
        Task GenerateCodeAsync(
            StringBuilder text,
            IElementInfo elementInfo,
            ICodeGenerationContext codeGenerationContext,
            CancellationToken cancellationToken = default);
    }
}