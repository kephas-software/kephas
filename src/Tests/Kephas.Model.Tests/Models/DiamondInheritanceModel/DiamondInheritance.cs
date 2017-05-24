// --------------------------------------------------------------------------------------------------------------------
// <copyright file="INamed.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
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