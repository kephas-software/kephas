// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RuntimeEnvironment.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the runtime environment class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Runtime
{
    using System;

    /// <summary>
    /// Helper methods for interacting with the runtime environment.
    /// </summary>
    public static class RuntimeEnvironment
    {
        /// <summary>
        /// The library path environment variable.
        /// </summary>
        public const string LibraryPathEnvVariable = "LD_LIBRARY_PATH";

        private static PlatformID? platform;

        /// <summary>
        /// Gets the platform.
        /// </summary>
        public static PlatformID Platform => platform ?? (platform = ComputePlatform()).Value;

        /// <summary>
        /// Indicates wheter the application runs on the Mono runtime.
        /// </summary>
        /// <returns>
        /// True if the application runs on the Mono runtime, false if not.
        /// </returns>
        public static bool IsMonoRuntime()
        {
            return Type.GetType("Mono.Runtime") != null;
        }

        /// <summary>
        /// Checks whether .NET is running on the Mono Platform by asking Environment.OSVersion.Platform. Can
        /// be overridden for testing purposes by setting AppEnvironment.Platform.
        /// </summary>
        /// <returns>
        /// True if the operating system is Unix like, false if not.
        /// </returns>
        public static bool IsUnix()
        {
            switch (Platform)
            {
                case PlatformID.Unix:
                case PlatformID.MacOSX:
                    return true;
                default:
                    return false;
            }
        }

        /// <summary>
        /// Checks whether .NET is running on the Mono Platform by asking Environment.OSVersion.Platform. Can
        /// be overridden for testing purposes by setting AppEnvironment.Platform.
        /// </summary>
        /// <returns>
        /// True if the operating system is Unix like, false if not.
        /// </returns>
        public static bool IsWindows()
        {
            switch (Platform)
            {
                case PlatformID.Win32NT:
                case PlatformID.Win32S:
                case PlatformID.Win32Windows:
                case PlatformID.WinCE:
                    return true;
                default:
                    return false;
            }
        }

        private static PlatformID ComputePlatform()
        {
            var platform = Environment.OSVersion.Platform;
            if (IsMonoRuntime() && platform == PlatformID.Unix)
            {
                var isMacOSProperty = typeof(System.Environment)
                    .GetProperty("IsMacOS", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static);
                if (isMacOSProperty != null)
                {
                    var isMacOS = (bool)isMacOSProperty.GetValue(null);
                    if (isMacOS)
                    {
                        return PlatformID.MacOSX;
                    }
                }
            }

            return platform;
        }
    }
}
