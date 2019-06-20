// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ILLBLGenDataContext.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Implements the illbl generate data context class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Data.LLBLGen
{
    using Kephas.Data;

    using SD.LLBLGen.Pro.ORMSupportClasses;

    /// <summary>
    /// LLBLGen specialization of an <see cref="IDataContext"/>.
    /// </summary>
    public interface ILLBLGenDataContext : IDataContext
    {
        /// <summary>
        /// Gets the data access adapter.
        /// </summary>
        /// <value>
        /// The data access adapter.
        /// </value>
        DataAccessAdapterBase DataAccessAdapter { get; }
    }
}