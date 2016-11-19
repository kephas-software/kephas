﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IGraphDataContext.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Declares the IGraphDataContext interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Data.Behaviors
{
    /// <summary>
    /// Contract for graph operations contexts.
    /// </summary>
    public interface IGraphDataContext : IDataOperationContext
    {
        /// <summary>
        /// Gets or sets a value indicating whether the loose parts should be loaded.
        /// </summary>
        /// <value>
        /// <c>true</c> if loose parts should be loaded, <c>false</c> if not.
        /// </value>
        bool LoadLooseParts { get; set; }
    }
}