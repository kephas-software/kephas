// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IFormat.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Contract for serialization formats.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Serialization
{
    using Kephas.Net.Mime;
    using Kephas.Services;

    /// <summary>
    /// Contract for serialization formats.
    /// </summary>
    [SharedAppServiceContract(AllowMultiple = true, MetadataAttributes = new[] { typeof(SupportedMediaTypesAttribute) })]
    public interface IFormat
    {
    }
}