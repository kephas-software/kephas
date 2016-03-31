namespace RoleGame.Composition.ScopeFactory
{
    using System.Composition;

    using Kephas.Composition.Mef.ScopeFactory;

    /// <summary>
    /// A MEF scope provider.
    /// </summary>
    [SharingBoundaryScope(ScopeNames.User)]
    public class UserMefScopeFactory : MefScopeFactoryBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UserMefScopeFactory"/> class.
        /// </summary>
        /// <param name="scopedContextFactory">The scoped context factory.</param>
        [ImportingConstructor]
        public UserMefScopeFactory([SharingBoundary(ScopeNames.User)] ExportFactory<CompositionContext> scopedContextFactory)
            : base(scopedContextFactory)
        {
        }
    }
}