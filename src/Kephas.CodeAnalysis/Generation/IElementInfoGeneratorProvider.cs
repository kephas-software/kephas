// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IElementInfoGeneratorProvider.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
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
    [SharedAppServiceContract]
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