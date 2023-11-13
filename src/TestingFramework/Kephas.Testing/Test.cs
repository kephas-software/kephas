// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Test.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System.Text;
using Kephas.Testing.Logging;
using Microsoft.Extensions.Logging;

namespace Kephas.Testing
{
    /// <summary>
    /// Provides testing primitives.
    /// </summary>
    public static class Test
    {
        /// <summary>
        /// Gets the test <see cref="ILogger{T}"/>.
        /// </summary>
        /// <returns>The test logger.</returns>
        public static ILogger<T> GetTestLogger<T>(LogLevel? minimumLevel = null) => new TestLogger<T>(minimumLevel: minimumLevel);

        /// <summary>
        /// Gets the test <see cref="ILogger{T}"/>.
        /// </summary>
        /// <returns>The test logger.</returns>
        public static ILogger<T> GetTestLogger<T>(StringBuilder? stringBuilder, LogLevel? minimumLevel = null) => new TestLogger<T>(stringBuilder, minimumLevel: minimumLevel);
    }
}
