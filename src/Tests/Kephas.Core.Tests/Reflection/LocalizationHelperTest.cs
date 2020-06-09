// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LocalizationHelperTest.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Defines the LocalizationHelperTest type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Core.Tests.Reflection
{
    using System.Reflection;

    using Kephas.Reflection;
    using Kephas.Reflection.Localization;
    using Kephas.Runtime;
    using NSubstitute;
    using NUnit.Framework;

    [TestFixture]
    public class LocalizationHelperTest
    {
        [Test]
        public void GetLocalization_Type()
        {
            var localizableTypeInfo = typeof(LocalizableTestEntity);
            var localization = LocalizationHelper.GetLocalization(localizableTypeInfo);
            Assert.IsNotNull(localization);
            Assert.AreEqual("LocalizableTestEntity-Name", localization.Name);
            Assert.AreEqual("LocalizableTestEntity-Description", localization.Description);
        }

        [Test]
        public void GetLocalization_TypeInfo()
        {
            var localizableTypeInfo = typeof(LocalizableTestEntity).GetTypeInfo();
            var localization = LocalizationHelper.GetLocalization(localizableTypeInfo);
            Assert.IsNotNull(localization);
            Assert.AreEqual("LocalizableTestEntity-Name", localization.Name);
            Assert.AreEqual("LocalizableTestEntity-Description", localization.Description);
        }

        [Test]
        public void GetLocalization_ITypeInfo()
        {
            var localizableTypeInfo = typeof(LocalizableTestEntity).AsRuntimeTypeInfo();
            var localization = LocalizationHelper.GetLocalization(localizableTypeInfo);
            Assert.IsNotNull(localization);
            Assert.AreEqual("LocalizableTestEntity-Name", localization.Name);
            Assert.AreEqual("LocalizableTestEntity-Description", localization.Description);
        }

        [Test]
        public void GetLocalization_ITypeInfo_same_if_called_twice()
        {
            var localizableTypeInfo = typeof(LocalizableTestEntity).AsRuntimeTypeInfo();
            var localization = LocalizationHelper.GetLocalization(localizableTypeInfo);
            var localization2 = LocalizationHelper.GetLocalization(localizableTypeInfo);
            Assert.AreSame(localization, localization2);
        }

        [Test]
        public void GetLocalization_PropertyInfo()
        {
            var localizableTypeInfo = typeof(LocalizableTestEntity);
            var propertyInfo = localizableTypeInfo.GetProperty("Id");
            var localization = LocalizationHelper.GetLocalization(propertyInfo);
            Assert.IsNotNull(localization);
            Assert.AreEqual("Id-Name", localization.Name);
            Assert.AreEqual("Id-Description", localization.Description);
        }

        [Test]
        public void GetLocalization_IPropertyInfo()
        {
            var localizableTypeInfo = typeof(LocalizableTestEntity).AsRuntimeTypeInfo();
            var propertyInfo = localizableTypeInfo.Properties["Id"];
            var localization = LocalizationHelper.GetLocalization(propertyInfo);
            Assert.IsNotNull(localization);
            Assert.AreEqual("Id-Name", localization.Name);
            Assert.AreEqual("Id-Description", localization.Description);
        }

        [Test]
        public void CreateTypeInfoLocalization()
        {
            var oldFunc = LocalizationHelper.CreateTypeInfoLocalization;
            var localization = Substitute.For<ITypeInfoLocalization>();
            LocalizationHelper.CreateTypeInfoLocalization = ti => localization;
            var typeLocalization = LocalizationHelper.GetLocalization(Substitute.For<ITypeInfo>());
            LocalizationHelper.CreateTypeInfoLocalization = oldFunc;

            Assert.AreSame(localization, typeLocalization);
        }
    }
}