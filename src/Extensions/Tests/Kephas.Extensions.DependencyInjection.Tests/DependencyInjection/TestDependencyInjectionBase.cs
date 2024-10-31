// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TestDependencyInjectionBase.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using Kephas.DependencyInjection.Reflection;
using Kephas.Testing;

namespace Kephas.Extensions.Tests.DependencyInjection;

public abstract class TestDependencyInjectionBase
{
    internal static IServiceProvider BuildTestServiceProvider(IServiceInfoCollection serviceInfos)
        => Test.BuildEmptyTestServiceProvider(builderConfig: b => b.AddServiceInfos(serviceInfos.ToArray()));

    internal static IServiceProvider BuildTestServiceProvider(params Type[] types)
        => Test.BuildEmptyTestServiceProvider(builderConfig: b => b.AddTypes(types));
}