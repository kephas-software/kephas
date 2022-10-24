// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AmbientServicesTest.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Test class for <see cref="AmbientServices" />.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Tests
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using Kephas;
    using NUnit.Framework;

    /// <summary>
    /// Test class for <see cref="AmbientServices"/>.
    /// </summary>
    [TestFixture]
    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented", Justification = "Reviewed. Suppression is OK here.")]
    public class AmbientServicesTest : AmbientServicesTestBase
    {
        protected override IServiceProvider BuildServiceProvider(IAmbientServices ambientServices)
        {
            return this.CreateServicesBuilder(ambientServices).BuildWithDependencyInjection();
        }
    }
}