namespace Kephas.Tests;

using System.Dynamic;
using Kephas.Dynamic;
using NSubstitute;
using NUnit.Framework;

[TestFixture]
public class DynamicHelperTest
{
    [Test]
    public void ToDynamicObject_dynamic()
    {
        var obj = new ExpandoObject();
        var dyn = obj.ToDynamicObject();
        Assert.AreSame(obj, dyn);
    }

    [Test]
    public void ToDynamicObject_non_dynamic_method()
    {
        var obj = new List<string>();
        var dyn = obj.ToDynamicObject();
        Assert.AreNotSame(obj, dyn);

        dyn.Add("John");
        Assert.AreEqual(1, obj.Count);
        Assert.IsTrue(obj.Contains("John"));
    }

    [Test]
    public void ToDynamicObject_non_dynamic_property()
    {
        var obj = new List<string> { "one", "two" };
        var dyn = obj.ToDynamicObject();
        Assert.AreNotSame(obj, dyn);

        Assert.AreEqual(2, dyn.Count);
    }

    [Test]
    public void ToDynamicObject_null_exception()
    {
        Assert.Throws<ArgumentNullException>(() => ((object)null).ToDynamicObject());
    }

    [Test]
    public void ToDynamic_null_exception()
    {
        Assert.Throws<ArgumentNullException>(() => ((object)null!).ToDynamic());
    }

    [Test]
    public void ToDynamic_object()
    {
        var obj = new { Name = "John", FamilyName = "Doe" };
        var expando = obj.ToDynamic();

        Assert.AreNotSame(obj, expando);
        Assert.AreEqual("John", expando["Name"]);
        Assert.AreEqual("Doe", expando["FamilyName"]);
    }

    [Test]
    public void ToDynamic_dynamic()
    {
        var expando = Substitute.For<IDynamic>();
        Assert.AreSame(expando, expando.ToDynamic());
    }

    [Test]
    public void ToDynamic_dictionary_string_object()
    {
        var dictionary = new Dictionary<string, object?>();
        var expando = dictionary.ToDynamic();

        expando["hi"] = "there";

        Assert.AreEqual("there", dictionary["hi"]);
    }

    [Test]
    public void ToDynamic_dictionary_string_string()
    {
        var dictionary = new Dictionary<string, string>();
        var expando = dictionary.ToDynamic();

        expando["hi"] = "there";

        Assert.AreEqual("there", dictionary["hi"]);
    }

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