// --------------------------------------------------------------------------------------------------------------------
// <copyright file="INamed.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Declares the INamed interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Model.Tests.Models.DiamondInheritanceModel
{
    using Kephas.Model.AttributedModel;

    [Mixin]
    public interface INamed
    {
        string Name { get; set; }
    }

    [Mixin]
    //// [NaturalKey(new[] { "Name" })]
    public interface IUniquelyNamed : INamed
    {
    }

    [Mixin]
    public interface IParameter : INamed
    {
    }

    [Mixin]
    public interface IAppParameter : IParameter, IUniquelyNamed
    {
    }
}