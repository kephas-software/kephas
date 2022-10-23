// --------------------------------------------------------------------------------------------------------------------
// <FlowActivityTestBase file="ActivityTestBase.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Workflow.Tests.Activities;

using Kephas.Dynamic;
using Kephas.Operations;
using Kephas.Testing;
using Kephas.Testing.Services;
using NSubstitute;

public abstract class FlowActivityTestBase  : TestBase
{
    protected IActivityContext CreateActivityContext(IDynamic? scope = null)
    {
        var context = Substitute.For<IActivityContext>();
        var processor = Substitute.For<IWorkflowProcessor>();
        context.WorkflowProcessor.Returns(processor);
        context.Scope.Returns(scope ?? new Expando());

        processor.ExecuteAsync(
                Arg.Any<IActivity>(),
                Arg.Any<object?>(),
                Arg.Any<IDynamic?>(),
                Arg.Any<Action<IActivityContext>?>(),
                Arg.Any<CancellationToken>())
            .Returns(ci => ci.Arg<IActivity>() is IOperation operation
                ? operation.ExecuteAsync(context, ci.Arg<CancellationToken>())
                : null);
        return context;
    }
}