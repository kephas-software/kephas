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
    using System.Runtime.InteropServices;

    /// <summary>
    /// Helper methods for interacting with the runtime environment.
    /// </summary>
    public static class RuntimeEnvironment
    {
        /// <summary>
        /// The library path environment variable.
        /// </summary>
        public static readonly string LibraryPathEnvVariable = "LD_LIBRARY_PATH";

        /// <summary>
        /// The Windows path separator.
        /// </summary>
        public static readonly char WindowsDirectorySeparatorChar = '\\';

        /// <summary>
        /// The Unix path separator.
        /// </summary>
        public static readonly char UnixDirectorySeparatorChar = '/';

        /// <summary>
        /// The Windows new line.
        /// </summary>
        public static readonly string WindowsNewLine = "\r\n";

        /// <summary>
        /// The Unix new line.
        /// </summary>
        public static readonly string UnixNewLine = "\n";

        private static PlatformID? platform;

        /// <summary>
        /// Gets the platform.
        /// </summary>
        public static PlatformID Platform => platform ?? (platform = ComputePlatform()).Value;

        /// <summary>
        /// Gets a value indicating whether the machine domain joined.
        /// </summary>
        /// <value>
        /// True if the machine is domain joined, false if not.
        /// </value>
        public static bool IsDomainJoinedMachine => !Environment.MachineName.Equals(Environment.UserDomainName, StringComparison.OrdinalIgnoreCase);

#if NET461
#else
        //
        // Do not use the " { get; } = <expression> " pattern here. Having all the initialization happen in the type initializer
        // means that one exception anywhere means all tests using PlatformDetection fail. If you feel a value is worth latching,
        // do it in a way that failures don't cascade.
        //

        /// <summary>
        /// Gets a value indicating whether the runtime is the full framework.
        /// </summary>
        /// <value>
        /// True if the runtime is the full framework, false if not.
        /// </value>
        public static bool IsNetFull => RuntimeInformation.FrameworkDescription.StartsWith(".NET Framework", StringComparison.OrdinalIgnoreCase);

        /// <summary>
        /// Gets a value indicating whether the runtime is the .NET native.
        /// </summary>
        /// <value>
        /// True if the runtime is the .NET native, false if not.
        /// </value>
        public static bool IsNetNative => RuntimeInformation.FrameworkDescription.StartsWith(".NET Native", StringComparison.OrdinalIgnoreCase);

        /// <summary>
        /// Gets a value indicating whether the runtime is the .NET Core.
        /// </summary>
        /// <value>
        /// True if the runtime is the .NET Core, false if not.
        /// </value>
        public static bool IsNetCore => RuntimeInformation.FrameworkDescription.StartsWith(".NET Core", StringComparison.OrdinalIgnoreCase);
#endif

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
