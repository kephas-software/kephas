namespace Kephas.Model.Application
{
    using System.Diagnostics.Contracts;
    using System.Threading;
    using System.Threading.Tasks;

    using Kephas.Application;
    using Kephas.Services;

    /// <summary>
    /// Application initializer for the model space.
    /// </summary>
    [ProcessingPriority(Priority.High)]
    public class ModelSpaceAppInitializer : IAppInitializer
    {
        /// <summary>
        /// The model space provider.
        /// </summary>
        private readonly IModelSpaceProvider modelSpaceProvider;

        /// <summary>
        /// Initializes a new instance of the <see cref="ModelSpaceAppInitializer"/> class.
        /// </summary>
        /// <param name="modelSpaceProvider">The model space provider.</param>
        public ModelSpaceAppInitializer(IModelSpaceProvider modelSpaceProvider)
        {
            Contract.Requires(modelSpaceProvider != null);

            this.modelSpaceProvider = modelSpaceProvider;
        }

        /// <summary>
        /// Initializes the application asynchronously.
        /// </summary>
        /// <param name="appContext">Context for the application.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>
        /// A Task.
        /// </returns>
        public Task InitializeAsync(IAppContext appContext, CancellationToken cancellationToken = default(CancellationToken))
        {
            return this.modelSpaceProvider.InitializeAsync(appContext, cancellationToken);
        }
    }
}