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
    using System.Threading;
    using System.Threading.Tasks;

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
    using Kephas.Threading.Tasks;

    /// <summary>
    /// A file settings provider.
    /// </summary>
    [ProcessingPriority(Priority.BelowNormal)]
    public class FileSettingsProvider : Loggable, ISettingsProvider
    {
        private readonly ICollection<Lazy<IMediaType, MediaTypeMetadata>> mediaTypes;
        private readonly ConcurrentDictionary<Type, (string filePath, IOperationResult result, Type mediaType)> fileInfos =
            new ConcurrentDictionary<Type, (string filePath, IOperationResult result, Type mediaType)>();

        /// <summary>
        /// Initializes a new instance of the <see cref="FileSettingsProvider"/> class.
        /// </summary>
        /// <param name="appRuntime">The application runtime.</param>
        /// <param name="serializationService">The serialization service.</param>
        /// <param name="mediaTypes">List of supported media types.</param>
        /// <param name="logManager">Optional. Manager for log.</param>
        public FileSettingsProvider(
            IAppRuntime appRuntime,
            ISerializationService serializationService,
            ICollection<Lazy<IMediaType, MediaTypeMetadata>> mediaTypes,
            ILogManager? logManager = null)
            : base(logManager)
        {
            Requires.NotNull(appRuntime, nameof(appRuntime));
            Requires.NotNull(serializationService, nameof(serializationService));
            Requires.NotNull(mediaTypes, nameof(mediaTypes));

            this.AppRuntime = appRuntime;
            this.SerializationService = serializationService;
            this.mediaTypes = mediaTypes;
        }

        /// <summary>
        /// Gets the application runtime.
        /// </summary>
        /// <value>
        /// The application runtime.
        /// </value>
        protected IAppRuntime AppRuntime { get; }

        /// <summary>
        /// Gets the serialization service.
        /// </summary>
        /// <value>
        /// The serialization service.
        /// </value>
        protected ISerializationService SerializationService { get; }

        /// <summary>
        /// Gets the settings with the provided type.
        /// </summary>
        /// <param name="settingsType">Type of the settings.</param>
        /// <returns>The settings.</returns>
        public virtual object? GetSettings(Type settingsType)
        {
            var (filePath, result, mediaType) = this.GetSettingsFileInfo(settingsType);
            if (filePath == null)
            {
                this.Logger.Warn(result.Exceptions.First().Message);
                return null;
            }

            this.Logger.Debug(result.Messages.First().Message);
            var settingsContent = File.ReadAllText(filePath);
            var settings = this.SerializationService.Deserialize(settingsContent, ctx => ctx.RootObjectType(settingsType).MediaType(mediaType));

            return settings;
        }

        /// <summary>
        /// Updates the settings asynchronously.
        /// </summary>
        /// <param name="settings">The settings to be updated.</param>
        /// <param name="cancellationToken">Optional. The cancellation token.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        public virtual async Task UpdateSettingsAsync(object settings, CancellationToken cancellationToken = default)
        {
            Requires.NotNull(settings, nameof(settings));

            await Task.Yield();

            var settingsType = settings.GetType();
            var (filePath, result, mediaType) = this.GetSettingsFileInfo(settingsType);
            if (filePath == null)
            {
                this.Logger.Warn(result.Exceptions.First().Message);
                return;
            }

            this.Logger.Debug(result.Messages.First().Message);

            var settingsString = await this.SerializationService.SerializeAsync(
                settings,
                ctx => ctx
                    .Indent(true)
                    .MediaType(mediaType)
                    .IncludeTypeInfo(false),
                cancellationToken: cancellationToken)
                .PreserveThreadContext();
            File.WriteAllText(filePath, settingsString);

            this.Logger.Info("Settings '{settingsType}' updated.", settingsType);
        }

        /// <summary>
        /// Gets the settings file information.
        /// </summary>
        /// <param name="settingsType">Type of the settings.</param>
        /// <returns>
        /// The settings file path.
        /// </returns>
        protected virtual (string filePath, IOperationResult result, Type mediaType) GetSettingsFileInfo(Type settingsType)
        {
            return this.fileInfos.GetOrAdd(settingsType, _ => this.ComputeSettingsFileInfo(settingsType));
        }

        /// <summary>
        /// Gets the probing folders for configuration files.
        /// </summary>
        /// <returns>
        /// An enumerator that allows foreach to be used to process the probing folders in this
        /// collection.
        /// </returns>
        protected virtual IEnumerable<string> GetProbingFolders() => this.AppRuntime.GetAppConfigLocations();

        private (string filePath, IOperationResult result, Type mediaType) ComputeSettingsFileInfo(Type settingsType)
        {
            var result = new OperationResult();
            var settingsName = settingsType.Name.ToCamelCase();

            var probingFolders = this.GetProbingFolders();
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
                        return (filePath, result, mediaType.Value.GetType());
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
