// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IGraphNode.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Declares the IGraphNode interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

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

    /// <summary>
    /// Defines the contract for a graph node holding a value.
    /// </summary>
    /// <typeparam name="TValue">Type of the value.</typeparam>
    public interface IGraphNode<TValue> : IGraphNode
    {
        /// <summary>
        /// Gets or sets the node value.
        /// </summary>
        /// <value>
        /// The node value.
        /// </value>
        TValue Value { get; set; }
    }
}