// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ICodeConverter.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Declares the ICodeConverter interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Conversion
{
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;

    using Kephas.Reflection;

    /// <summary>
    /// Interface for code converter.
    /// </summary>
    public interface ICodeConverter
    {
        /// <summary>
        /// Converts the provided code elements to other code elements.
        /// </summary>
        /// <param name="codeElements">The code elements to convert.</param>
        /// <param name="codeConversionContext">The conversion context.</param>
        /// <param name="cancellationToken">The cancellation token (optional).</param>
        /// <returns>
        /// A promise of converted code elements.
        /// </returns>
        Task<IEnumerable<IElementInfo>> ConvertCodeAsync(
            IEnumerable<IElementInfo> codeElements,
            ICodeConversionContext codeConversionContext,
            CancellationToken cancellationToken = default(CancellationToken));
    }
}