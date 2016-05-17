namespace SignalRChat.WebApp.Messages
{
    using Kephas.Messaging;

    /// <summary>
    /// A post message.
    /// </summary>
    public class PostMessage : IMessage
    {
        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>
        /// The name.
        /// </value>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the message.
        /// </summary>
        /// <value>
        /// The message.
        /// </value>
        public string Message { get; set; }
    }
}