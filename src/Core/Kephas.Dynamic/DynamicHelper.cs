// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DynamicHelper.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


using System.Collections.Concurrent;
using System.Dynamic;
using System.Reflection;
using System.Runtime.CompilerServices;

using Kephas.Collections;
using Kephas.Dynamic;
using Kephas.Reflection;

using Microsoft.CSharp.RuntimeBinder;

using Binder = Microsoft.CSharp.RuntimeBinder.Binder;

namespace Kephas;

/// <summary>
/// Extension methods for dynamic objects.
/// </summary>
public static class DynamicHelper
{
    private static readonly MethodInfo ToDictionaryExpandoMethod = typeof(DynamicHelper)
            .GetMethods(BindingFlags.Static | BindingFlags.NonPublic)
            .Single(m => m.Name == nameof(ToDictionaryExpando));

    /// <summary>
    /// Converts the provided instance to a dictionary.
    /// </summary>
    /// <param name="obj">The object to convert.</param>
    /// <returns>The provided instance, if it is a dictionary, or a dictionary containing its content.</returns>
    public static IDictionary<string, object?> ToDictionary(this object obj)
    {
        obj = obj ?? throw new ArgumentNullException(nameof(obj));

        return obj switch
        {
            IDictionary<string, object?> dictionary => dictionary,
            IDictionaryConvertible expando => expando.ToDictionary(),
            _ => obj.ToDynamic().ToDictionary()
        };
    }

    /// <summary>
    /// Gets a dynamic object out of the provided instance.
    /// </summary>
    /// <param name="obj">The object to convert.</param>
    /// <returns>The provided instance, if it is a dynamic object, or a dynamic wrapper over it.</returns>
    public static dynamic ToDynamicObject(this object obj)
    {
        obj = obj ?? throw new ArgumentNullException(nameof(obj));

        return obj switch
        {
            IDynamicMetaObjectProvider => obj,
            DynamicExpando dynamicAdapter => dynamicAdapter.GetDynamicMetaObjectProvider(),
            _ => obj.ToDynamic()
        };
    }

    /// <summary>
    /// Gets an <see cref="IDynamic"/> object out of the provided instance.
    /// </summary>
    /// <param name="obj">The object to convert.</param>
    /// <returns>The provided instance, if it is an <see cref="IDynamic"/>, or a dynamic wrapper over it.</returns>
    public static IDynamic ToDynamic(this object obj)
    {
        obj = obj ?? throw new ArgumentNullException(nameof(obj));

        return obj switch
        {
            IDynamic dynamic => dynamic,
            IDictionary<string, object?> objectDictionary => new DictionaryExpando<object?>(objectDictionary),
            IDynamicMetaObjectProvider dynamic => new DynamicExpando(dynamic),
            _ => ToDynamicCore(obj),
        };
    }

    private static IDynamic ToDynamicCore(object obj)
    {
        var keyItemTypePair = obj.GetType().TryGetDictionaryKeyItemType();
        if (keyItemTypePair is null || keyItemTypePair.Value.keyType != typeof(string))
        {
            return new ObjectExpando(obj);
        }

        var toTypedExpando = ToDictionaryExpandoMethod.MakeGenericMethod(keyItemTypePair.Value.itemType);
        return toTypedExpando.Call<IDynamic>(null, obj)!;
    }

    private static IDynamic ToDictionaryExpando<T>(IDictionary<string, T> dictionary) =>
        new DictionaryExpando<T>(dictionary);

    private static (Type keyType, Type itemType)? TryGetDictionaryKeyItemType(this Type type)
    {
        var dictionaryGenericType = typeof(IDictionary<,>);

        bool IsRequestedDictionary(Type t)
        {
            return t.IsGenericType && t.GetGenericTypeDefinition() == dictionaryGenericType;
        }

        var dictionaryType = IsRequestedDictionary(type)
            ? type
            : type.GetInterfaces().SingleOrDefault(IsRequestedDictionary);

        return dictionaryType == null
            ? null
            : (dictionaryType.GenericTypeArguments[0], dictionaryType.GenericTypeArguments[1]);
    }
}
