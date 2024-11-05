// --------------------------------------------------------------------------------------------------------------------
// <copyright file="OtherAppServices.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using Kephas.DependencyInjection;
using Kephas.DependencyInjection.Metadata;

namespace Kephas.Extensions.Tests.TestServices;

[SingletonService]
public interface ITestAppService;

[SingletonService]
public interface ITestMultiAppService;

[SingletonService]
public interface ITestMultiAppServiceConsumer;

public class TestAppService : ITestAppService;

[Priority(Priority.High)]
public class TestOverrideAppService : ITestAppService;

public class TestMultiAppService1 : ITestMultiAppService;

public class TestMultiAppService2 : ITestMultiAppService;

public class TestMultiAppServiceConsumer : ITestMultiAppServiceConsumer
{
    public TestMultiAppServiceConsumer(
        IEnumerable<IFactory<ITestMultiAppService>> factories,
        IEnumerable<IFactory<ITestMultiAppService, ServiceMetadata>> metadataFactories)
    {
        Factories = factories;
        MetadataFactories = metadataFactories;
    }

    public IEnumerable<IFactory<ITestMultiAppService>> Factories { get; set; }

    public IEnumerable<IFactory<ITestMultiAppService, ServiceMetadata>> MetadataFactories { get; set; }
}

public interface IConverter;

[SingletonService(ServiceType = typeof(IConverter))]
public interface IConverter<TSource, TTarget> : IConverter;

[Priority(100)]
public class StringToIntConverter : IConverter<string, int>;

public class SecondStringToIntConverter : IConverter<string, int>;

[ScopedService]
public interface ITestMyScopedExport;

public class TestMyScopedExport : ITestMyScopedExport;

[ScopedService]
public interface ITestScopedExport;

public class TestScopedExport : ITestScopedExport;

[SingletonService]
public interface ITestGenericExport<T>;

public class TestGenericExport : ITestGenericExport<string>;

public interface ITestGenericWithNonGenericExport;

[TransientService(ServiceType = typeof(ITestGenericWithNonGenericExport))]
public interface ITestGenericWithNonGenericExport<T> : ITestGenericWithNonGenericExport;

public class TestGenericWithNonGenericExport : ITestGenericWithNonGenericExport<string>;