// --------------------------------------------------------------------------------------------------------------------
// <copyright file="OverrideAttribute.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Implements the override attribute class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Model.AttributedModel
{
    using System;

    /// <summary>
    /// Attribute for indicating that classifiers or members override their base. This class cannot be inherited.
    /// </summary>
    [AttributeUsage(AttributeTargets.Interface | AttributeTargets.Property | AttributeTargets.Method, Inherited = false, AllowMultiple = false)]
    public sealed class OverrideAttribute : Attribute
    {
    }
}