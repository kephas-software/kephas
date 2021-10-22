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

        /// <summary>
        /// Gets the name of the .NET Framework runtime.
        /// </summary>
        public const string NetFrameworkRuntime = ".NET Framework";

        /// <summary>
        /// Gets the name of the .NET Core runtime.
        /// </summary>
        public const string NetCoreRuntime = ".NET Core";

        /// <summary>
        /// Gets the name of the .NET Native runtime.
        /// </summary>
        public const string NetNativeRuntime = ".NET Native";

        /// <summary>
        /// Gets the name of the .NET runtime.
        /// </summary>
        public const string NetRuntime = ".NET";

        /// <summary>
        /// Gets the name of the Mono runtime.
        /// </summary>
        public const string MonoRuntime = "Mono";

        private static PlatformID? platform;
        private static string? frameworkName;

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

        /*
         * Do not use the " { get; } = <expression> " pattern here. Having all the initialization happen in the type initializer
         * means that one exception anywhere means all tests using PlatformDetection fail. If you feel a value is worth latching,
         * do it in a way that failures don't cascade.
         */

        /// <summary>
        /// Gets a value indicating whether the runtime is the full framework.
        /// </summary>
        /// <value>
        /// True if the runtime is the full framework, false if not.
        /// </value>
        public static bool IsNetFramework => RuntimeInformation.FrameworkDescription.StartsWith(NetFrameworkRuntime, StringComparison.OrdinalIgnoreCase);

        /// <summary>
        /// Gets a value indicating whether the runtime is the .NET native.
        /// </summary>
        /// <value>
        /// True if the runtime is the .NET native, false if not.
        /// </value>
        public static bool IsNetNative => RuntimeInformation.FrameworkDescription.StartsWith(NetNativeRuntime, StringComparison.OrdinalIgnoreCase);

        /// <summary>
        /// Gets a value indicating whether the runtime is the .NET Core.
        /// </summary>
        /// <value>
        /// True if the runtime is the .NET Core, false if not.
        /// </value>
        public static bool IsNetCore => RuntimeInformation.FrameworkDescription.StartsWith(NetCoreRuntime, StringComparison.OrdinalIgnoreCase);

        /// <summary>
        /// Gets a value indicating whether the runtime is the unified .NET (>= 5).
        /// </summary>
        /// <value>
        /// True if the runtime is the unified .NET (>= 5), false if not.
        /// </value>
        public static bool IsNet =>
            RuntimeInformation.FrameworkDescription.StartsWith(NetRuntime, StringComparison.OrdinalIgnoreCase) &&
            "123456789".IndexOf(RuntimeInformation.FrameworkDescription[5]) > 0;

        /// <summary>
        /// Gets the name of the underlying framework runtime without the version. Check also the Net*Runtime constants.
        /// </summary>
        public static string FrameworkName =>
            frameworkName ??= IsNet
                ? NetRuntime
                : IsNetCore
                    ? NetCoreRuntime
                    : IsNetFramework
                        ? NetFrameworkRuntime
                        : IsNetNative
                            ? NetNativeRuntime
                            : IsMonoRuntime
                                ? MonoRuntime
                                : RuntimeInformation.FrameworkDescription;

        /// <summary>
        /// Gets a value indicating whether the application runs on the Mono runtime.
        /// </summary>
        /// <returns>
        /// True if the application runs on the Mono runtime, false if not.
        /// </returns>
        public static bool IsMonoRuntime => Type.GetType("Mono.Runtime") != null;

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
            if (IsMonoRuntime && platform == PlatformID.Unix)
            {
                var isMacOSProperty = typeof(Environment)
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
