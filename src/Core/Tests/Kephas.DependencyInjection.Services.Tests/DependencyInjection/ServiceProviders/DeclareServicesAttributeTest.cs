// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AppServicesAttributeTest.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using Kephas.DependencyInjection;
using Kephas.DependencyInjection.ServiceProviders;
using Xunit;

namespace Kephas.Tests.DependencyInjection.ServiceProviders;

public class DeclareServicesAttributeTest
{
    [Fact]
    public void Constructor_null_provider()
    {
        var attr = new DeclareServicesAttribute<NullServiceInfoProvider>();

        Assert.Empty(attr.GetServiceDeclarationInfos());
        Assert.Empty(attr.GetImplementationDeclarations([]));
    }

    [Fact]
    public void Constructor_infos_provider()
    {
        var attr = new DeclareServicesAttribute<TestServiceInfoProvider>();

        var serviceInfos = attr.GetServiceDeclarationInfos();
        Assert.Equal(new[] { typeof(TestServiceInfoProvider.IService) }, serviceInfos.Select(i => i.ServiceDeclarationType).ToArray());
        Assert.Empty(attr.GetImplementationDeclarations([]));
    }

    private class NullServiceInfoProvider : IServiceInfoProvider
    {
    }

    private class TestServiceInfoProvider : IServiceInfoProvider
    {
        /// <summary>
        /// Gets the service declaration types.
        /// </summary>
        /// <returns>
        /// The service declaration types.
        /// </returns>
        IEnumerable<ServiceDeclaration>? IServiceInfoProvider.GetServiceDeclarations()
        {
            yield return new ServiceDeclaration(typeof(IService), null);
        }

        [TransientService]
        public interface IService
        {
        }
    }
}