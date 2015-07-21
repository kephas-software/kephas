// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TaskHelper.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Helper class for tasks.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Threading.Tasks
{
    using System.Threading.Tasks;

    /// <summary>
    /// Helper class for tasks.
    /// </summary>
    public static class TaskHelper
    {
        /// <summary>
        /// Gets a resolved task returning the default value of <typeparamref name="T"/>.
        /// </summary>
        /// <typeparam name="T">The result type.</typeparam>
        public static Task<T> EmptyTask<T>() => Task.FromResult(default(T));
    }
}