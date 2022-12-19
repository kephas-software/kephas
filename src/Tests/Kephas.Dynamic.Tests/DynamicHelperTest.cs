namespace Kephas.Tests;

using Kephas.Dynamic;
using NUnit.Framework;

[TestFixture]
public class DynamicHelperTest
{
    [TestCase(typeof(string), "Name", "String")]
    [TestCase("123", "Length", 3)]
    [Test]
    public void GetValue_simple(object target, string name, object? value)
    {
        Assert.AreEqual(value, DynamicHelper.GetValue(target, name));
    }

    [TestCase("name", "John Doe")]
    [Test]
    public void GetValue_dynamic(string name, object? value)
    {
        var expando = new DictionaryExpando<object?>(new Dictionary<string, object?> { { "name", "John Doe" } });
        Assert.AreEqual(value, DynamicHelper.GetValue(expando, name));
    }

    [TestCase("Name", "Jane Doe")]
    [TestCase("Age", 3)]
    [Test]
    public void SetValue_simple(string name, object? value)
    {
        var person = new Person { Name = "John Doe", Age = 23 };
        DynamicHelper.SetValue(person, name, value);
        Assert.AreEqual(value, DynamicHelper.GetValue(person, name));
    }

    [TestCase("Name", "Jane Doe")]
    [TestCase("Age", 3)]
    [Test]
    public void SetValue_dynamic(string name, object? value)
    {
        var person = new ObjectExpando(new Person { Name = "John Doe", Age = 23 });
        DynamicHelper.SetValue(person, name, value);
        Assert.AreEqual(value, DynamicHelper.GetValue(person, name));
    }

    public class Person
    {
        public string? Name { get; set; }

        public int Age { get; set; }
    }
}