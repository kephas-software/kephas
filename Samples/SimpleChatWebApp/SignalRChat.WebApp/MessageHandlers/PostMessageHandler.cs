namespace SignalRChat.WebApp.MessageHandlers
{
    using System.Threading;
    using System.Threading.Tasks;

    using Kephas.Messaging.Server;

    using Microsoft.AspNet.SignalR;

    using SignalRChat.WebApp.Messages;

    public class PostMessageHandler : MessageHandlerBase<PostMessage, EmptyMessage>
    {
        /// <summary>
        /// Processes the provided message asynchronously and returns a response promise.
        /// </summary>
        /// <param name="message">The message to be handled.</param>
        /// <param name="context">The processing context.</param>
        /// <param name="token">The cancellation token.</param>
        /// <returns>The response promise.</returns>
        public override Task<EmptyMessage> ProcessAsync(PostMessage message, IMessageProcessingContext context, CancellationToken token)
        {
            var hubContext = GlobalHost.ConnectionManager.GetHubContext<ChatHub>();
            hubContext.Clients.All.broadcastMessage(message.Name, message.Message);

            return Task.FromResult(new EmptyMessage());
        }
    }
}