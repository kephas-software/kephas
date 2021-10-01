// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IExpando.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Contract for dynamic objects allowing getting or setting
//   properties by their name through an indexer.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Dynamic
{
    using System;
    using System.Dynamic;

    /// <summary>
    /// Enumerates the flags for controlling the member binding through indexer or dynamic member access.
    /// </summary>
    [Flags]
    public enum ExpandoMemberBinderKind
    {
        /// <summary>
        /// No binders should be used.
        /// </summary>
        None = 0x00,

        /// <summary>
        /// Use only the binder for the inner dictionary.
        /// </summary>
        InnerDictionary = 0x01,

        /// <summary>
        /// Use only the binder for the inner object.
        /// </summary>
        InnerObject = 0x02,

        /// <summary>
        /// Use only the binder for the current expando object.
        /// </summary>
        This = 0x04,

        /// <summary>
        /// Use all member binders.
        /// </summary>
        All = InnerDictionary | InnerObject | This,
    }

    /// <summary>
    /// Contract for dynamic objects allowing getting or setting
    /// properties by their name through an indexer.
    /// </summary>
    public interface IExpando : IDynamicMetaObjectProvider, IExpandoBase
    {
    }
}