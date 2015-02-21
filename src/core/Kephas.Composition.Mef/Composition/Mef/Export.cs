// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Export.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   A handle allowing the graph of parts associated with an exported instance
//   to be released.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Composition.Mef
{
    using System;

    /// <summary>
    /// A handle allowing the graph of parts associated with an exported instance
    /// to be released.
    /// </summary>
    /// <typeparam name="T">The export type.</typeparam>
    public sealed class Export<T> : IExport<T>
    {
        /// <summary>
        /// The inner export.
        /// </summary>
        private readonly System.Composition.Export<T> innerExport;

        /// <summary>
        /// Initializes a new instance of the <see cref="Export{T}" /> class.
        /// </summary>
        /// <param name="innerExport">The inner export.</param>
        internal Export(System.Composition.Export<T> innerExport)
        {
            this.innerExport = innerExport;
        }

        /// <summary>
        /// Gets the exported value.
        /// </summary>
        public T Value
        {
            get { return this.innerExport.Value; }
        }

        /// <summary>
        /// Release the parts associated with the exported value.
        /// </summary>
        public void Dispose()
        {
            this.innerExport.Dispose();
        }
    }
}