// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IElementInfoGeneratorSelector.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Declares the IElementInfoGeneratorSelector interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.CodeAnalysis.Generation
{
    using Kephas.Reflection;
    using Kephas.Services;

    /// <summary>
    /// Service for selecting the appropriate <see cref="IElementInfoGenerator"/>.
    /// </summary>
    [SingletonAppServiceContract]
    public interface IElementInfoGeneratorSelector
    {
        /// <summary>
        /// Gets the <see cref="IElementInfoGenerator"/> for the provided <see cref="IElementInfo"/>.
        /// </summary>
        /// <param name="elementInfo">The <see cref="IElementInfo"/> to be generated.</param>
        /// <param name="codeGenerationContext">Context for the code generation.</param>
        /// <returns>
        /// The element info generator.
        /// </returns>
        IElementInfoGenerator GetElementInfoGenerator(IElementInfo elementInfo, ICodeGenerationContext codeGenerationContext);
    }
}