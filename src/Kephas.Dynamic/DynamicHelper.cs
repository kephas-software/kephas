// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DynamicHelper.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas;

using System.Collections.Concurrent;
using System.Dynamic;
using System.Reflection;
using System.Runtime.CompilerServices;

using Kephas.Dynamic;
using Microsoft.CSharp.RuntimeBinder;

using Binder = Microsoft.CSharp.RuntimeBinder.Binder;

/// <summary>
/// Extension methods for dynamic objects.
/// </summary>
public static class DynamicHelper
{
    private static readonly ConcurrentDictionary<string, CallSite<Func<CallSite, object, object>>> Getters = new();

    private static readonly ConcurrentDictionary<string, CallSite<Func<CallSite, object, object?, object>>> Setters =
        new();

    private static readonly MethodInfo ToDictionaryExpandoMethod =
        typeof(DynamicHelper).GetMethod(nameof(ToDictionaryExpando))!;

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
            IDynamic expando => expando.ToDictionary(),
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
    [Obsolete("Use ToDynamic() extension method instead.", error: true)]
    public static IDynamic ToExpando(this object obj) => ToDynamic(obj);

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

    /// <summary>
    /// Invokes the <paramref name="methodInfo"/> with the provided parameters,
    /// ensuring in case of an exception that the original exception is thrown.
    /// </summary>
    /// <typeparam name="TReturn">The return type.</typeparam>
    /// <param name="methodInfo">The method information.</param>
    /// <param name="instance">The instance.</param>
    /// <param name="arguments">A variable-length parameters list containing arguments.</param>
    /// <returns>
    /// The invocation result.
    /// </returns>
    public static TReturn Call<TReturn>(this MethodInfo methodInfo, object? instance, params object?[]? arguments)
    {
        var returnValue = Call(methodInfo, instance, arguments);
        return (TReturn)returnValue!;
    }

    /// <summary>
    /// Invokes the <paramref name="methodInfo"/> with the provided parameters,
    /// ensuring in case of an exception that the original exception is thrown.
    /// </summary>
    /// <param name="methodInfo">The method information.</param>
    /// <param name="instance">The instance.</param>
    /// <param name="arguments">A variable-length parameters list containing arguments.</param>
    /// <returns>
    /// The invocation result.
    /// </returns>
    public static object? Call(this MethodInfo methodInfo, object? instance, params object?[]? arguments)
    {
        try
        {
            var result = methodInfo.Invoke(instance, arguments);
            return result;
        }
        catch (TargetInvocationException tie)
        {
            throw tie.InnerException!;
        }
    }

    /// <summary>
    /// Gets the value of the property identified by its name.
    /// </summary>
    /// <param name="target">The target object.</param>
    /// <param name="name">The property name.</param>
    /// <returns>The property value.</returns>
    public static object? GetValue(object target, string name)
    {
        target = target ?? throw new ArgumentNullException(nameof(target));

        var callSite = Getters.GetOrAdd(name, _ =>
        {
            return CallSite<Func<CallSite, object, object>>.Create(
                Binder.GetMember(
                    CSharpBinderFlags.None,
                    name,
                    typeof(DynamicHelper),
                    new[]
                    {
                        CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null),
                    }));
        });

        return callSite.Target(callSite, target);
    }

    /// <summary>
    /// Sets the value of the property identified by its name.
    /// </summary>
    /// <param name="target">The target object.</param>
    /// <param name="name">The property name.</param>
    /// <param name="value">The value to be set.</param>
    public static void SetValue(object target, string name, object? value)
    {
        target = target ?? throw new ArgumentNullException(nameof(target));

        if (target is not IDynamicMetaObjectProvider)
        {
            // TODO optimize
            // workaround for non dynamic targets.
            var propertyInfo = target.GetType().GetProperty(name, BindingFlags.Instance | BindingFlags.Public);
            if (propertyInfo is null)
            {
                throw new InvalidOperationException($"Property {name} was not found.");
            }

            propertyInfo.SetValue(target, value);
            return;
        }

        var callSite = Setters.GetOrAdd(name, _ =>
        {
            return CallSite<Func<CallSite, object, object?, object>>.Create(
                Binder.SetMember(
                    CSharpBinderFlags.None,
                    name,
                    typeof(DynamicHelper),
                    new[]
                    {
                        CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null),
                        CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType, null),
                    }));
        });

        callSite.Target(callSite, target, value);
    }

    /// <summary>
    /// Indicates whether the identifier is private.
    /// </summary>
    /// <param name="identifier">The identifier to act on.</param>
    /// <returns>
    /// True if the identifier is private, false if not.
    /// </returns>
    public static bool IsPrivate(this string identifier)
    {
        return identifier.StartsWith("_") || identifier.StartsWith("#");
    }

    /// <summary>
    /// Gets the type's proper properties: public, non-static, and without parameters.
    /// </summary>
    /// <param name="type">The type.</param>
    /// <returns>An enumeration of property infos.</returns>
    internal static IEnumerable<PropertyInfo> GetTypeProperties(Type type)
    {
        return type.GetRuntimeProperties()
            .Where(p => p.GetMethod != null && !p.GetMethod.IsStatic && p.GetMethod.IsPublic
                        && p.GetIndexParameters().Length == 0);
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