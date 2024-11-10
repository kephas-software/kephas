// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ReflectionHelperDynamicTest.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using Kephas.Dynamic;
using Kephas.Reflection;
using Xunit;

namespace Kephas.Tests;

public class ReflectionHelperDynamicTest
{
    [InlineData("name", "John Doe")]
    [Theory]
    public void GetPropertyValue_dynamic(string name, object? value)
    {
        var expando = new DictionaryExpando<object?>(new Dictionary<string, object?> { { "name", "John Doe" } });
        Assert.Equal(value, expando.GetPropertyValue(name));
    }

    [InlineData("Name", "Jane Doe")]
    [InlineData("Age", 3)]
    [Theory]
    public void SetPropertyValue_dynamic(string name, object? value)
    {
        var person = new ObjectExpando(new Person { Name = "John Doe", Age = 23 });
        person.SetPropertyValue(name, value);
        Assert.Equal(value, person.GetPropertyValue(name));
    }

    private class Person
    {
        public string? Name { get; set; }

        public int Age { get; set; }
    }
}