namespace RoleGame.Services
{
    using System.Composition;

    public class GameManager : IGameManager
    {
        [ImportingConstructor]
        public GameManager(IUser user)
        {
            this.User = user;
        }

        public IUser User { get; }
    }
}