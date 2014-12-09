namespace Kephas.Model
{
    /// <summary>
    /// Contract for model projections.
    /// </summary>
    public interface IModelProjection : INamedElement
    {
        /// <summary>
        /// Gets a value indicating whether this projection is the result of aggregating one or more projections.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is aggregated; otherwise, <c>false</c>.
        /// </value>
        bool IsAggregated { get; }
    }
}