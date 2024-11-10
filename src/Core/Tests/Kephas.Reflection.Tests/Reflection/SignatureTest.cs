// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SignatureTest.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the signature test class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using Kephas.Reflection;
using Xunit;

namespace Kephas.Tests.Reflection;

public class SignatureTest
{
    [Fact]
    public void Equals_this()
    {
        var s = new Signature();
        Assert.True(s.Equals(s));
    }

    [Fact]
    public void Equals_empty()
    {
        var s = new Signature();
        var s2 = new Signature();
        Assert.True(s.Equals(s2));
    }

    [Fact]
    public void Equals_same_types()
    {
        var s = new Signature(typeof(string), typeof(int));
        var s2 = new Signature(typeof(string), typeof(int));
        Assert.True(s.Equals(s2));
    }

    [Fact]
    public void Equals_different_types()
    {
        var s = new Signature(typeof(string), typeof(long));
        var s2 = new Signature(typeof(string), typeof(int));
        Assert.False(s.Equals(s2));
    }

    [Fact]
    public void ToString_int_string()
    {
        var s = new Signature(typeof(int), typeof(string));
        Assert.Equal("(Int32, String)", s.ToString());
    }

    [Fact]
    public void ToString_Signature_nullablelong_null()
    {
        var s = new Signature(typeof(Signature), typeof(long?), null);
        Assert.Equal("(Signature, Int64?, )", s.ToString());
    }

    [Fact]
    public void GetEnumerator_type()
    {
        var s = new Signature(typeof(Signature), typeof(bool), typeof(int?));
        var e = s.GetEnumerator();
        e.MoveNext();
        Assert.Same(typeof(Signature), e.Current);
        e.MoveNext();
        Assert.Same(typeof(bool), e.Current);
        e.MoveNext();
        Assert.Same(typeof(int?), e.Current);
        Assert.False(e.MoveNext());
    }
}