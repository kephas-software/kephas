﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LogicalThreadContext.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the logical thread context class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Scheduling.Quartz.Threading
{
    using System.Collections.Concurrent;
    using System.Security;

    // Workaround for getting off remoting removed in NET Core: http://www.cazzulino.com/callcontext-netstandard-netcore.html
    using System.Threading;

    /// <summary>
    /// Wrapper class to access thread local data.
    /// Data is either accessed from thread or HTTP Context's 
    /// data if HTTP Context is available.
    /// </summary>
    /// <author>Marko Lahma .NET</author>
    public static class LogicalThreadContext
    {
        /// <summary>
        /// Retrieves an object with the specified name.
        /// </summary>
        /// <param name="name">The name of the item.</param>
        /// <returns>The object in the call context associated with the specified name or null if no object has been stored previously</returns>

        static ConcurrentDictionary<string, AsyncLocal<object>> state = new ConcurrentDictionary<string, AsyncLocal<object>>();

        /// <summary>
        /// Gets the object associated with the specified name.
        /// </summary>
        /// <typeparam name="T">The .</typeparam>
        /// <param name="name">The name with which to associate the new item.</param>
        /// <returns>
        /// The data.
        /// </returns>
        public static T GetData<T>(string name)
        {
            return state.TryGetValue(name, out AsyncLocal<object> data) ? (T)data.Value : default;
        }

        /// <summary>
        /// Stores a given object and associates it with the specified name.
        /// </summary>
        /// <param name="name">The name with which to associate the new item.</param>
        /// <param name="value">The object to store in the call context.</param>
        public static void SetData(string name, object value)
        {
            state.GetOrAdd(name, _ => new AsyncLocal<object>()).Value = value;
        }

        /// <summary>
        /// Empties a data slot with the specified name.
        /// </summary>
        /// <param name="name">The name of the data slot to empty.</param>
        public static void FreeNamedDataSlot(string name)
        {
            state.TryRemove(name, out AsyncLocal<object> discard);
        }
    }
}