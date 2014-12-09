using Kephas.Services;

namespace Kephas.Model.Factory
{
    /// <summary>
    /// Contract for providing a model space.
    /// </summary>
    [SharedAppServiceContract]
    public interface IModelSpaceProvider
    {
        /// <summary>
        /// Gets the model space.
        /// </summary>
        /// <returns>The model space.</returns>
        IModelSpace GetModelSpace();
    }
}