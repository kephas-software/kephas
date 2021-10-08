// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MixinInheritance.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the mixin inheritance class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Model.Tests.Models.MixinInheritanceModel
{
    using System.Text;

    using Kephas.Model.AttributedModel;

    [Mixin]
    interface INamed
    {
    }

    [Mixin]
    interface IUniquelyNamed : INamed
    {
    }

    interface IParameter : IUniquelyNamed
    {
    }

    public class MyString {}

    public class MyStringBuilder {}

    [AspectFor(typeof(MyString))]
    interface IStringAspect
    {
    }

    [AspectFor(typeof(MyStringBuilder))]
    interface IStringBuilderAspect : IStringAspect
    {
    }
}
