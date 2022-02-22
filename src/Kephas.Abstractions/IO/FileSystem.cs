// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FileSystem.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the file system class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.IO
{
    using System;
    using System.IO;

    using Kephas.Runtime;

    /// <summary>
    /// A file system.
    /// </summary>
    public static class FileSystem
    {
        /// <summary>
        /// Prefix character for marking a hidden location.
        /// </summary>
        public const char HiddenLocationPrefixChar = '.';
        
        /// <summary>
        /// Copies the directory.
        /// </summary>
        /// <exception cref="DirectoryNotFoundException">Thrown when the requested directory is not
        ///                                              present.</exception>
        /// <param name="sourceDirName">Pathname of the source directory.</param>
        /// <param name="destDirName">Pathname of the destination directory.</param>
        /// <param name="copySubDirs">Optional. True to copy sub directories.</param>
        public static void CopyDirectory(string sourceDirName, string destDirName, bool copySubDirs = true)
        {
            // Get the subdirectories for the specified directory.
            DirectoryInfo dir = new DirectoryInfo(sourceDirName);

            if (!dir.Exists)
            {
                throw new DirectoryNotFoundException($"Source directory does not exist or could not be found: {sourceDirName}.");
            }

            var dirs = dir.GetDirectories();

            // If the destination directory doesn't exist, create it.
            if (!Directory.Exists(destDirName))
            {
                Directory.CreateDirectory(destDirName);
            }

            // Get the files in the directory and copy them to the new location.
            var files = dir.GetFiles();
            foreach (var file in files)
            {
                var temppath = Path.Combine(destDirName, file.Name);
                file.CopyTo(temppath, false);
            }

            // If copying subdirectories, copy them and their contents to new location.
            if (copySubDirs)
            {
                foreach (var subdir in dirs)
                {
                    var temppath = Path.Combine(destDirName, subdir.Name);
                    CopyDirectory(subdir.FullName, temppath, copySubDirs);
                }
            }
        }

        /// <summary>
        /// Indicates whether a location is hidden.
        /// </summary>
        /// <param name="location">The location to check.</param>
        /// <returns><c>true</c> if the location is hidden (starts with dot), <c>false</c> otherwise.</returns>
        public static bool IsHiddenLocation(this string location)
            => location.StartsWith(HiddenLocationPrefixChar);

        /// <summary>
        /// Makes the provided location (relative path) a hidden location.
        /// </summary>
        /// <param name="location">The raw location.</param>
        /// <returns>A location transformed to be hidden.</returns>
        public static string MakeHiddenLocation(this string location)
            => IsHiddenLocation(location) ? location : $"{HiddenLocationPrefixChar}{location}";

        /// <summary>
        /// Deletes the directory recursively, ensuring that the directory exists.
        /// </summary>
        /// <param name="directoryPath">Full pathname of the directory file.</param>
        public static void DeleteDirectory(string directoryPath)
        {
            if (Directory.Exists(directoryPath))
            {
                Directory.Delete(directoryPath, recursive: true);
            }
        }

        /// <summary>
        /// Normalizes the path, using proper path separator characters and expanding the environment variables if present.
        /// </summary>
        /// <param name="path">Full pathname of the file or directory.</param>
        /// <returns>
        /// The normalized path.
        /// </returns>
        public static string NormalizePath(string path)
        {
            if (string.IsNullOrEmpty(path))
            {
                return path;
            }

            path = path.Replace(RuntimeEnvironment.IsUnix() ? RuntimeEnvironment.WindowsDirectorySeparatorChar : RuntimeEnvironment.UnixDirectorySeparatorChar, Path.DirectorySeparatorChar);
            return Environment.ExpandEnvironmentVariables(path);
        }

        /// <summary>
        /// Gets the full path of the file or folder. If the name is a relative path, it will be made relative to the application location.
        /// </summary>
        /// <param name="path">Relative or absolute path of the file or folder.</param>
        /// <param name="rootPath">Optional. The root path.</param>
        /// <returns>
        /// The full path of the file or folder.
        /// </returns>
        public static string GetFullPath(string? path, string? rootPath = null)
        {
            string GetRootPath() => rootPath == null ? Directory.GetCurrentDirectory() : NormalizePath(rootPath);

            if (string.IsNullOrEmpty(path))
            {
                return GetRootPath();
            }

            path = NormalizePath(path);
            return Path.GetFullPath(Path.IsPathRooted(path) ? path : Path.Combine(GetRootPath(), path));
        }
    }
}
