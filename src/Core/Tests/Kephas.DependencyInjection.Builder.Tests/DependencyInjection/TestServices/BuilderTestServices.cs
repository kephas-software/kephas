// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BuilderTestServices.cs" company="Kephas Software SRL">
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

    [SingletonService]
    public interface IMultipleTestService;

    public class SingleTestService : ISingleTestService;

    [Priority(Priority.High)]
    public class SingleOverridenTestService : ISingleTestService;

    public class MultipleTestService : IMultipleTestService;

    public class NewMultipleTestService : IMultipleTestService;

    public class DerivedMultipleTestService : MultipleTestService;

    [OverrideBaseImplementation]
    public class ChainSingleOverrideTestService : SingleOverridenTestService;

    [OverrideBaseImplementation]
    public class DerivedOverrideSingleTestService : SingleTestService;

    [SingletonService]
    public interface IGenericAppService<T>;

    public interface IOneGenericAppService;

    [SingletonService(ServiceType = typeof(IOneGenericAppService))]
    public interface IOneGenericAppService<T> : IOneGenericAppService;

    public interface ITwoGenericAppService;

    [SingletonService(ServiceType = typeof(ITwoGenericAppService))]
    public interface ITwoGenericAppService<TFrom, ToType> : ITwoGenericAppService;

    public class GenericAppService<T> : IGenericAppService<T>;

    public class OneGenericAppServiceString : IOneGenericAppService<string>;

    public class TwoGenericAppServiceIntBool : ITwoGenericAppService<int, bool>;

    [SingletonService(AsOpenGeneric = true)]
    public interface ITypedLogger<T>;

    public class TypedLogger<T> : ITypedLogger<T>;

    namespace CustomValueAppServiceMetadata
    {
        [SingletonService]
        public interface ICustomValueMetadataAppService;

        [CustomValueMetadata("hi there")]
        public class CustomValueMetadataAppService : ICustomValueMetadataAppService;

        public class CustomValueNullMetadataAppService : ICustomValueMetadataAppService;

        public class CustomValueMetadataAttribute(string value) : Attribute, IServiceMetadataValue<string>
        {
            public string Value { get; } = value;
        }
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