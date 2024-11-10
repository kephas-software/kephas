// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BuilderNonGenericTestServices.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using Kephas.DependencyInjection;
using Kephas.DependencyInjection.Metadata;

namespace Kephas.Tests.DependencyInjection.TestServices
{
    [SingletonService]
    public interface ISingleTestService;

    public class SingleTestService : ISingleTestService;

    [Priority(Priority.High)]
    public class SingleOverridenTestService : ISingleTestService;

    [OverrideBaseImplementation]
    public class ChainSingleOverrideTestService : SingleOverridenTestService;

    [OverrideBaseImplementation]
    public class DerivedOverrideSingleTestService : SingleTestService;

    [SingletonService]
    public interface IMultipleTestService;

    public class MultipleTestService : IMultipleTestService;

    public class NewMultipleTestService : IMultipleTestService;

    public class DerivedMultipleTestService : MultipleTestService;

    namespace CustomValueAppServiceMetadata
    {
        [SingletonService]
        public interface ICustomValueMetadataAppService;

        [CustomValueMetadata("hi there")]
        public class CustomValueMetadataAppService : ICustomValueMetadataAppService;

        public class CustomValueNullMetadataAppService : ICustomValueMetadataAppService;

        public class CustomValueMetadataAttribute(string value) : ServiceMetadataValueAttributeBase<string>(value);
    }

    namespace CustomNamedValueAppServiceMetadata
    {
        [SingletonService]
        public interface ICustomNamedValueMetadataService;

        [CustomNamedValueMetadata("hi there", "bob", IconName = "ICXP")]
        public class CustomNamedValueMetadataService : ICustomNamedValueMetadataService;

        public class CustomNamedValueNullMetadataService : ICustomNamedValueMetadataService;

        public class CustomNamedValueMetadataAttribute(string name, string? description)
            : Attribute, IServiceMetadataProvider
        {
            public string Name { get; } = name;

            public string? IconName { get; set; }

            public string? Description { get; } = description;

            public IEnumerable<KeyValuePair<string, object?>> GetMetadata()
            {
                yield return KeyValuePair.Create(nameof(Name), (object?)Name);
                yield return KeyValuePair.Create(nameof(IconName), (object?)IconName);
            }
        }
    }

    namespace DefaultAppServiceMetadata
    {
        [TransientService]
        public interface IDefaultMetadataAppService;

        public class DefaultMetadataAppService : IDefaultMetadataAppService;
    }

    namespace DefaultExplicitAppServiceMetadata
    {
        [SingletonService]
        public interface IExplicitMetadataAppService;

        [Priority(100)]
        public class ExplicitMetadataAppService : IExplicitMetadataAppService;

        public class NullExplicitMetadataAppService : IExplicitMetadataAppService;
    }
}