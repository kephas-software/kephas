// --------------------------------------------------------------------------------------------------------------------
// <copyright file="NonDeclaredServices.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using Kephas.DependencyInjection.Metadata;

namespace Kephas.Extensions.Tests.TestServices;

public interface INonDeclaredService;

public class NonDeclaredService : INonDeclaredService;

public class NonDeclaredServiceConsumer(INonDeclaredService Service);

public record NonDeclaredMetadata : ServiceMetadata
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ServiceMetadata"/> class.
    /// </summary>
    /// <param name="metadata">The metadata.</param>
    public NonDeclaredMetadata(IDictionary<string, object?>? metadata)
        : base(metadata)
    {
        if (metadata == null)
        {
            return;
        }

        Name = (string?)TryGetValue(metadata, nameof(Name));
    }

    public string? Name { get; }
}
