// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AppRunResult.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Application;

/// <summary>
/// The result of an application run operation.
/// </summary>
public record AppRunResult(IAppContext? AppContext, AppShutdownInstruction Instruction);
