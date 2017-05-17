// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IDataIOContext.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Declares the IDataIOContext interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Data.IO
{
    using System;

    using Kephas.Services;

    /// <summary>
    /// Interface for data i/o context.
    /// </summary>
    public interface IDataIOContext : IContext
    {
        /// <summary>
        /// Gets or sets the type of the root object.
        /// </summary>
        /// <value>
        /// The type of the root object.
        /// </value>
        Type RootObjectType { get; set; }
    }
}