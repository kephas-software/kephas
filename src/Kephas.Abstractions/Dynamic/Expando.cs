// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Expando.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Dynamic;

/// <summary>
/// <para>
/// Class that provides extensible properties and methods. This
/// dynamic object stores 'extra' properties in a dictionary or
/// checks the actual properties of the instance.
/// This means you can subclass this expando and retrieve either
/// native properties or properties from values in the dictionary.
/// </para>
/// <para>
/// This type allows you three ways to access its properties:
/// <list type="bullet">
/// <item>
/// <term>Directly</term>
/// <description>any explicitly declared properties are accessible</description>
/// </item>
/// <item>
/// <term>Dynamic</term>
/// <description>dynamic cast allows access to dictionary and native properties/methods</description>
/// </item>
/// <item>
/// <term>Dictionary</term>
/// <description>Any of the extended properties are accessible via IDictionary interface</description>
/// </item>
/// </list>
/// </para>
/// </summary>
public class Expando : Expando<object?>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="Expando"/> class. This constructor just works
    /// off the internal dictionary and any public properties of this object.
    /// </summary>
    public Expando()
        : base(isThreadSafe: false)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Expando"/> class. This constructor just works
    /// off the internal dictionary and any public properties of this object.
    /// </summary>
    /// <param name="isThreadSafe"><c>true</c> if this object is thread safe when working with the internal dictionary, <c>false</c> otherwise.</param>
    public Expando(bool isThreadSafe)
        : base(isThreadSafe)
    {
    }
}