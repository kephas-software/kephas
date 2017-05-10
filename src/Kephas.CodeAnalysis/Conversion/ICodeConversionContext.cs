// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ICodeConversionContext.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Declares the ICodeConversionContext interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Conversion
{
    using Kephas.Services;

    /// <summary>
    /// Interface for code conversion context.
    /// </summary>
    public interface ICodeConversionContext : IContext
    {
        /// <summary>
        /// Gets the code converter.
        /// </summary>
        /// <value>
        /// The code converter.
        /// </value>
        ICodeConverter CodeConverter { get; }
    }
}