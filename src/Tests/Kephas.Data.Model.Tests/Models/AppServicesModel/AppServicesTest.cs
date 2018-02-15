// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AppServicesTest.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Implements the application services test class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Data.Model.Tests.Models.AppServicesModel
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;

    using Kephas.Model;
    using Kephas.Model.Elements;
    using Kephas.Model.Tests;

    using NUnit.Framework;

    [TestFixture]
    public class AppServicesTest : ModelTestBase
    {
        [Test]
        public async Task InitializeAsync_data_app_service()
        {
            var container = this.CreateContainerForModel();
            var provider = container.GetExport<IModelSpaceProvider>();

            await provider.InitializeAsync();

            var modelSpace = provider.GetModelSpace();
            var dataContextClassifier = modelSpace.Classifiers.Single(c => c.Name == "DataContext");

            Assert.IsInstanceOf<AppService>(dataContextClassifier);
            Assert.AreEqual(1, dataContextClassifier.Parts.Count());
        }
    }
}