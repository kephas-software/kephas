// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AmbientServicesTest.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Test class for <see cref="AmbientServices" />.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Core.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;

    using Kephas;
    using Kephas.Composition;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using Telerik.JustMock;

    /// <summary>
    /// Test class for <see cref="AmbientServices"/>.
    /// </summary>
    [TestClass]
    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented", Justification = "Reviewed. Suppression is OK here.")]
    public class AmbientServicesTest
    {
        [TestMethod]
        [ExpectedException(typeof(Exception), AllowDerivedTypes = true)]
        public void CompositionContainer_cannot_set_null()
        {
            var ambientServices = new AmbientServices();
            ambientServices.CompositionContainer = null;
        }

        [TestMethod]
        public void CompositionContainer_works_fine_when_explicitely_set()
        {
            var ambientServices = new AmbientServices();
            var mockContainer = Mock.Create<ICompositionContainer>();
            ambientServices.CompositionContainer = mockContainer;
            var part = new List<object>();
            ambientServices.CompositionContainer.SatisfyImports(part);
        }
    }
}