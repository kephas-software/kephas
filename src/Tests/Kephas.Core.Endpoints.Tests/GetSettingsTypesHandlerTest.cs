using Kephas.Cryptography;
using Kephas.Data;

namespace Kephas.Core.Endpoints.Tests
{
    using System.Linq;
    using System.Collections.Generic;
    using System.Reflection;
    using System.Threading.Tasks;

    using Kephas.Application;
    using Kephas.Messaging;
    using Kephas.Reflection;
    using Kephas.Services;
    using NSubstitute;
    using NUnit.Framework;

    [TestFixture]
    public class GetSettingsTypesHandlerTest
    {
        [Test]
        public async Task ProcessAsync()
        {
            var appRuntime = Substitute.For<IAmbientServices>();
            appRuntime
                .GetAppAssemblies()
                .Returns(new[] { typeof(CoreSettings).Assembly });

            var handler = new GetSettingsTypesHandler(appRuntime, new DefaultTypeLoader());
            var result = await handler.ProcessAsync(
                new GetSettingsTypesMessage(),
                Substitute.For<IMessagingContext>(),
                default);

            CollectionAssert.IsSubsetOf(new[] { nameof(CoreSettings), nameof(IdGeneratorSettings), nameof(HashingSettings) }, result.SettingsTypes.Select(t => t.Name));
        }
    }
}