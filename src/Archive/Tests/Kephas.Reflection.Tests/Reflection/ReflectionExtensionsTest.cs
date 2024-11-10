// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ReflectionExtensionsTest.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Test class for <see cref="ReflectionHelper" />.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Tests.Reflection;

using System.Collections.Generic;

using Kephas.Dynamic;
using Kephas.Reflection;
using NSubstitute;
using NSubstitute.Core;
using NUnit.Framework;
using ReflectionExtensions = Kephas.Reflection.ReflectionExtensions;

/// <summary>
/// Test class for <see cref="ReflectionHelper"/>.
/// </summary>
[TestFixture]
public class ReflectionExtensionsTest
{
    [Test]
    public void AsRuntimeAssemblyInfo()
    {
        var assemblyInfo = ReflectionExtensions.AsRuntimeAssemblyInfo(this.GetType().Assembly);
        Assert.AreSame(assemblyInfo.GetUnderlyingAssemblyInfo(), this.GetType().Assembly);
    }

    [Test]
    public void GetTypeInfo_non_IInstance()
    {
        var typeInfo = ReflectionExtensions.GetTypeInfo("123");
        Assert.AreSame(typeof(string).AsRuntimeTypeInfo(), typeInfo);
    }

    [Test]
    public void GetTypeInfo_IInstance()
    {
        var instance = Substitute.For<IInstance>();
        var typeInfo = Substitute.For<ITypeInfo>();
        instance.GetTypeInfo().Returns(typeInfo);

        var obj = (object)instance;
        var objTypeInfo = ReflectionExtensions.GetTypeInfo(obj);

        Assert.AreSame(typeInfo, objTypeInfo);
    }
}