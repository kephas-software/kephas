﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DefaultConnectionProviderTest.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Connectivity.Tests;

using Kephas.Testing;
using NUnit.Framework;

[TestFixture]
public class DefaultConnectionProviderTest : DefaultConnectionProviderTestBase
{
    protected override IServiceProvider BuildServiceProvider(params Type[] parts)
    {
        return this.CreateServicesBuilder().WithParts(parts).BuildWithDependencyInjection();
    }
}