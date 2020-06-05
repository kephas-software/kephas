namespace Kephas.Core.Tests.Reflection.Localization
{
    using System.Linq;

    using Kephas.Reflection;
    using Kephas.Reflection.Localization;
    using Kephas.Runtime;
    using NUnit.Framework;

    [TestFixture]
    public class TypeInfoLocalizationTest
    {
        private readonly RuntimeTypeRegistry typeRegistry;

        public TypeInfoLocalizationTest()
        {
            this.typeRegistry = new RuntimeTypeRegistry();
        }

        [Test]
        public void Name_from_attribute()
        {
            var localization = new TypeInfoLocalization(typeof(LocalizableTestEntity).AsRuntimeTypeInfo(this.typeRegistry));
            Assert.AreEqual("LocalizableTestEntity-Name", localization.Name);
        }

        [Test]
        public void Description_from_attribute()
        {
            var localization = new TypeInfoLocalization(typeof(LocalizableTestEntity).AsRuntimeTypeInfo(this.typeRegistry));
            Assert.AreEqual("LocalizableTestEntity-Description", localization.Description);
        }

        [Test]
        public void Members_from_attribute()
        {
            var typeInfo = typeof(LocalizableTestEntity).AsRuntimeTypeInfo(this.typeRegistry);
            var localization = new TypeInfoLocalization(typeInfo);
            var members = localization.Members;
            Assert.AreEqual(typeInfo.Members.Count, members.Count);
            Assert.IsTrue(typeInfo.Members.All(m => members.ContainsKey(m.Key)));
        }

        [Test]
        public void Members_from_attribute_with_overloads()
        {
            var typeInfo = typeof(LocalizableTestEntityWithOverloads).AsRuntimeTypeInfo(this.typeRegistry);
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