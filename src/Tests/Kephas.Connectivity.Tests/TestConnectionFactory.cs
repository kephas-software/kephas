// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TestConnectionFactory.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using Kephas.Connectivity.AttributedModel;
using NSubstitute;

namespace Kephas.Connectivity.Tests;

[ConnectionKind("test")]
public class TestConnectionFactory : IConnectionFactory
{
    public IConnection CreateConnection(IConnectionContext context)
    {
        return context["connection"] as IConnection ?? Substitute.For<IConnection>();
    }
}
