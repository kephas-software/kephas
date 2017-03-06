namespace Kephas.Core.Tests.Reflection
{
    using Kephas.Reflection;

    using NSubstitute;

    using NUnit.Framework;

    [TestFixture]
    public class DefaultTypeResolverTest
    {
        [Test]
        public void ResolveType_DotNet_Native()
        {
            var loader = Substitute.For<IAssemblyLoader>();
            var resolver = new DefaultTypeResolver(loader);
            var type = resolver.ResolveType("System.String");
            Assert.AreEqual(typeof(string), type);
        }

        [Test]
        public void ResolveType_AssemblyQualifiedName()
        {
            var loader = Substitute.For<IAssemblyLoader>();
            var resolver = new DefaultTypeResolver(loader);
            var type = resolver.ResolveType(this.GetType().AssemblyQualifiedName);
            Assert.AreEqual(this.GetType(), type);
        }
    }
}