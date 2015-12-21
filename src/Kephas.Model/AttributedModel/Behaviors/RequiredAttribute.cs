// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RequiredAttribute.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Attribute indicating a required field or property. This class cannot be inherited.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Model.AttributedModel.Behaviors
{
    using System;

    /// <summary>
    /// Attribute indicating a required field or property. This class cannot be inherited.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false)]
    public sealed class RequiredAttribute : Attribute
    {
    }
}