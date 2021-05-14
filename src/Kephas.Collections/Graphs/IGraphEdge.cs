// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IGraphEdge.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Declares the IGraphEdge interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Graphs
{
    using Kephas.Dynamic;

    /// <summary>
    /// Defines the contract for a graph edge.
    /// </summary>
    public interface IGraphEdge : IExpando
    {
        /// <summary>
        /// Gets the node from which the edge starts.
        /// </summary>
        /// <value>
        /// The node from which the edge starts.
        /// </value>
        IGraphNode From { get; }

        /// <summary>
        /// Gets the node where the edge ends.
        /// </summary>
        /// <value>
        /// The node where the edge ends.
        /// </value>
        IGraphNode To { get; }
    }

    /// <summary>
    /// Defines the contract for a graph edge.
    /// </summary>
    /// <typeparam name="TNodeValue">Type of the node value.</typeparam>
    /// <typeparam name="TEdgeValue">The edge value.</typeparam>
    public interface IGraphEdge<TNodeValue, TEdgeValue> : IGraphEdge
    {
        /// <summary>
        /// Gets the node from which the edge starts.
        /// </summary>
        /// <value>
        /// The node from which the edge starts.
        /// </value>
        new IGraphNode<TNodeValue> From { get; }

        /// <summary>
        /// Gets the node where the edge ends.
        /// </summary>
        /// <value>
        /// The node where the edge ends.
        /// </value>
        new IGraphNode<TNodeValue> To { get; }

        /// <summary>
        /// Gets or sets the value of the edge.
        /// </summary>
        TEdgeValue? Value { get; set; }
    }
}