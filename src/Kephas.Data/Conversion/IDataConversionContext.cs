// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IDataConversionContext.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Declares the IDataConversionContext interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Data.Conversion
{
    using Kephas.Services;

    /// <summary>
    /// Contract interface for data conversion contexts.
    /// </summary>
    public interface IDataConversionContext : IContext
    {
        /// <summary>
        /// Gets a value indicating whether to throw an exception when an error occurs.
        /// </summary>
        /// <value>
        /// <c>true</c> to throw an error on exceptions, <c>false</c> if not.
        /// </value>
        bool ThrowOnError { get; }

        /// <summary>
        /// Gets a <see cref="IDataContext"/> for the source object.
        /// </summary>
        /// <value>
        /// The source data context.
        /// </value>
        IDataContext SourceDataContext { get; }

        /// <summary>
        /// Gets a <see cref="IDataContext"/> for the target object.
        /// </summary>
        /// <value>
        /// The target data context.
        /// </value>
        IDataContext TargetDataContext { get; }
    }
}