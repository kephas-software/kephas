// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AutofacDefaultTemplateProcessorTest.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Templating.Tests.Injection.Autofac
{
    using System.Threading.Tasks;
    using Kephas.Testing;
    using NUnit.Framework;

    [TestFixture]
    public class AutofacDefaultTemplateProcessorTest : DefaultTemplateProcessorTestBase
    {
        protected override IServiceProvider BuildServiceProvider(params Type[] parts)
        {
            return this.CreateServicesBuilder().WithParts(parts).BuildWithAutofac();
        }
    }
}