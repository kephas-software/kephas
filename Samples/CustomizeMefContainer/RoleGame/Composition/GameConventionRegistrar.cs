namespace RoleGame.Composition
{
    using System.Composition.Convention;

    using RoleGame.Services;

    public class GameConventionRegistrar : IConventionRegistrar
    {
        /// <summary>
        /// Registers the conventions.
        /// </summary>
        /// <param name="builder">The registration builder.</param>
        public void RegisterConventions(ConventionBuilder builder)
        {
            builder
                .ForTypesDerivedFrom<IGameManager>()
                .Export(b => b.AsContractType<IGameManager>())
                .Shared(ScopeNames.User);

            builder
                .ForTypesDerivedFrom<IUser>()
                .Export(b => b.AsContractType<IUser>())
                .Shared(ScopeNames.User);
        }
    }
}