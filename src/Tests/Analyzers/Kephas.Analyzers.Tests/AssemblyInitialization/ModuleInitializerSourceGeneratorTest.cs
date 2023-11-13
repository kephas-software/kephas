// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ModuleInitializerSourceGeneratorTest.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System.Reflection;
using System.Runtime.CompilerServices;
using Kephas.Analyzers.TestAssembly;
using NUnit.Framework;

namespace Kephas.Analyzers.Tests.AssemblyInitialization
{
    [TestFixture]
    public class ModuleInitializerSourceGeneratorTest
    {
        [Test]
        public void Execute_generated_ModuleInitializer()
        {
            var testAssembly = typeof(NoService).Assembly;

            var moduleInitializerType = testAssembly.GetTypes().SingleOrDefault(t => t.Name == "ModuleInitializer_Kephas_Analyzers_TestAssembly");
            Assert.NotNull(moduleInitializerType);
            Assert.AreEqual("Kephas.AssemblyInitialization.Generated", moduleInitializerType!.Namespace);

            var initializeMethod = moduleInitializerType.GetMethod("InitializeModule");
            Assert.NotNull(initializeMethod);
            Assert.IsTrue(initializeMethod!.IsStatic);
            Assert.NotNull(initializeMethod!.GetCustomAttribute<ModuleInitializerAttribute>());
        }
    }
}
