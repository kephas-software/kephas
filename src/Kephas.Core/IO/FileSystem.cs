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
    }
}
