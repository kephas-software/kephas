namespace RoleGame.Services
{
    public class GameManager : IGameManager
    {
        public GameManager(IUser user)
        {
            this.User = user;
        }

        public IUser User { get; }
    }
}