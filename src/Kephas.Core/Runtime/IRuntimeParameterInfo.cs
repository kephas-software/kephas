// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IRuntimeParameterInfo.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Declares the IRuntimeParameterInfo interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Runtime
{
    using Kephas.Reflection;

    /// <summary>
    /// Interface for runtime parameter information.
    /// </summary>
    public interface IRuntimeParameterInfo : IParameterInfo, IRuntimeElementInfo
    {
    }
}