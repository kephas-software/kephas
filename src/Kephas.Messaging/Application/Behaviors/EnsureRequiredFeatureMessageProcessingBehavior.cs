// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EnsureRequiredFeatureMessageProcessingBehavior.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Messaging.Application.Behaviors;

using System.Collections.Concurrent;
using System.Reflection;
using Kephas.Application;
using Kephas.Collections;
using Kephas.Messaging.Behaviors;

/// <summary>
/// Behavior ensuring that the required features of the message being processed are enabled.
/// </summary>
public class EnsureRequiredFeatureMessageProcessingBehavior : MessagingBehaviorBase<IMessage>
{
    private readonly IAppRuntime appRuntime;
    private readonly ConcurrentDictionary<Type, IReadOnlyList<string>> featuresMap = new ();

    /// <summary>
    /// Initializes a new instance of the <see cref="EnsureRequiredFeatureMessageProcessingBehavior"/> class.
    /// </summary>
    /// <param name="appRuntime">The application runtime.</param>
    public EnsureRequiredFeatureMessageProcessingBehavior(IAppRuntime appRuntime)
    {
        this.appRuntime = appRuntime ?? throw new ArgumentNullException(nameof(appRuntime));
    }

    /// <summary>
    /// Interception called before invoking the handler to process the message.
    /// </summary>
    /// <param name="message">The message.</param>
    /// <param name="context">The processing context.</param>
    /// <param name="token">The cancellation token.</param>
    /// <returns>
    /// A task.
    /// </returns>
    public override Task BeforeProcessAsync(IMessage message, IMessagingContext context, CancellationToken token)
    {
        var messageType = message.GetType();

        var requiredFeatures = this.GetRequiredFeatures(messageType);
        if (requiredFeatures?.Count is > 0 && requiredFeatures.Any(f => !this.appRuntime.ContainsFeature(f)))
        {
            throw new MessagingException($"Message type '{messageType}' requires features '{requiredFeatures.JoinWith("', '")}' for successful processing. Check whether the features are installed and that they are enabled.");
        }

        return Task.CompletedTask;
    }

    /// <summary>
    /// Gets the required features.
    /// </summary>
    /// <param name="messageType">Type of the message.</param>
    /// <returns>
    /// The required features.
    /// </returns>
    private IReadOnlyList<string> GetRequiredFeatures(Type messageType)
    {
        var perms = this.featuresMap.GetOrAdd(messageType, this.ComputeFeatures);
        return perms;
    }

    private IReadOnlyList<string> ComputeFeatures(Type messageType)
    {
        var attrs = messageType.GetCustomAttributes<RequiresFeatureAttribute>(true);
        return new HashSet<string>(attrs.Select(a => a.Value)).ToList().AsReadOnly();
    }
}