﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ArgsTest.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the application arguments test class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Core.Tests.Commands
{
    using System;
    using System.Linq;

    using Kephas.Commands;
    using Kephas.Dynamic;
    using NSubstitute;
    using NUnit.Framework;

    [TestFixture]
    public class ArgsTest
    {
        [Test]
        public void Args_string()
        {
            var args = new Args("-Hi=there -Hello=\"World\"");
            var dictArgs = args.ToDictionary();
            Assert.AreEqual(2, dictArgs.Count);
            Assert.AreEqual("there", args["Hi"]);
            Assert.AreEqual("World", args["Hello"]);
        }

        [Test]
        public void Args_args()
        {
            var args = new Args(new[] { "-Hi=there", "-Hello=\"World\"" });
            var dictArgs = args.ToDictionary();
            Assert.AreEqual(2, dictArgs.Count);
            Assert.AreEqual("there", args["Hi"]);
            Assert.AreEqual("World", args["Hello"]);
        }

        [Test]
        public void Args_args_missing_values()
        {
            var args = new Args(new[] { "-Hi=there", "run", "-again", "-Hello=\"World\"" });
            var dictArgs = args.ToDictionary();
            Assert.AreEqual(4, dictArgs.Count);
            Assert.AreEqual("there", args["Hi"]);
            Assert.AreEqual(true, args["run"]);
            Assert.AreEqual(true, args["again"]);
            Assert.AreEqual("World", args["Hello"]);
        }

        [Test]
        public void Args_args_with_override()
        {
            var args = new Args(new[] { "-Hi=there", "-hi", "you" });
            var dictArgs = args.ToDictionary();
            Assert.AreEqual(1, dictArgs.Count);
            Assert.AreEqual("you", args["Hi"]);
        }

        [Test]
        public void Args_args_dash_convention()
        {
            var args = new Args(new[] { "-Hi", "there", "--Hello", "\"World\"" });
            var dictArgs = args.ToDictionary();
            Assert.AreEqual(2, dictArgs.Count);
            Assert.AreEqual("there", args["Hi"]);
            Assert.AreEqual("World", args["Hello"]);
        }

        [Test]
        public void Args_args_dash_convention_last_arg_no_value()
        {
            var args = new Args(new[] { "-Hi" });
            var dictArgs = args.ToDictionary();
            Assert.AreEqual(1, dictArgs.Count);
            Assert.AreEqual(true, args["Hi"]);
        }

        [Test]
        public void Args_string_special_separators()
        {
            var args = new Args("-Hi=there \t-Hello=\"World\" \r\n-coming=True");
            var dictArgs = args.ToDictionary();
            Assert.AreEqual(3, dictArgs.Count);
            Assert.AreEqual("there", args["Hi"]);
            Assert.AreEqual("World", args["Hello"]);
            Assert.AreEqual("True", args["coming"]);
        }

        [Test]
        public void Args_string_special_separators_dash_convention()
        {
            var args = new Args("-Hi there \t-Hello \"World\" \r\n-coming \tTrue");
            var dictArgs = args.ToDictionary();
            Assert.AreEqual(3, dictArgs.Count);
            Assert.AreEqual("there", args["Hi"]);
            Assert.AreEqual("World", args["Hello"]);
            Assert.AreEqual("True", args["coming"]);
        }

        [Test]
        public void Args_expando()
        {
            var expando = new Expando { ["gigi"] = "belogea" };
            var args = new Args(expando);

            Assert.AreEqual("belogea", args["gigi"]);
        }

        [Test]
        public void ToCommandArgs_multiple()
        {
            var args = new Args(string.Empty)
            {
                ["b"] = true,
                ["empty"] = string.Empty,
                ["date"] = new DateTime(2020, 12, 25),
                ["int"] = 3,
            };

            var argArray = args.ToCommandArgs().ToArray();
            Assert.AreEqual("b=true", argArray[0]);
            Assert.AreEqual("empty=\"\"", argArray[1]);
            Assert.AreEqual("date=\"2020-12-25T00:00:00\"", argArray[2]);
            Assert.AreEqual("int=3", argArray[3]);
        }

        [Test]
        public void AsArgs_same()
        {
            var args = Substitute.For<IArgs>();
            Assert.AreSame(args, args.AsArgs());
        }

        [Test]
        public void AsArgs_different()
        {
            var args = new Expando();
            var appArgs = args.AsArgs();
            Assert.AreNotSame(args, appArgs);
        }
    }
}