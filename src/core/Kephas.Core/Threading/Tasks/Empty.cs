// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Empty.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Static class providing empty tasks.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Threading.Tasks
{
    using System.Threading.Tasks;

    /// <summary>
    /// Static class providing empty tasks.
    /// </summary>
    /// <typeparam name="T">The result type.</typeparam>
    public static class Empty<T>
    {
        /// <summary>
        /// The empty task.
        /// </summary>
        public static readonly Task<T> Task = System.Threading.Tasks.Task.FromResult(default(T));
    }
}