// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FileSettingsProvider.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the file settings provider class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Configuration.Providers
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;

    using Kephas.Application;
    using Kephas.Diagnostics.Contracts;
    using Kephas.ExceptionHandling;
    using Kephas.Logging;
    using Kephas.Net.Mime;
    using Kephas.Net.Mime.Composition;
    using Kephas.Operations;
    using Kephas.Reflection;
    using Kephas.Serialization;
    using Kephas.Services;

    /// <summary>
    /// A file settings provider.
    /// </summary>
    [ProcessingPriority(Priority.BelowNormal)]
    public class FileSettingsProvider : Loggable, ISettingsProvider
    {
        private readonly IAppRuntime appRuntime;
        private readonly ISerializationService serializationService;
        private readonly ICollection<Lazy<IMediaType, MediaTypeMetadata>> mediaTypes;
        private ConcurrentDictionary<Type, (string filePath, IOperationResult result, Type mediaType)> filePaths =
            new ConcurrentDictionary<Type, (string filePath, IOperationResult result, Type mediaType)>();

        /// <summary>
        /// Initializes a new instance of the <see cref="FileSettingsProvider"/> class.
        /// </summary>
        /// <param name="appRuntime">The application runtime.</param>
        /// <param name="serializationService">The serialization service.</param>
        /// <param name="mediaTypes">List of supported media types.</param>
        public FileSettingsProvider(
            IAppRuntime appRuntime,
            ISerializationService serializationService,
            ICollection<Lazy<IMediaType, MediaTypeMetadata>> mediaTypes)
        {
            Requires.NotNull(appRuntime, nameof(appRuntime));
            Requires.NotNull(serializationService, nameof(serializationService));
            Requires.NotNull(mediaTypes, nameof(mediaTypes));

            this.appRuntime = appRuntime;
            this.serializationService = serializationService;
            this.mediaTypes = mediaTypes;
        }

        /// <summary>Gets the settings with the provided type.</summary>
        /// <param name="settingsType">Type of the settings.</param>
        /// <returns>The settings.</returns>
        public object GetSettings(Type settingsType)
        {
            var (filePath, result, mediaType) = this.filePaths.GetOrAdd(settingsType, _ => this.GetSettingsFilePath(settingsType));
            if (filePath == null)
            {
                this.Logger.Warn(result.Exceptions.First().Message);
                return null;
            }

            this.Logger.Debug(result.Messages.First().Message);
            var settingsContent = File.ReadAllText(filePath);
            var settings = this.serializationService.Deserialize(settingsContent, ctx => ctx.RootObjectType(settingsType).MediaType(mediaType));

            return settings;
        }

        private (string filePath, IOperationResult result, Type mediaType) GetSettingsFilePath(Type settingsType)
        {
            var result = new OperationResult();
            var settingsName = settingsType.Name.ToCamelCase();

            var appBinFolders = this.appRuntime.GetAppBinDirectories();
            var probingFolders = appBinFolders.Select(f => Path.Combine(f, ConfigurationHelper.ConfigFolder)).ToArray();
            var probingFiles = new List<string>();
            foreach (var mediaType in this.mediaTypes)
            {
                foreach (var fileExtension in mediaType.Metadata.SupportedFileExtensions ?? Enumerable.Empty<string>())
                {
                    var fileName = $"{settingsName}.{fileExtension}";
                    probingFiles.Add(fileName);
                    var filePath = probingFolders.Select(f => this.TryGetFilePath(f, fileName)).FirstOrDefault(f => f != null);

                    if (filePath != null)
                    {
                        result.MergeMessage($"Configuration file {fileName} found as '{filePath}'.");
                        return (filePath, result, mediaType.GetType());
                    }
                }
            }

            var probingFilesString = string.Join("', '", probingFiles);
            result.MergeException(
                new OperationException(
                    $"Configuration files: '{probingFilesString}' could not be found in any of the following folders: '{string.Join("', '", probingFolders)}'.")
                {
                    Severity = SeverityLevel.Warning,
                });

            return (null, result, null);
        }

        /// <summary>
        /// Try to get the file path under a specified root location.
        /// </summary>
        /// <remarks>
        /// This method will search also in the plugin folders.
        /// </remarks>
        /// <param name="rootLocation">The root location.</param>
        /// <param name="fileName">Filename of the file.</param>
        /// <returns>
        /// The file path.
        /// </returns>
        private string TryGetFilePath(string rootLocation, string fileName)
        {
            var filePath = Path.Combine(rootLocation, fileName);
            if (File.Exists(filePath))
            {
                return filePath;
            }

            if (!Directory.Exists(rootLocation))
            {
                return null;
            }

            var directoryInfo = new DirectoryInfo(rootLocation);
            var foundFiles = directoryInfo.EnumerateFiles(fileName, SearchOption.AllDirectories).Take(2).ToList();
            if (foundFiles.Count == 0)
            {
                return null;
            }

            if (foundFiles.Count == 1)
            {
                return foundFiles[0].FullName;
            }

            // TODO localization
            throw new FileLoadException($"Multiple files found for '{fileName}' under '{rootLocation}': '{string.Join("', '", foundFiles.Select(f => f.FullName))}'");
        }
    }
}
