// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AddLicenseHandler.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Core.Endpoints
{
    using System;
    using System.IO;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;

    using Kephas.Application;
    using Kephas.ExceptionHandling;
    using Kephas.Logging;
    using Kephas.Messaging;
    using Kephas.Messaging.Messages;

    /// <summary>
    /// Handler for the <see cref="AddLicenseMessage"/>.
    /// </summary>
    public class AddLicenseHandler : MessageHandlerBase<AddLicenseMessage, ResponseMessage>
    {
        private readonly IAppRuntime appRuntime;

        /// <summary>
        /// Initializes a new instance of the <see cref="AddLicenseHandler"/> class.
        /// </summary>
        /// <param name="appRuntime">The application runtime.</param>
        public AddLicenseHandler(IAppRuntime appRuntime)
        {
            this.appRuntime = appRuntime;
        }

        /// <summary>
        /// Processes the provided message asynchronously and returns a response promise.
        /// </summary>
        /// <param name="message">The message to be handled.</param>
        /// <param name="context">The processing context.</param>
        /// <param name="token">The cancellation token.</param>
        /// <returns>
        /// The response promise.
        /// </returns>
        public override async Task<ResponseMessage?> ProcessAsync(AddLicenseMessage message, IMessagingContext context, CancellationToken token)
        {
            await Task.Yield();

            if (string.IsNullOrEmpty(message.Name))
            {
                throw new ArgumentNullException(nameof(message.Name), $"License name not provided.");
            }

            if (string.IsNullOrEmpty(message.Content))
            {
                throw new ArgumentNullException(nameof(message.Content), $"License content not provided.");
            }

            var licensesFolder = this.appRuntime.GetAppLicenseLocations().First();
            var fileName = Path.Combine(licensesFolder, message.Name);
            string? oldFileRename = null;
            if (File.Exists(fileName))
            {
                oldFileRename = $".{Guid.NewGuid():N}";
                File.Move(fileName, fileName + oldFileRename);
            }

            File.WriteAllText(fileName, message.Content);
            if (oldFileRename == null)
            {
                this.Logger.Info($"License saved to '{message.Name}' at the license location.");
                return new ResponseMessage
                {
                    Message = $"License saved to '{message.Name}' at the license location.",
                    Severity = SeverityLevel.Info,
                };
            }

            this.Logger.Warn($"License saved to '{message.Name}' at the license location. The existing license was renamed to '{message.Name + oldFileRename}'.");
            return new ResponseMessage
            {
                Message = $"License saved to '{message.Name}' at the license location. The existing license was renamed to '{message.Name + oldFileRename}'.",
                Severity = SeverityLevel.Warning,
            };
        }
    }
}
