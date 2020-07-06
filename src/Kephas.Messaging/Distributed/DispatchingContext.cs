// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DispatchingContext.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the dispatching context class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Messaging.Distributed
{
    using Kephas.Application;
    using Kephas.Composition;
    using Kephas.Configuration;
    using Kephas.Messaging.Distributed.Routing;
    using Kephas.Security.Authentication;
    using Kephas.Services;

    /// <summary>
    /// A dispatching context.
    /// </summary>
    public class DispatchingContext : Context, IDispatchingContext
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DispatchingContext"/> class.
        /// </summary>
        /// <param name="compositionContext">Context for the composition.</param>
        /// <param name="messagingConfig">The messaging configuration.</param>
        /// <param name="messageBroker">The message broker.</param>
        /// <param name="appRuntime">The application runtime.</param>
        /// <param name="authenticationService">The authentication service.</param>
        /// <param name="message">Optional. The message to be dispatched.</param>
        public DispatchingContext(ICompositionContext compositionContext, IConfiguration<MessagingSettings> messagingConfig, IMessageBroker messageBroker, IAppRuntime appRuntime, IAuthenticationService authenticationService, object? message = null)
            : base(compositionContext)
        {
            this.MessageBroker = messageBroker;
            this.AppRuntime = appRuntime;
            this.AuthenticationService = authenticationService;
            if (message is IBrokeredMessage brokeredMessage)
            {
                this.BrokeredMessage = brokeredMessage;
            }
            else
            {
                this.BrokeredMessage = message == null
                    ? new BrokeredMessage()
                    : new BrokeredMessage(message);
                this.BrokeredMessage.Timeout = messagingConfig.Settings?.Distributed?.DefaultTimeout ?? Distributed.BrokeredMessage.DefaultTimeout;
                this.BrokeredMessage.Sender = this.CreateAppInstanceEndpoint();
            }
        }

        /// <summary>
        /// Gets the message broker.
        /// </summary>
        /// <value>
        /// The message broker.
        /// </value>
        public IMessageBroker MessageBroker { get; }

        /// <summary>
        /// Gets the application runtime.
        /// </summary>
        /// <value>
        /// The application runtime.
        /// </value>
        public IAppRuntime AppRuntime { get; }

        /// <summary>
        /// Gets the authentication service.
        /// </summary>
        /// <value>
        /// The authentication service.
        /// </value>
        public IAuthenticationService AuthenticationService { get; }

        /// <summary>
        /// Gets or sets the brokered message.
        /// </summary>
        /// <value>
        /// The brokered message.
        /// </value>
        public IBrokeredMessage BrokeredMessage { get; protected set; }

        /// <summary>
        /// Gets or sets the message router which received the message to be dispatched.
        /// </summary>
        /// <value>
        /// The message router.
        /// </value>
        public IMessageRouter InputRouter { get; set; }

        /// <summary>
        /// Creates an endpoint for the current application instance.
        /// </summary>
        /// <param name="endpointId">Optional. Identifier for the endpoint.</param>
        /// <param name="scheme">Optional. The address scheme.</param>
        /// <returns>
        /// The new endpoint.
        /// </returns>
        public virtual IEndpoint CreateAppInstanceEndpoint(string? endpointId = null, string? scheme = null)
        {
            return Endpoint.CreateAppInstanceEndpoint(this.AppRuntime, endpointId, scheme: scheme);
        }

        /// <summary>
        /// Issues the <see cref="E:Kephas.Services.Context.IdentityChanged" /> event.
        /// </summary>
        protected override void OnIdentityChanged()
        {
            base.OnIdentityChanged();

            this.BrokeredMessage.BearerToken = this.GetBearerToken();
        }

        /// <summary>
        /// Gets the bearer token.
        /// </summary>
        /// <returns>
        /// The bearer token.
        /// </returns>
        protected virtual string GetBearerToken()
        {
            return this.AuthenticationService.GetToken(this.Identity, ctx => ctx.Impersonate(this))?.ToString();
        }
    }
}
