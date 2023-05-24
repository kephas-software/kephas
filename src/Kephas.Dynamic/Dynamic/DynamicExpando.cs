// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DynamicExpando.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Dynamic;

using System.Dynamic;

/// <summary>
/// An <see cref="IDynamic"/> wrapper over a dynamic object.
/// </summary>
internal class DynamicExpando : IDynamic
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
    /// <returns>The inner <see cref="IDynamicMetaObjectProvider"/>.</returns>
    internal IDynamicMetaObjectProvider GetDynamicMetaObjectProvider() => this.obj;

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
        get => DynamicHelper.GetValue(this.obj, key);
        set => DynamicHelper.SetValue(this.obj, key, value);
    }

    /// <summary>
    /// Gets the dynamic member names of this instance.
    /// </summary>
    /// <returns>An enumeration of member names.</returns>
    public IEnumerable<string> GetDynamicMemberNames() =>
        this.obj is DynamicObject dynObject
            ? dynObject.GetDynamicMemberNames()
            : Array.Empty<string>();

    /// <summary>
    /// Indicates whether the <paramref name="memberName"/> is defined in the expando.
    /// </summary>
    /// <param name="memberName">Name of the member.</param>
    /// <returns>
    /// True if defined, false if not.
    /// </returns>
    public bool HasDynamicMember(string memberName) => this.GetDynamicMemberNames().Contains(memberName);
}