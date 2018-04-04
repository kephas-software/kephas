// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MixinInheritance.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
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

    [AspectFor(typeof(string))]
    interface IStringAspect
    {
    }

    [AspectFor(typeof(StringBuilder))]
    interface IStringBuilderAspect : IStringAspect
    {
    }
}
