namespace Kephas.Graphs
{
    using System.Collections.Generic;

    using Kephas.Dynamic;

    /// <summary>
    /// Defines the contract for a graph node.
    /// </summary>
    public interface IGraphNode : IExpando
    {
        /// <summary>
        /// Gets the incoming nodes from this node.
        /// </summary>
        /// <value>
        /// The incoming edges.
        /// </value>
        IReadOnlyCollection<IGraphEdge> IncomingEdges { get; }

        /// <summary>
        /// Gets the outgoing edges from this node.
        /// </summary>
        /// <value>
        /// The outgoing edges.
        /// </value>
        IReadOnlyCollection<IGraphEdge> OutgoingEdges { get; }

        /// <summary>
        /// Gets the connected edges.
        /// </summary>
        /// <value>
        /// The connected edges.
        /// </value>
        IReadOnlyCollection<IGraphEdge> ConnectedEdges { get; }

        /// <summary>
        /// Gets the connected nodes.
        /// </summary>
        /// <value>
        /// The connected nodes.
        /// </value>
        IReadOnlyCollection<IGraphNode> ConnectedNodes { get; }
    }
}