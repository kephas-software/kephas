// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DataContextCache.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Implements the data context cache class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Data.Caching
{
    using System.Collections.Generic;

    using Kephas.Data.Capabilities;

    /// <summary>
    /// A basic implementation of a data context cache.
    /// </summary>
    public class DataContextCache : Dictionary<Id, IEntityInfo>, IDataContextCache
    {
    }
}