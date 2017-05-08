namespace Kephas.Tests.Reflection.Localization
{
    using System.Linq;

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

        [Test]
        public void Members_from_attribute()
        {
            var typeInfo = typeof(LocalizableTestEntity).AsRuntimeTypeInfo();
            var localization = new TypeInfoLocalization(typeInfo);
            var members = localization.Members;
            Assert.AreEqual(typeInfo.Members.Count, members.Count);
            Assert.IsTrue(typeInfo.Members.All(m => members.ContainsKey(m.Key)));
        }

        [Test]
        public void Members_from_attribute_with_overloads()
        {
            var typeInfo = typeof(LocalizableTestEntityWithOverloads).AsRuntimeTypeInfo();
            var localization = new TypeInfoLocalization(typeInfo);
            var members = localization.Members;
            Assert.IsTrue(typeInfo.Members.All(m => members.ContainsKey(m.Value.Name)));
            var overloadedMember = members[nameof(LocalizableTestEntityWithOverloads.Id)];
            Assert.AreEqual("New-Id-Name", overloadedMember.Name);
            Assert.AreEqual("New-Id-ShortName", overloadedMember.ShortName);
            Assert.AreEqual("New-Id-Description", overloadedMember.Description);
            Assert.AreEqual("New-Id-Prompt", overloadedMember.Prompt);
        }
    }
}