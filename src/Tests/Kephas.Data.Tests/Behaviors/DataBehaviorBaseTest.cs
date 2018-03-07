// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DataBehaviorBaseTest.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Implements the data behavior base test class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Data.Tests.Behaviors
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;

    using Kephas.Data.Behaviors;
    using Kephas.Data.Capabilities;
    using Kephas.Data.Validation;

    using NUnit.Framework;

    [TestFixture]
    public class DataBehaviorBaseTest
    {
        [Test]
        public async Task ValidateAsync_uses_Validate()
        {
            var behavior = new StringDataBehavior((e, ei, ctx) => e == "123" ? DataValidationResult.Success : new DataValidationResult("bad param"));

            var result = await behavior.ValidateAsync("123", null, null, default);
            Assert.IsFalse(result.HasErrors());

            result = await behavior.ValidateAsync("321", null, null, default);
            Assert.IsTrue(result.HasErrors());
            Assert.AreEqual("bad param", result.FirstOrDefault().Message);
        }
    }

    public class StringDataBehavior : DataBehaviorBase<string>
    {
        private readonly Func<string, IEntityInfo, IDataOperationContext, IDataValidationResult> validateFn;

        public StringDataBehavior(
            Func<string, IEntityInfo, IDataOperationContext, IDataValidationResult> validateFn = null)
        {
            this.validateFn = validateFn;
        }

        /// <summary>
        /// Callback invoked after upon entity validation.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <param name="entityInfo">The entity information.</param>
        /// <param name="operationContext">The operation context.</param>
        /// <returns>
        /// An <see cref="IDataValidationResult"/>.
        /// </returns>
        public override IDataValidationResult Validate(string entity, IEntityInfo entityInfo, IDataOperationContext operationContext)
        {
            return this.validateFn?.Invoke(entity, entityInfo, operationContext) ?? DataValidationResult.Success;
        }
    }
}