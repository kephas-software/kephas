// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AppServicesWithAutofacInjectionTest.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Data.Model.Tests.Models.AppServicesModel
{
    using System.Linq;
    using System.Threading.Tasks;

    using Kephas.Model;
    using Kephas.Model.Elements;
    using NUnit.Framework;

    [TestFixture]
    public class AppServicesWithAutofacInjectionTest : DataModelWithAutofacInjectionTestBase
    {
        [Test]
        public async Task InitializeAsync_data_app_service()
        {
            var container = this.CreateInjectorForModel(typeof(IDataContext));
            var provider = container.Resolve<IModelSpaceProvider>();

            await provider.InitializeAsync();

            var modelSpace = provider.GetModelSpace();
            var dataContextClassifier = modelSpace.Classifiers.Single(c => c.Name == "DataContext");

            Assert.IsInstanceOf<AppServiceType>(dataContextClassifier);
            Assert.AreEqual(1, dataContextClassifier.Parts.Count());
        }
    }
}