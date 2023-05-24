// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PipelineContext.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using Kephas.Services;

namespace Kephas.Pipelines
{
    /// <summary>
    /// Service providing a pipeline context.
    /// </summary>
    [AppServiceContract]
    public class PipelineContext : Context
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PipelineContext"/> class.
        /// </summary>
        /// <param name="serviceProvider"></param>
        public PipelineContext(IServiceProvider serviceProvider)
            : base(serviceProvider, true)
        {
        }
    }
}