// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FuncJobTest.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the function job test class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Scheduling.Tests.Jobs
{
    using Kephas.Scheduling.Jobs;
    using Kephas.Scheduling.Runtime;
    using NUnit.Framework;

    [TestFixture]
    public class FuncJobTest
    {
        [Test]
        public void GetTypeInfo_is_RuntimeFuncJobInfo()
        {
            var execution = 0;
            var job = new FuncJob(() => execution++);

            var jobInfo = job.GetTypeInfo();

            Assert.IsInstanceOf<RuntimeFuncJobInfo>(jobInfo);
        }
    }
}
