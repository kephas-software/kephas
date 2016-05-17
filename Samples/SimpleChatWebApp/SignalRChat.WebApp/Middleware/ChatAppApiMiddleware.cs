namespace SignalRChat.WebApp.Middleware
{
    using System;
    using System.Collections.Generic;
    using System.Net;
    using System.Threading;
    using System.Threading.Tasks;

    using Kephas.Messaging.Server;
    using Kephas.Serialization;
    using Kephas.Threading.Tasks;

    using Microsoft.Owin;

    using SignalRChat.WebApp.Messages;

    internal class ChatAppApiMiddleware
    {
        private readonly IMessageProcessor messageProcessor;

        private readonly ISerializer serializer;

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
        /// <param name="serializer">The serializer.</param>
        public ChatAppApiMiddleware(Func<IDictionary<string, object>, Task> next, IMessageProcessor messageProcessor, ISerializer serializer)
        {
            this.next = next;
            this.messageProcessor = messageProcessor;
            this.serializer = serializer;
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
                var msg = new PostMessage
                                {
                                    Name = "test",
                                    Message = context.Request.Uri.Query,
                                };

                var msgResponse = await this.messageProcessor.ProcessAsync(msg).WithServerThreadingContext();

                context.Response.StatusCode = (int)HttpStatusCode.OK;
                context.Response.ContentType = "application/json";
                await context.Response.WriteAsync("{ response: 'ok' }");

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