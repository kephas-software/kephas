// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IElementInfoGeneratorProvider.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Declares the IElementInfoGeneratorProvider interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Generation
{
    using Kephas.Reflection;
    using Kephas.Services;

    /// <summary>
    /// Interface for element information generator provider.
    /// </summary>
    [SingletonAppServiceContract]
    public interface IElementInfoGeneratorProvider
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