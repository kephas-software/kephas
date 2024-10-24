// --------------------------------------------------------------------------------------------------------------------
// <copyright file="OrderedServicCollectionExtensionsTest.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the composition helper test class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using Kephas.Collections;
using Kephas.DependencyInjection;
using Kephas.DependencyInjection.Metadata;

namespace Kephas.Tests.Services;

using System;
using System.Collections.Generic;
using Xunit;

public class ServiceEnumerableExtensionsTest
{
    [Fact]
    public void OrderAsDictionary_empty()
    {
        var factories = new List<IFactory<string, ServiceMetadata>>();

        var dict = factories.OrderAsDictionary(f => f.Metadata.ServiceKey, f => f.Create());

        Assert.Equal(0, dict.Count);
    }

    [Fact]
    public void OrderAsDictionary_lazy_empty()
    {
        var factories = new List<Lazy<string, ServiceMetadata>>();

        var dict = factories.OrderAsDictionary(f => f.Metadata.ServiceKey, f => f.Value);

        Assert.Equal(0, dict.Count);
    }

    [Fact]
    public void OrderAsDictionary_ordered()
    {
        var factories = new List<IFactory<string, ServiceMetadata>>()
        {
            new Factory<string, ServiceMetadata>(
                () => "one service",
                new ServiceMetadata(
                    priority: (Priority)2,
                    serviceKey: "one")),
            new Factory<string, ServiceMetadata>(
                () => "two service",
                new ServiceMetadata(
                    priority: (Priority)5,
                    serviceKey: "two")),
        };

        var dict = factories.OrderAsDictionary(f => f.Metadata.ServiceKey, f => f.Create());

        Assert.Equal(2, dict.Count);
        Assert.Equal("one service", dict["one"]);
        Assert.Equal("two service", dict["two"]);
    }

    [Fact]
    public void OrderAsDictionary_lazy_ordered()
    {
        var factories = new List<Lazy<string, ServiceMetadata>>()
        {
            new Lazy<string, ServiceMetadata>(
                () => "one service",
                new ServiceMetadata(
                    priority: (Priority)2,
                    serviceKey: "one")),
            new Lazy<string, ServiceMetadata>(
                () => "two service",
                new ServiceMetadata(
                    priority: (Priority)5,
                    serviceKey: "two")),
        };

        var dict = factories.OrderAsDictionary(f => f.Metadata.ServiceKey, f => f.Value);

        Assert.Equal(2, dict.Count);
        Assert.Equal("one service", dict["one"]);
        Assert.Equal("two service", dict["two"]);
    }

    [Fact]
    public void OrderAsDictionary_override_non_deterministic_siblings()
    {
        var factories = new List<IFactory<string, ServiceMetadata>>()
        {
            new Factory<string, ServiceMetadata>(
                () => "one service",
                new ServiceMetadata(
                    priority: (Priority)2,
                    serviceKey: "one")),
            new Factory<string, ServiceMetadata>(
                () => "one service 2",
                new ServiceMetadata(
                    priority: (Priority)2,
                    serviceKey: "one")),
            new Factory<string, ServiceMetadata>(
                () => "two service",
                new ServiceMetadata(
                    priority: (Priority)5,
                    serviceKey: "two")),
            new Factory<string, ServiceMetadata>(
                () => "one service reloaded",
                new ServiceMetadata(
                    priority: (Priority)5,
                    serviceKey: "one")),
        };

        var dict = factories.OrderAsDictionary(f => f.Metadata.ServiceKey, f => f.Create());

        Assert.Equal(2, dict.Count);
        Assert.Equal("one service reloaded", dict["one"]);
        Assert.Equal("two service", dict["two"]);
    }

    [Fact]
    public void OrderAsDictionary_lazy_override_non_deterministic_siblings()
    {
        var factories = new List<Lazy<string, ServiceMetadata>>()
        {
            new Lazy<string, ServiceMetadata>(
                () => "one service",
                new ServiceMetadata(
                    priority: (Priority)2,
                    serviceKey: "one")),
            new Lazy<string, ServiceMetadata>(
                () => "one service 2",
                new ServiceMetadata(
                    priority: (Priority)2,
                    serviceKey: "one")),
            new Lazy<string, ServiceMetadata>(
                () => "two service",
                new ServiceMetadata(
                    priority: (Priority)5,
                    serviceKey: "two")),
            new Lazy<string, ServiceMetadata>(
                () => "one service reloaded",
                new ServiceMetadata(
                    priority: (Priority)5,
                    serviceKey: "one")),
        };

        var dict = factories.OrderAsDictionary(f => f.Metadata.ServiceKey, f => f.Value);

        Assert.Equal(2, dict.Count);
        Assert.Equal("one service reloaded", dict["one"]);
        Assert.Equal("two service", dict["two"]);
    }

    [Fact]
    public void OrderAsDictionary_override_non_deterministic_siblings_with_key_comparer()
    {
        var factories = new List<IFactory<string, ServiceMetadata>>()
        {
            new Factory<string, ServiceMetadata>(
                () => "one service",
                new ServiceMetadata(
                    priority: (Priority)2,
                    serviceKey: "One")),
            new Factory<string, ServiceMetadata>(
                () => "one service 2",
                new ServiceMetadata(
                    priority: (Priority)2,
                    serviceKey: "oNe")),
            new Factory<string, ServiceMetadata>(
                () => "two service",
                new ServiceMetadata(
                    priority: (Priority)5,
                    serviceKey: "two")),
            new Factory<string, ServiceMetadata>(
                () => "one service reloaded",
                new ServiceMetadata(
                    priority: (Priority)5,
                    serviceKey: "onE")),
        };

        var dict = factories.OrderAsDictionary(f => f.Metadata.ServiceKey, f => f.Create(), StringComparer.OrdinalIgnoreCase);

        Assert.Equal(2, dict.Count);
        Assert.Equal("one service reloaded", dict["one"]);
        Assert.Equal("two service", dict["two"]);
    }

    [Fact]
    public void OrderAsDictionary_lazy_override_non_deterministic_siblings_with_key_comparer()
    {
        var factories = new List<Lazy<string, ServiceMetadata>>()
        {
            new Lazy<string, ServiceMetadata>(
                () => "one service",
                new ServiceMetadata(
                    priority: (Priority)2,
                    serviceKey: "One")),
            new Lazy<string, ServiceMetadata>(
                () => "one service 2",
                new ServiceMetadata(
                    priority: (Priority)2,
                    serviceKey: "oNe")),
            new Lazy<string, ServiceMetadata>(
                () => "two service",
                new ServiceMetadata(
                    priority: (Priority)5,
                    serviceKey: "two")),
            new Lazy<string, ServiceMetadata>(
                () => "one service reloaded",
                new ServiceMetadata(
                    priority: (Priority)5,
                    serviceKey: "onE")),
        };

        var dict = factories.OrderAsDictionary(f => f.Metadata.ServiceKey, f => f.Value, StringComparer.OrdinalIgnoreCase);

        Assert.Equal(2, dict.Count);
        Assert.Equal("one service reloaded", dict["one"]);
        Assert.Equal("two service", dict["two"]);
    }

    [Fact]
    public void OrderAsDictionary_override_non_deterministic_exception()
    {
        var factories = new List<IFactory<string, ServiceMetadata>>()
        {
            new Factory<string, ServiceMetadata>(
                () => "one service",
                new ServiceMetadata(
                    priority: (Priority)2,
                    serviceKey: "one")),
            new Factory<string, ServiceMetadata>(
                () => "two service",
                new ServiceMetadata(
                    priority: (Priority)5,
                    serviceKey: "two")),
            new Factory<string, ServiceMetadata>(
                () => "one service reloaded",
                new ServiceMetadata(
                    priority: (Priority)2,
                    serviceKey: "one")),
        };

        Assert.Throws<DuplicateKeyException>(() => factories.OrderAsDictionary(f => f.Metadata.ServiceKey, f => f.Create()));
    }

    [Fact]
    public void OrderAsDictionary_lazy_override_non_deterministic_exception()
    {
        var factories = new List<Lazy<string, ServiceMetadata>>()
        {
            new Lazy<string, ServiceMetadata>(
                () => "one service",
                new ServiceMetadata(
                    priority: (Priority)2,
                    serviceKey: "one")),
            new Lazy<string, ServiceMetadata>(
                () => "two service",
                new ServiceMetadata(
                    priority: (Priority)5,
                    serviceKey: "two")),
            new Lazy<string, ServiceMetadata>(
                () => "one service reloaded",
                new ServiceMetadata(
                    priority: (Priority)2,
                    serviceKey: "one")),
        };

        Assert.Throws<DuplicateKeyException>(() => factories.OrderAsDictionary(f => f.Metadata.ServiceKey, f => f.Value));
    }

    [Fact]
    public void OrderAsDictionary_export_factory_override_non_deterministic_exception()
    {
        var factories = new List<IFactory<string, ServiceMetadata>>()
        {
            new Factory<string, ServiceMetadata>(
                () => "one service",
                new ServiceMetadata(
                    priority: (Priority)2,
                    serviceKey: "one")),
            new Factory<string, ServiceMetadata>(
                () => "two service",
                new ServiceMetadata(
                    priority: (Priority)5,
                    serviceKey: "two")),
            new Factory<string, ServiceMetadata>(
                () => "one service reloaded",
                new ServiceMetadata(
                    priority: (Priority)2,
                    serviceKey: "one")),
        };

        Assert.Throws<DuplicateKeyException>(() => factories.OrderAsDictionary(f => f.Metadata.ServiceKey));
    }

    [Fact]
    public void OrderAsDictionary_lazy_export_factory_override_non_deterministic_exception()
    {
        var factories = new List<Lazy<string, ServiceMetadata>>()
        {
            new Lazy<string, ServiceMetadata>(
                () => "one service",
                new ServiceMetadata(
                    priority: (Priority)2,
                    serviceKey: "one")),
            new Lazy<string, ServiceMetadata>(
                () => "two service",
                new ServiceMetadata(
                    priority: (Priority)5,
                    serviceKey: "two")),
            new Lazy<string, ServiceMetadata>(
                () => "one service reloaded",
                new ServiceMetadata(
                    priority: (Priority)2,
                    serviceKey: "one")),
        };

        Assert.Throws<DuplicateKeyException>(() => factories.OrderAsDictionary(f => f.Metadata.ServiceKey));
    }

    [Fact]
    public void OrderAsDictionary_override_non_sortable_keys()
    {
        var factories = new List<IFactory<string, ServiceMetadata>>()
        {
            new Factory<string, ServiceMetadata>(
                () => "one service",
                new ServiceMetadata(
                    priority: (Priority)2,
                    serviceKey: "one") { ["Key"] = typeof(int) }),
            new Factory<string, ServiceMetadata>(
                () => "two service",
                new ServiceMetadata(
                    priority: (Priority)5,
                    serviceKey: "two") { ["Key"] = typeof(string) }),
            new Factory<string, ServiceMetadata>(
                () => "one service reloaded",
                new ServiceMetadata(
                    priority: (Priority)5,
                    serviceKey: "one") { ["Key"] = typeof(int) }),
        };

        var dict = factories.OrderAsDictionary(f => (Type)f.Metadata["Key"], f => f.Create());

        Assert.Equal(2, dict.Count);
        Assert.Equal("one service reloaded", dict[typeof(int)]);
        Assert.Equal("two service", dict[typeof(string)]);
    }

    [Fact]
    public void OrderAsDictionary_lazy_override_non_sortable_keys()
    {
        var factories = new List<Lazy<string, ServiceMetadata>>()
        {
            new Lazy<string, ServiceMetadata>(
                () => "one service",
                new ServiceMetadata(
                    priority: (Priority)2,
                    serviceKey: "one") { ["Key"] = typeof(int) }),
            new Lazy<string, ServiceMetadata>(
                () => "two service",
                new ServiceMetadata(
                    priority: (Priority)5,
                    serviceKey: "two") { ["Key"] = typeof(string) }),
            new Lazy<string, ServiceMetadata>(
                () => "one service reloaded",
                new ServiceMetadata(
                    priority: (Priority)5,
                    serviceKey: "one") { ["Key"] = typeof(int) }),
        };

        var dict = factories.OrderAsDictionary(f => (Type)f.Metadata["Key"], f => f.Value);

        Assert.Equal(2, dict.Count);
        Assert.Equal("one service reloaded", dict[typeof(int)]);
        Assert.Equal("two service", dict[typeof(string)]);
    }

    [Fact]
    public void OrderAsDictionary_export_factory_override_non_sortable_keys()
    {
        var factories = new List<IFactory<string, ServiceMetadata>>()
        {
            new Factory<string, ServiceMetadata>(
                () => "one service",
                new ServiceMetadata(
                    priority: (Priority)2,
                    serviceKey: "one") { ["Key"] = typeof(int) }),
            new Factory<string, ServiceMetadata>(
                () => "two service",
                new ServiceMetadata(
                    priority: (Priority)5,
                    serviceKey: "two") { ["Key"] = typeof(string) }),
            new Factory<string, ServiceMetadata>(
                () => "one service reloaded",
                new ServiceMetadata(
                    priority: (Priority)5,
                    serviceKey: "one") { ["Key"] = typeof(int) }),
        };

        var dict = factories.OrderAsDictionary(f => (Type)f.Metadata["Key"]);

        Assert.Equal(2, dict.Count);
        Assert.Equal("one service reloaded", dict[typeof(int)].Create());
        Assert.Equal("two service", dict[typeof(string)].Create());
    }

    [Fact]
    public void OrderAsDictionary_lazy_export_factory_override_non_sortable_keys()
    {
        var factories = new List<Lazy<string, ServiceMetadata>>()
        {
            new Lazy<string, ServiceMetadata>(
                () => "one service",
                new ServiceMetadata(
                    priority: (Priority)2,
                    serviceKey: "one") { ["Key"] = typeof(int) }),
            new Lazy<string, ServiceMetadata>(
                () => "two service",
                new ServiceMetadata(
                    priority: (Priority)5,
                    serviceKey: "two") { ["Key"] = typeof(string) }),
            new Lazy<string, ServiceMetadata>(
                () => "one service reloaded",
                new ServiceMetadata(
                    priority: (Priority)5,
                    serviceKey: "one") { ["Key"] = typeof(int) }),
        };

        var dict = factories.OrderAsDictionary(f => (Type)f.Metadata["Key"]);

        Assert.Equal(2, dict.Count);
        Assert.Equal("one service reloaded", dict[typeof(int)].Value);
        Assert.Equal("two service", dict[typeof(string)].Value);
    }
}