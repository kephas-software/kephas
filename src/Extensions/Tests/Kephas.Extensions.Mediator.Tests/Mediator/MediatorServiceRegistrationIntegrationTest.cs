// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MediatorServiceRegistrationIntegrationTest.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using FluentAssertions;
using Kephas.Extensions.Mediator;
using Kephas.Testing;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Kephas.Extensions.Tests.Mediator;

public class MediatorServiceRegistrationIntegrationTest : IntegrationTestBase<MediatorServiceRegistrationIntegrationTest>
{
    [Fact]
    public void GetRequiredService()
    {
        var mediator = ServiceProvider.GetRequiredService<IMediator>();
        mediator.Should().BeOfType<MediatorService>();
    }
}