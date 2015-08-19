namespace Kephas.Data.Tests.Model.Runtime
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using Kephas.Composition;
    using Kephas.Data.Model.Runtime;
    using Kephas.Model.Runtime.Construction;
    using Kephas.Model.Runtime.Factory;

    using NUnit.Framework;

    using Telerik.JustMock;

    [TestFixture]
    public class DataSimpleTypesModelInfoProviderTest
    {
        [Test]
        public async Task GetElementInfosAsync()
        {
            var provider = new DataSimpleTypesModelInfoProvider(Mock.Create<IRuntimeModelInfoFactory>());
            var elementInfos = (await provider.GetElementInfosAsync()).Cast<RuntimeValueTypeInfo>().ToList();

            Assert.IsTrue(elementInfos.Any(info => info.RuntimeElement == typeof(Id) && info.IsPrimitive));
        }
    }
}