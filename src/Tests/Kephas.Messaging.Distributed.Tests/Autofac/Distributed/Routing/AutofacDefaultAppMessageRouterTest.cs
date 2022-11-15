// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AutofacDefaultAppMessageRouterTest.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Messaging.Tests.Distributed.Routing;

using NUnit.Framework;

[TestFixture]
public class AutofacDefaultAppMessageRouterTest : DefaultAppMessageRouterTestBase
{
    protected override IServiceProvider BuildServiceProvider()
    {
        return this.CreateServicesBuilder().BuildWithAutofac();
    }
}