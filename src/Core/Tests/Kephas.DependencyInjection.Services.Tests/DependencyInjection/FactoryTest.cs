// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ExportFactoryExtensionsTest.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the export factory extensions test class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using Kephas.DependencyInjection;
using NSubstitute;
using Xunit;

namespace Kephas.Tests.DependencyInjection;

public class FactoryTest
{
    [Fact]
    public void Create()
    {
        var service = Substitute.For<ITestService>();
        var factory = new Factory<ITestService>(() => service);

        var exportedValue = factory.GetService();

        Assert.Same(service, exportedValue);
    }

    public interface ITestService {}
}