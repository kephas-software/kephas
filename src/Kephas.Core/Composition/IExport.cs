// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IExport.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Contract for a handle allowing the graph of parts associated with an exported instance
//   to be released.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Composition
{
    using System;

    /// <summary>
    /// Contract for a handle allowing the graph of parts associated with an exported instance
    /// to be released.
    /// </summary>
    /// <typeparam name="T">The contract type of the created parts.</typeparam>    
    public interface IExport<out T> : IDisposable
    {
        /// <summary>
        /// Gets the exported value.
        /// </summary>
        T Value { get; }
    }
}