namespace SignalRChat.WebApp.Middleware
{
    using System;
    using System.Collections.Generic;
    using System.Net;
    using System.Threading.Tasks;

    using Kephas.Messaging;
    using Kephas.Serialization;
    using Kephas.Threading.Tasks;

    using Microsoft.Owin;

    using SignalRChat.WebApp.Messages;

    internal class ChatAppApiMiddleware
    {
        private readonly IMessageProcessor messageProcessor;

        private readonly ISerializationService serializationService;

        /// <summary>
        /// Gets the next middleware.
        /// </summary>
        /// <value>
        /// The next.
        /// </value>
        private readonly Func<IDictionary<string, object>, Task> next;

        /// <summary>
        /// Initializes a new instance of the <see cref="ChatAppApiMiddleware"/> class.
        /// </summary>
        /// <param name="next">The next.</param>
        /// <param name="messageProcessor">The message processor.</param>
        /// <param name="serializationService">The serializationService.</param>
        public ChatAppApiMiddleware(Func<IDictionary<string, object>, Task> next, IMessageProcessor messageProcessor, ISerializationService serializationService)
        {
            this.next = next;
            this.messageProcessor = messageProcessor;
            this.serializationService = serializationService;
        }

        /// <summary>
        /// Executes the given operation on a different thread, and waits for the result.
        /// </summary>
        /// <param name="env">The environment.</param>
        /// <returns>
        /// A Task.
        /// </returns>
        public async Task Invoke(IDictionary<string, object> env)
        {
            var context = new OwinContext(env);

            if (context.Request.Uri.LocalPath.ToLower().StartsWith("/post"))
            {
                var content = Uri.UnescapeDataString(context.Request.Uri.Query.Substring(1));
                var msg = (IMessage)await this.serializationService.JsonDeserializeAsync<PostMessage>(content).PreserveThreadContext();

                var msgResponse = await this.messageProcessor.ProcessAsync(msg).PreserveThreadContext();
                var msgJson = await this.serializationService.JsonSerializeAsync(msgResponse).PreserveThreadContext();

                context.Response.StatusCode = (int)HttpStatusCode.OK;
                context.Response.ContentType = "application/json";
                await context.Response.WriteAsync(msgJson);

                return;
            }

            try
            {
                await this.next(env);
            }
            catch (OperationCanceledException)
            {
                // be silent on cancelled.
            }
        }
    }
}