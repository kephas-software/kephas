// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AutofacJsonSerializerTest.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Tests for <see cref="JsonSerializer" />.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Serialization.Json.Tests.Autofac
{
    using System.Diagnostics.CodeAnalysis;

    using Kephas.Serialization.Json;
    using Kephas.Services.Builder;
    using NUnit.Framework;

    /// <summary>
    /// Tests for <see cref="JsonSerializer"/>.
    /// </summary>
    [TestFixture]
    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented", Justification = "Reviewed. Suppression is OK here.")]
    public class AutofacJsonSerializerTest : JsonSerializerIntegrationTestBase
    {
        protected override IServiceProvider BuildServiceProvider(IAppServiceCollectionBuilder servicesBuilder)
        {
            return servicesBuilder.BuildWithAutofac();
        }
    }
}