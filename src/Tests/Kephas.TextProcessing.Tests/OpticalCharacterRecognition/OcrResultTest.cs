// --------------------------------------------------------------------------------------------------------------------
// <copyright file="OcrResultTest.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System;

namespace Kephas.TextProcessing.Tests.OpticalCharacterRecognition
{
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
                Regions = new[]
                {
                    new OcrRegion
                    {
                        Lines = new[]
                        {
                            new OcrLine
                            {
                                Words = new[]
                                {
                                    new OcrWord { Text = "hi", BoundingBox = "box1" },
                                    new OcrWord { Text = "there", BoundingBox = "box1" },
                                },
                            },
                            new OcrLine
                            {
                                Words = new[]
                                {
                                    new OcrWord { Text = "my", BoundingBox = "box1" },
                                    new OcrWord { Text = "friend", BoundingBox = "box1" },
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