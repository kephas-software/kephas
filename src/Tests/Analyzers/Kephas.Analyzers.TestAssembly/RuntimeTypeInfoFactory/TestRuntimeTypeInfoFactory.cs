namespace Kephas.Analyzers.TestAssembly.RuntimeTypeInfoFactory
{
    using System;
    using System.Reflection;
    using Kephas.Logging;
    using Kephas.Runtime;
    using Kephas.Runtime.Factories;

    public class TestRuntimeTypeInfoFactory : IRuntimeTypeInfoFactory
    {
        public IRuntimeTypeInfo? TryCreateElementInfo(IRuntimeTypeRegistry registry, Type reflectInfo, int position = -1,
            ILogger? logger = null)
        {
            throw new NotImplementedException();
        }

        public IRuntimeElementInfo? TryCreateElementInfo(IRuntimeTypeRegistry registry, MemberInfo reflectInfo, int position = -1,
            ILogger? logger = null)
        {
            throw new NotImplementedException();
        }
    }
}