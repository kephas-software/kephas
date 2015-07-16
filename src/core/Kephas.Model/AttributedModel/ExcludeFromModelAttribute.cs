// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ExcludeFromModelAttribute.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Marks a type or a member being excluded from the model.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Model.AttributedModel
{
    using System;

    /// <summary>
    /// Marks a type or a member being excluded from the model.
    /// </summary>
    [AttributeUsage(AttributeTargets.All, Inherited = false, AllowMultiple = false)]
    public sealed class ExcludeFromModelAttribute : Attribute
    {
    }
}