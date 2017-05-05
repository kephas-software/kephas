namespace Kephas.Tests.Reflection.Localization
{
    using Kephas.ComponentModel.DataAnnotations;
    using Kephas.Reflection;
    using Kephas.Reflection.Localization;

    using NUnit.Framework;

    [TestFixture]
    public class TypeInfoLocalizationTest
    {
        [Test]
        public void Name_from_attribute()
        {
            var localization = new TypeInfoLocalization(typeof(LocalizableTestEntity).AsRuntimeTypeInfo());
            Assert.AreEqual("LocalizableTestEntity-Name", localization.Name);
        }

        [Test]
        public void Description_from_attribute()
        {
            var localization = new TypeInfoLocalization(typeof(LocalizableTestEntity).AsRuntimeTypeInfo());
            Assert.AreEqual("LocalizableTestEntity-Description", localization.Description);
        }
    }
}