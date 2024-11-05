// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Messages.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using MediatR;

namespace Kephas.Extensions.Tests.TestServices;

public interface IStreamReturnStringMessage : IStreamRequest<string>;

public record OneStreamReturnStringTypedMessage : IStreamReturnStringMessage;

public record TwoStreamReturnStringTypedMessage : IStreamReturnStringMessage;

public record StreamReturnStringMessage : IStreamRequest<string>;

public record StreamReturnIntMessage : IStreamRequest<int>;
