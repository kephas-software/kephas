namespace Kephas.Messaging.Pipes.Endpoints
{
    /// <summary>
    /// Message for registering a peer.
    /// </summary>
    public class UnregisterPeerMessage // : IMessage // do not register as message, as this is something internal for the pipes infrastructure
    {
        /// <summary>
        /// Gets or sets the caller application instance ID.
        /// </summary>
        public string? AppInstanceId { get; set; }

        /// <summary>
        /// Gets or sets the name of the input pipe.
        /// </summary>
        public string? InputPipeName { get; set; }

        /// <summary>
        /// Gets or sets the name of the server hosting the pipe.
        /// </summary>
        public string? ServerName { get; set; }
    }
}