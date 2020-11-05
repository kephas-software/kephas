// --------------------------------------------------------------------------------------------------------------------
// <copyright file="StateMachines.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Workflow.Model.Tests.Models.StateMachinesModel
{
    using Kephas.Workflow.AttributedModel;

    public interface IDocument
    {
        public DocumentState State { get; set; }
    }

    public enum DocumentState
    {
        Open,
        Closed,
    }

    public interface IDocumentStateMachine : IStateMachine<IDocument, DocumentState>
    {
        [Transition(DocumentState.Open, DocumentState.Closed)]
        void Close();
    }
}