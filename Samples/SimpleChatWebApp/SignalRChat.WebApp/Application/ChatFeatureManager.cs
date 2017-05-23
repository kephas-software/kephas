namespace SignalRChat.WebApp.Application
{
    using System.Threading;
    using System.Threading.Tasks;

    using Kephas.Application;
    using Kephas.Messaging.Server;
    using Kephas.Serialization;
    using Kephas.Threading.Tasks;
    using Kephas.Web.Owin.Application;

    using Owin;

    using SignalRChat.WebApp.Middleware;

    [FeatureInfo(AppFeature.Chat, dependencies: new[] { AppFeature.SignalR })]
    public class ChatFeatureManager : OwinFeatureManagerBase
    {
        private readonly IMessageProcessor messageProcessor;

        private readonly ISerializationService serializationService;

        /// <summary>
        /// Initializes a new instance of the <see cref="ChatFeatureManager"/> class.
        /// </summary>
        /// <param name="messageProcessor">The message processor.</param>
        /// <param name="serializationService">The serialization service.</param>
        public ChatFeatureManager(IMessageProcessor messageProcessor, ISerializationService serializationService)
        {
            this.messageProcessor = messageProcessor;
            this.serializationService = serializationService;
        }

        /// <summary>Initializes the application asynchronously.</summary>
        /// <param name="appContext">Context for the application.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A Task.</returns>
        protected override Task InitializeCoreAsync(IOwinAppContext appContext, CancellationToken cancellationToken)
        {
            appContext.AppBuilder.Use<ChatAppApiMiddleware>(this.messageProcessor, this.serializationService);

            return TaskHelper.CompletedTask;
        }
    }
}