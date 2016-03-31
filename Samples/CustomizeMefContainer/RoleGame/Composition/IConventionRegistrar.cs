namespace RoleGame.Composition
{
    using System.Composition.Convention;

    /// <summary>
    /// Registrar for composition conventions.
    /// </summary>
    public interface IConventionRegistrar
    {
        /// <summary>
        /// Registers the conventions.
        /// </summary>
        /// <param name="builder">The registration builder.</param>
        void RegisterConventions(ConventionBuilder builder);
    }
}