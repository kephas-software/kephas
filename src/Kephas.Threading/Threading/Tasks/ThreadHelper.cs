// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ThreadHelper.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Threading.Tasks;

using System.Globalization;

/// <summary>
/// Helper methods for threads.
/// </summary>
public static class ThreadHelper
{
    /// <summary>
    /// Stores the current culture in the threading context.
    /// </summary>
    /// <param name="threadContext">Context for the server threading.</param>
    public static void StoreThreadCulture(this ThreadContext threadContext)
    {
        threadContext.CurrentCulture = CultureInfo.CurrentCulture;
        threadContext.CurrentUICulture = CultureInfo.CurrentUICulture;
    }

    /// <summary>
    /// Restores the current culture from the threading context.
    /// </summary>
    /// <param name="threadContext">Context for the server threading.</param>
    public static void RestoreThreadCulture(this ThreadContext threadContext)
    {
        if (threadContext.CurrentCulture != null)
        {
            CultureInfo.CurrentCulture = threadContext.CurrentCulture;
        }

        if (threadContext.CurrentUICulture != null)
        {
            CultureInfo.CurrentUICulture = threadContext.CurrentUICulture;
        }
    }
}