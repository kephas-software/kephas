namespace Kephas.Dynamic;

using System.Dynamic;
using Kephas.Reflection;

/// <summary>
/// An <see cref="IDynamic"/> wrapper over a dynamic object.
/// </summary>
internal class DynamicExpando : IDynamic, IAdapter<IDynamicMetaObjectProvider>
{
    private readonly IDynamicMetaObjectProvider obj;

    /// <summary>
    /// Initializes a new instance of the <see cref="DynamicExpando"/> class.
    /// </summary>
    /// <param name="obj">The dynamic object.</param>
    public DynamicExpando(IDynamicMetaObjectProvider obj)
    {
        this.obj = obj ?? throw new ArgumentNullException(nameof(obj));
    }

    /// <summary>
    /// Gets the object the current instance adapts.
    /// </summary>
    /// <value>
    /// The object the current instance adapts.
    /// </value>
    IDynamicMetaObjectProvider IAdapter<IDynamicMetaObjectProvider>.Of => this.obj;

    /// <summary>
    /// Convenience method that provides a string Indexer
    /// to the Members collection AND the strongly typed
    /// members of the object by name.
    /// // dynamic
    /// exp["Address"] = "112 nowhere lane";
    /// // strong
    /// var name = exp["StronglyTypedProperty"] as string;.
    /// </summary>
    /// <value>
    /// The <see cref="object" /> identified by the key.
    /// </value>
    /// <param name="key">The key identifying the member name.</param>
    /// <returns>The requested member value.</returns>
    public object? this[string key]
    {
        get => ReflectionHelper.GetValue(this.obj, key);
        set => ReflectionHelper.SetValue(this.obj, key, value);
    }
}