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
            var qname = new QualifiedFullName("Full.Type.Generic`1[[Full.Type.Name, AssemblyName]], GenericAssemblyName, Version, Token");

            Assert.AreEqual("GenericAssemblyName", qname.AssemblyName.FullName);
            Assert.AreEqual("Full.Type.Generic`1[[Full.Type.Name, AssemblyName]]", qname.TypeName);
            Assert.AreEqual("Generic`1[[Full.Type.Name, AssemblyName]]", qname.Name);
            Assert.AreEqual("Full.Type", qname.Namespace);
        }

        [Test]
        public void QualifiedFullName_generic_with_assembly_type_param_with_assembly_no_namespace()
        {
            var qname = new QualifiedFullName("Generic`1[[Full.Type.Name, AssemblyName]], GenericAssemblyName, Version, Token");

            Assert.AreEqual("GenericAssemblyName", qname.AssemblyName.FullName);
            Assert.AreEqual("Generic`1[[Full.Type.Name, AssemblyName]]", qname.TypeName);
            Assert.AreEqual("Generic`1[[Full.Type.Name, AssemblyName]]", qname.Name);
            Assert.IsNull(qname.Namespace);
        }

        [Test]
        public void QualifiedFullName_generic_without_assembly_type_param_with_assembly()
        {
            var qname = new QualifiedFullName("Full.Type.Generic`1[[Full.Type.Name, AssemblyName]]");

            Assert.IsNull(qname.AssemblyName);
            Assert.AreEqual("Full.Type.Generic`1[[Full.Type.Name, AssemblyName]]", qname.TypeName);
            Assert.AreEqual("Generic`1[[Full.Type.Name, AssemblyName]]", qname.Name);
            Assert.AreEqual("Full.Type", qname.Namespace);
        }

        [Test]
        public void QualifiedFullName_generic_without_assembly_type_param_with_assembly_no_namespace()
        {
            var qname = new QualifiedFullName("Generic`1[[Full.Type.Name, AssemblyName]]");

            Assert.IsNull(qname.AssemblyName);
            Assert.AreEqual("Generic`1[[Full.Type.Name, AssemblyName]]", qname.TypeName);
            Assert.AreEqual("Generic`1[[Full.Type.Name, AssemblyName]]", qname.Name);
            Assert.IsNull(qname.Namespace);
        }

        [Test]
        public void GetGenericTypeArguments_fail()
        {
            var qname = new QualifiedFullName("My.Type");

            Assert.Throws<InvalidOperationException>(() => qname.GetGenericTypeArguments());
        }

        [TestCase("Generic`1[[Full.Type.Name, AssemblyName],[Another.Type]]", 2, "Full.Type.Name", "Another.Type")]
        [TestCase("Full.Type.Generic`1[[Full.Type.Name, AssemblyName]]", 1, "Full.Type.Name", null)]
        public void GetGenericTypeArguments_returns_open_generic(string fullName, int count, string type1, string? type2)
        {
            var qname = new QualifiedFullName(fullName);

            var args = qname.GetGenericTypeArguments().ToList();

            Assert.AreEqual(count, args.Count);
            Assert.AreEqual(type1, args[0].TypeName);
            if (count > 1)
            {
                Assert.AreEqual(type2, args[1].TypeName);
            }
        }

        [TestCase("Generic`1[[Full.Type.Name, AssemblyName]]", "Generic`1", null)]
        [TestCase("Full.Type.Generic`1[[Full.Type.Name, AssemblyName]]", "Full.Type.Generic`1", null)]
        [TestCase("Full.Type.Generic`1[[Full.Type.Name, AssemblyName]], GenericAssemblyName, Version, Token", "Full.Type.Generic`1", "GenericAssemblyName")]
        public void GetGenericTypeDefinition_returns_open_generic(string fullName, string openGenericTypeName, string? assemblyName)
        {
            var qname = new QualifiedFullName(fullName);

            var qgeneric = qname.GetGenericTypeDefinition();

            Assert.IsNotNull(qgeneric.TypeName);
            Assert.AreEqual(qgeneric!.TypeName, openGenericTypeName);
            Assert.AreEqual(qgeneric!.AssemblyName?.Name, assemblyName);
        }

        [Test]
        public void GetGenericTypeDefinition_not_generic_returns_null()
        {
            var qname = new QualifiedFullName("Full.Type.Name, AssemblyName, Version, Token");

            Assert.IsNull(qname.GetGenericTypeDefinition());
        }

        [Test]
        public void QualifiedFullName_with_assembly()
        {
            var qname = new QualifiedFullName("Full.Type.Name, AssemblyName, Version, Token");

            Assert.AreEqual("AssemblyName", qname.AssemblyName.FullName);
            Assert.AreEqual("Full.Type.Name", qname.TypeName);
            Assert.AreEqual("Name", qname.Name);
            Assert.AreEqual("Full.Type", qname.Namespace);
        }

        [Test]
        public void QualifiedFullName_with_assembly_no_namespace()
        {
            var qname = new QualifiedFullName("Name, AssemblyName, Version, Token");

            Assert.AreEqual("AssemblyName", qname.AssemblyName.FullName);
            Assert.AreEqual("Name", qname.TypeName);
            Assert.AreEqual("Name", qname.Name);
            Assert.IsNull(qname.Namespace);
        }

        [Test]
        public void QualifiedFullName_without_assembly()
        {
            var qname = new QualifiedFullName("Full.Type.Name");

            Assert.IsNull(qname.AssemblyName);
            Assert.AreEqual("Full.Type.Name", qname.TypeName);
            Assert.AreEqual("Name", qname.Name);
            Assert.AreEqual("Full.Type", qname.Namespace);
        }

        [Test]
        public void QualifiedFullName_without_assembly_no_namespace()
        {
            var qname = new QualifiedFullName("Name");

            Assert.IsNull(qname.AssemblyName);
            Assert.AreEqual("Name", qname.TypeName);
            Assert.AreEqual("Name", qname.Name);
            Assert.IsNull(qname.Namespace);
        }
    }
}