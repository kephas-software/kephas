// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RequestValidationPipelineBehaviorIntegrationTest.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using FluentAssertions;
using Kephas.Extensions.Mediator.TestHandlers;
using Kephas.Testing;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Xunit;
using ValidationException = Kephas.Extensions.Validation.ValidationException;

namespace Kephas.Extensions.Tests.Mediator.PipelineBehaviors;

public class RequestValidationPipelineBehaviorIntegrationTest
    : IntegrationTestBase<RequestValidationPipelineBehaviorIntegrationTest>
{
    private IMediator mediator = null!;

    public RequestValidationPipelineBehaviorIntegrationTest()
        : base()
    {
        Services.AddLogging();
    }

    public override async Task InitializeAsync()
    {
        await base.InitializeAsync();
        mediator = ServiceProvider.GetRequiredService<IMediator>();
    }

    [Fact]
    public async Task Handle_invalid()
    {
        await Assert.ThrowsAsync<ValidationException>(() => mediator.Send(new ValidatedLaunchRequest("")));
        await Assert.ThrowsAsync<ValidationException>(() => mediator.Send(new ValidatedLaunchRequest(null!)));
    }

    [Fact]
    public async Task Handle_valid()
    {
        // Arrange

        // Act
        var exception = await Record.ExceptionAsync(() => mediator.Send(new ValidatedLaunchRequest("good boy")));

        // Assert
        exception.Should().BeNull();
    }
}
