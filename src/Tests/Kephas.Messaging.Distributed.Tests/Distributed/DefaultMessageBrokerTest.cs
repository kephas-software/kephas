// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DefaultMessageBrokerTest.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the in process message broker test class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Messaging.Tests.Distributed
{
    using System;

    using Kephas.Testing;
    using NUnit.Framework;

    [TestFixture]
    public class DefaultMessageBrokerTest : DefaultMessageBrokerTestBase
    {
        protected override IServiceProvider BuildServiceProvider(params Type[] parts)
        {
            return this.CreateServicesBuilder()
                .WithParts(parts)
                .BuildWithDependencyInjection();
        }
    }
}