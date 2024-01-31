namespace Kephas.Tests;

using System.Dynamic;
using Kephas.Dynamic;
using NSubstitute;
using Xunit;

public class DynamicHelperTest
{
    [Fact]
    public void ToDynamicObject_dynamic()
    {
        var obj = new ExpandoObject();
        var dyn = obj.ToDynamicObject();
        Assert.Same(obj, dyn);
    }

    [Fact]
    public void ToDynamicObject_non_dynamic_method()
    {
        var obj = new List<string>();
        var dyn = obj.ToDynamicObject();
        Assert.NotSame(obj, dyn);

        dyn.Add("John");
        Assert.Equal(1, obj.Count);
        Assert.True(obj.Contains("John"));
    }

    [Fact]
    public void ToDynamicObject_non_dynamic_property()
    {
        var obj = new List<string> { "one", "two" };
        var dyn = obj.ToDynamicObject();
        Assert.NotSame(obj, dyn);

        Assert.Equal(2, dyn.Count);
    }

    [Fact]
    public void ToDynamicObject_null_exception()
    {
        Assert.Throws<ArgumentNullException>(() => ((object)null).ToDynamicObject());
    }

    [Fact]
    public void ToDynamic_null_exception()
    {
        Assert.Throws<ArgumentNullException>(() => ((object)null!).ToDynamic());
    }

    [Fact]
    public void ToDynamic_object()
    {
        var obj = new { Name = "John", FamilyName = "Doe" };
        var expando = obj.ToDynamic();

        Assert.NotSame(obj, expando);
        Assert.Equal("John", expando["Name"]);
        Assert.Equal("Doe", expando["FamilyName"]);
    }

    [Fact]
    public void ToDynamic_dynamic()
    {
        var expando = Substitute.For<IDynamic>();
        Assert.Same(expando, expando.ToDynamic());
    }

    [Fact]
    public void ToDynamic_dictionary_string_object()
    {
        var dictionary = new Dictionary<string, object?>();
        var expando = dictionary.ToDynamic();

        expando["hi"] = "there";

        Assert.Equal("there", dictionary["hi"]);
    }

    [Fact]
    public void ToDynamic_dictionary_string_string()
    {
        var dictionary = new Dictionary<string, string>();
        var expando = dictionary.ToDynamic();

        expando["hi"] = "there";

        Assert.Equal("there", dictionary["hi"]);
    }

    [InlineData(typeof(string), "Name", "String")]
    [InlineData("123", "Length", 3)]
    [Theory]
    public void GetValue_simple(object target, string name, object? value)
    {
        Assert.Equal(value, DynamicHelper.GetValue(target, name));
    }

    [InlineData("name", "John Doe")]
    [Theory]
    public void GetValue_dynamic(string name, object? value)
    {
        var expando = new DictionaryExpando<object?>(new Dictionary<string, object?> { { "name", "John Doe" } });
        Assert.Equal(value, DynamicHelper.GetValue(expando, name));
    }

    [InlineData("Name", "Jane Doe")]
    [InlineData("Age", 3)]
    [Theory]
    public void SetValue_simple(string name, object? value)
    {
        var person = new Person { Name = "John Doe", Age = 23 };
        DynamicHelper.SetValue(person, name, value);
        Assert.Equal(value, DynamicHelper.GetValue(person, name));
    }

    [InlineData("Name", "Jane Doe")]
    [InlineData("Age", 3)]
    [Theory]
    public void SetValue_dynamic(string name, object? value)
    {
        var person = new ObjectExpando(new Person { Name = "John Doe", Age = 23 });
        DynamicHelper.SetValue(person, name, value);
        Assert.Equal(value, DynamicHelper.GetValue(person, name));
    }

    public class Person
    {
        public string? Name { get; set; }

        public int Age { get; set; }
    }
}
