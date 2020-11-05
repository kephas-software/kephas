// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Activities.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Workflow.Model.Tests.Models.ActivitiesModel
{
    public interface ILaughActivity : IActivity
    {
    }

    public interface IEnjoyActivity : IActivity
    {
        public string What { get; set; }
    }
}