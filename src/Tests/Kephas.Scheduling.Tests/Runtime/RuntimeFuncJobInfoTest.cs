// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RuntimeFuncJobInfoTest.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the runtime function job information test class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Scheduling.Tests.Runtime
{
    using Kephas.Runtime;
    using Kephas.Scheduling.Jobs;
    using Kephas.Scheduling.Runtime;
    using NUnit.Framework;

    [TestFixture]
    public class RuntimeFuncJobInfoTest
    {
        [Test]
        public void CreateInstance()
        {
            var execution = 0;
            var jobInfo = new RuntimeFuncJobInfo(new RuntimeTypeRegistry(),  () => execution++);

            var job1 = (FuncJob)jobInfo.CreateInstance();
            var job2 = (FuncJob)jobInfo.CreateInstance();

            job1.Execute();
            job2.Execute();

            Assert.AreEqual(2, execution);
        }
    }
}
