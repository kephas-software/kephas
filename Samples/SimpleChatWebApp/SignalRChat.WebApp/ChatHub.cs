namespace SignalRChat.WebApp
{
    using Microsoft.AspNet.SignalR;

    public class ChatHub : Hub
    {
        public void Send(string name, string message)
        {
            // Call the broadcastMessage method to update clients.
            this.Clients.All.broadcastMessage(name, message);
        }
    }
}