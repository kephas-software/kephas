// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DataBehaviorBaseTest.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
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
    using Kephas.Validation;
    using NUnit.Framework;

    [TestFixture]
    public class DataBehaviorBaseTest
    {
        [Test]
        public async Task ValidateAsync_uses_Validate()
        {
            var behavior = new StringDataBehavior((e, ei, ctx) => e == "123" ? ValidationResult.Success : new ValidationResult("bad param"));

            var result = await behavior.ValidateAsync("123", null, null, default);
            Assert.IsFalse(result.HasErrors());

            result = await behavior.ValidateAsync("321", null, null, default);
            Assert.IsTrue(result.HasErrors());
            Assert.AreEqual("bad param", result.FirstOrDefault().Message);
        }
    }

    public class StringDataBehavior : DataBehaviorBase<string>
    {
        private readonly Func<string, IEntityEntry, IDataOperationContext, IValidationResult> validateFn;

        public StringDataBehavior(
            Func<string, IEntityEntry, IDataOperationContext, IValidationResult> validateFn = null)
        {
            this.validateFn = validateFn;
        }

        /// <summary>
        /// Callback invoked after upon entity validation.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <param name="entityEntry">The entity entry.</param>
        /// <param name="operationContext">The operation context.</param>
        /// <returns>
        /// An <see cref="IValidationResult"/>.
        /// </returns>
        public override IValidationResult Validate(string entity, IEntityEntry entityEntry, IDataOperationContext operationContext)
        {
            return this.validateFn?.Invoke(entity, entityEntry, operationContext) ?? ValidationResult.Success;
        }
    }
}