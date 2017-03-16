// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IExpando.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Contract for dynamic objects allowing getting or setting
//   properties by their name through an indexer.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Dynamic
{
    using System.Dynamic;

    /// <summary>
    /// Contract for dynamic objects allowing getting or setting
    /// properties by their name through an indexer.
    /// </summary>
    public interface IExpando : IDynamicMetaObjectProvider, IIndexable
    {
    }
}