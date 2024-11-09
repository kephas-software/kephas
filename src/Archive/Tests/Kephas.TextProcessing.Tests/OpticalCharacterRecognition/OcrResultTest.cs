// --------------------------------------------------------------------------------------------------------------------
// <copyright file="OcrResultTest.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.TextProcessing.Tests.OpticalCharacterRecognition
{
    using System;

    using Kephas.TextProcessing.OpticalCharacterRecognition;
    using NUnit.Framework;

    [TestFixture]
    public class OcrResultTest
    {
        [Test]
        public void ToString_empty()
        {
            var result = new OcrResult();
            Assert.IsEmpty(result.ToString());
        }

        [Test]
        public void ToString_multiple_lines_and_words()
        {
            var result = new OcrResult
            {
                Pages = new[]
                {
                    new OcrPage
                    {
                        Lines = new[]
                        {
                            new OcrLine
                            {
                                Words = new[]
                                {
                                    new OcrWord { Text = "hi", BoundingBox = new[] { 1, 2, 3, 4, 5, 6, 7, 8 } },
                                    new OcrWord { Text = "there", BoundingBox = new[] { 11, 12, 13, 14, 15, 16, 17, 18 } },
                                },
                            },
                            new OcrLine
                            {
                                Words = new[]
                                {
                                    new OcrWord { Text = "my" },
                                    new OcrWord { Text = "friend" },
                                },
                            },
                        },
                    },
                },
            };

            Assert.AreEqual($"hi there{Environment.NewLine}my friend{Environment.NewLine}", result.ToString());
        }
    }
}