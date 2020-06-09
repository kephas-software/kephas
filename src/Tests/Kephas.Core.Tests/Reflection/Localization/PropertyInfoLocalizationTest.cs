namespace Kephas.Core.Tests.Reflection.Localization
{
    using Kephas.Reflection;
    using Kephas.Reflection.Localization;
    using Kephas.Runtime;
    using NUnit.Framework;

    [TestFixture]
    public class PropertyInfoLocalizationTest
    {
        [Test]
        public void Name_from_attribute()
        {
            var localization = new MemberInfoLocalization(typeof(LocalizableTestEntity).AsRuntimeTypeInfo().Properties["Id"]);
            Assert.AreEqual("Id-Name", localization.Name);
        }

        [Test]
        public void ShortName_from_attribute()
        {
            var localization = new MemberInfoLocalization(typeof(LocalizableTestEntity).AsRuntimeTypeInfo().Properties["Id"]);
            Assert.AreEqual("Id-ShortName", localization.ShortName);
        }

        [Test]
        public void Prompt_from_attribute()
        {
            var localization = new MemberInfoLocalization(typeof(LocalizableTestEntity).AsRuntimeTypeInfo().Properties["Id"]);
            Assert.AreEqual("Id-Prompt", localization.Prompt);
        }

        [Test]
        public void Description_from_attribute()
        {
            var localization = new MemberInfoLocalization(typeof(LocalizableTestEntity).AsRuntimeTypeInfo().Properties["Id"]);
            Assert.AreEqual("Id-Description", localization.Description);
        }
    }
}