namespace Kephas.Data.Model.Runtime
{
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;

    using Kephas.Model.Elements.Construction;
    using Kephas.Model.Factory;
    using Kephas.Model.Runtime.Construction.Builders;

    /// <summary>
    /// A model information provider for simple types.
    /// </summary>
    public class DataSimpleTypesModelInfoProvider : IModelInfoProvider
    {
        /// <summary>
        /// Gets the element infos used for building the model space.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>
        /// An awaitable task promising an enumeration of element information.
        /// </returns>
        public Task<IEnumerable<INamedElementInfo>> GetElementInfosAsync(CancellationToken cancellationToken = new CancellationToken())
        {
            var elementInfos = new List<INamedElementInfo>
                       {
                           new ValueTypeInfoBuilder(typeof(Id)).AsPrimitive().InCoreProjection().ElementInfo,
                       };

            return Task.FromResult((IEnumerable<INamedElementInfo>)elementInfos);
        }
    }
}