// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AutofacDefaultMessageProcessorTest.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Test class for <see cref="DefaultMessageProcessor" />.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Messaging.Tests.Autofac
{
    using System;
    using System.Diagnostics.CodeAnalysis;

    using NUnit.Framework;

    /// <summary>
    /// Test class for <see cref="DefaultMessageProcessor"/>.
    /// </summary>
    [TestFixture]
    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented", Justification = "Reviewed. Suppression is OK here.")]
    public class AutofacDefaultMessageProcessorTest : DefaultMessageProcessorTestBase
    {
        protected override IServiceProvider BuildServiceProvider()
        {
            return this.CreateServicesBuilder().BuildWithAutofac();
        }
    }
}