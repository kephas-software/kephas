// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DataAnnotationsBehaviorBaseTest.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Implements the data annotations behavior base test class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Data.Tests.Behaviors
{
    using System.ComponentModel.DataAnnotations;
    using System.Linq;

    using Kephas.Data.Behaviors;
    using Kephas.Data.Capabilities;
    using Kephas.Reflection;
    using Kephas.Runtime;

    using NUnit.Framework;

    using RangeAttribute = System.ComponentModel.DataAnnotations.RangeAttribute;

    [TestFixture]
    public class DataAnnotationsBehaviorBaseTest
    {
        [Test]
        public void Validate_RequiredAttribute()
        {
            var typeInfo = new RuntimeTypeInfo(typeof(ITestEntity));
            var testEntity = new TestEntity(typeInfo);
            var behavior = new StringDataAnnotationsBehavior();

            var result = behavior.Validate(testEntity, new EntityInfo(testEntity), null);
            Assert.AreEqual(1, result.Count());

            var validationItem = result.First();
            Assert.AreEqual(nameof(ITestEntity.Name), validationItem.MemberName);
            Assert.AreEqual("The Name field is required.", validationItem.Message);
        }

        [Test]
        public void Validate_RangeAttribute()
        {
            var typeInfo = new RuntimeTypeInfo(typeof(ITestEntity));
            var testEntity = new TestEntity(typeInfo) { Name = "gigi", Age = -1 };
            var behavior = new StringDataAnnotationsBehavior();

            var result = behavior.Validate(testEntity, new EntityInfo(testEntity), null);
            Assert.AreEqual(1, result.Count());

            var validationItem = result.First();
            Assert.AreEqual(nameof(ITestEntity.Age), validationItem.MemberName);
            Assert.AreEqual("The field Age must be between 0 and 130.", validationItem.Message);
        }

        [Test]
        public void Validate_multiple_properties()
        {
            var typeInfo = new RuntimeTypeInfo(typeof(ITestEntity));
            var testEntity = new TestEntity(typeInfo) { Name = null, Age = 200 };
            var behavior = new StringDataAnnotationsBehavior();

            var result = behavior.Validate(testEntity, new EntityInfo(testEntity), null);
            Assert.AreEqual(2, result.Count());
        }

        [Test]
        public void Validate_multiple_attributes()
        {
            var typeInfo = new RuntimeTypeInfo(typeof(ITestEntity));
            var testEntity = new TestEntity(typeInfo) { Name = "gigi", Email = "a@a" };
            var behavior = new StringDataAnnotationsBehavior();

            var result = behavior.Validate(testEntity, new EntityInfo(testEntity), null).ToList();
            Assert.AreEqual(2, result.Count);

            var validationItem = result[0];
            Assert.AreEqual(nameof(ITestEntity.Email), validationItem.MemberName);
            Assert.AreEqual("The field Email must be a string or array type with a minimum length of '4'.", validationItem.Message);
            validationItem = result[1];
            Assert.AreEqual(nameof(ITestEntity.Email), validationItem.MemberName);
            Assert.AreEqual("The Email field is not a valid e-mail address.", validationItem.Message);
        }

        public interface ITestEntity
        {
            [Required]
            string Name { get; set; }

            [Range(0, 130)]
            int Age { get; set; }

            [MinLength(4)]
            [EmailAddress]
            string Email { get; set; }
        }

        public class TestEntity : ITestEntity, IInstance
        {
            private readonly ITypeInfo typeInfo;

            public TestEntity(ITypeInfo typeInfo)
            {
                this.typeInfo = typeInfo;
            }

            public string Name { get; set; }

            public int Age { get; set; }

            public string Email { get; set; }

            /// <summary>
            /// Gets the type information for this instance.
            /// </summary>
            /// <returns>
            /// The type information.
            /// </returns>
            public ITypeInfo GetTypeInfo()
            {
                return this.typeInfo;
            }
        }
    }

    public class StringDataAnnotationsBehavior : DataAnnotationsBehaviorBase<DataAnnotationsBehaviorBaseTest.ITestEntity>
    {
    }
}