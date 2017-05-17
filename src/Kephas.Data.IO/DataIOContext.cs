// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DataIOContext.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Implements the data i/o context class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Data.IO
{
    using System;

    using Kephas.Services;

    /// <summary>
    /// A data i/o context.
    /// </summary>
    public class DataIOContext : Context, IDataIOContext
    {
        /// <summary>
        /// Gets or sets the type of the root object.
        /// </summary>
        /// <value>
        /// The type of the root object.
        /// </value>
        public Type RootObjectType { get; set; }
    }
}