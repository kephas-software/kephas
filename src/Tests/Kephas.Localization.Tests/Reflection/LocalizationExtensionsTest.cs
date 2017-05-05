// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LocalizationExtensionsTest.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Defines the LocalizationExtensionsTest type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Tests.Reflection
{
    using System.Reflection;

    using Kephas.Reflection;

    using NUnit.Framework;

    [TestFixture]
    public class LocalizationExtensionsTest
    {
        [Test]
        public void GetLocalization_Type()
        {
            var localizableTypeInfo = typeof(LocalizableTestEntity);
            var localization = localizableTypeInfo.GetLocalization();
            Assert.IsNotNull(localization);
            Assert.AreEqual("LocalizableTestEntity-Name", localization.Name);
            Assert.AreEqual("LocalizableTestEntity-Description", localization.Description);
        }

        [Test]
        public void GetLocalization_TypeInfo()
        {
            var localizableTypeInfo = typeof(LocalizableTestEntity).GetTypeInfo();
            var localization = localizableTypeInfo.GetLocalization();
            Assert.IsNotNull(localization);
            Assert.AreEqual("LocalizableTestEntity-Name", localization.Name);
            Assert.AreEqual("LocalizableTestEntity-Description", localization.Description);
        }

        [Test]
        public void GetLocalization_ITypeInfo()
        {
            var localizableTypeInfo = typeof(LocalizableTestEntity).AsRuntimeTypeInfo();
            var localization = localizableTypeInfo.GetLocalization();
            Assert.IsNotNull(localization);
            Assert.AreEqual("LocalizableTestEntity-Name", localization.Name);
            Assert.AreEqual("LocalizableTestEntity-Description", localization.Description);
        }

        [Test]
        public void GetLocalization_PropertyInfo()
        {
            var localizableTypeInfo = typeof(LocalizableTestEntity);
            var propertyInfo = localizableTypeInfo.GetProperty("Id");
            var localization = propertyInfo.GetLocalization();
            Assert.IsNotNull(localization);
            Assert.AreEqual("Id-Name", localization.Name);
            Assert.AreEqual("Id-Description", localization.Description);
        }

        [Test]
        public void GetLocalization_IPropertyInfo()
        {
            var localizableTypeInfo = typeof(LocalizableTestEntity).AsRuntimeTypeInfo();
            var propertyInfo = localizableTypeInfo.Properties["Id"];
            var localization = propertyInfo.GetLocalization();
            Assert.IsNotNull(localization);
            Assert.AreEqual("Id-Name", localization.Name);
            Assert.AreEqual("Id-Description", localization.Description);
        }
    }
}