// --------------------------------------------------------------------------------------------------------------------
// <copyright file="QualifiedFullNameTest.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the qualified full name test class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Core.Tests.Reflection
{
    using Kephas.Reflection;

    using NUnit.Framework;

    [TestFixture]
    public class QualifiedFullNameTest
    {
        [Test]
        public void QualifiedFullName_generic_with_assembly_type_param_with_assembly()
        {
            var qname = new QualifiedFullName("Generic`1[[Full.Type.Name, AssemblyName]], GenericAssemblyName, Version, Token");

            Assert.AreEqual("GenericAssemblyName", qname.AssemblyName.FullName);
            Assert.AreEqual("Generic`1[[Full.Type.Name, AssemblyName]]", qname.TypeName);
        }

        [Test]
        public void QualifiedFullName_generic_without_assembly_type_param_with_assembly()
        {
            var qname = new QualifiedFullName("Generic`1[[Full.Type.Name, AssemblyName]]");

            Assert.IsNull(qname.AssemblyName);
            Assert.AreEqual("Generic`1[[Full.Type.Name, AssemblyName]]", qname.TypeName);
        }

        [Test]
        public void QualifiedFullName_with_assembly()
        {
            var qname = new QualifiedFullName("Full.Type.Name, AssemblyName, Version, Token");

            Assert.AreEqual("AssemblyName", qname.AssemblyName.FullName);
            Assert.AreEqual("Full.Type.Name", qname.TypeName);
        }

        [Test]
        public void QualifiedFullName_without_assembly()
        {
            var qname = new QualifiedFullName("Full.Type.Name");

            Assert.IsNull(qname.AssemblyName);
            Assert.AreEqual("Full.Type.Name", qname.TypeName);
        }
    }
}