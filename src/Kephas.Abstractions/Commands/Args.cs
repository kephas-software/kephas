// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Args.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the application arguments class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Commands
{
    using System;
    using System.Collections.Generic;

    using Kephas.Collections;
    using Kephas.Dynamic;

    /// <summary>
    /// Arguments used in command line execution.
    /// </summary>
    public class Args : Expando<object?>, IArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Args"/> class.
        /// </summary>
        public Args()
            : this(ComputeArgs(string.Empty))
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Args"/> class.
        /// </summary>
        /// <param name="appArgs">The application arguments.</param>
        public Args(IEnumerable<string> appArgs)
            : this(ComputeArgs(appArgs))
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Args"/> class.
        /// </summary>
        /// <param name="commandLine">The command line.</param>
        public Args(string commandLine)
            : this(ComputeArgs(commandLine))
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Args"/> class.
        /// </summary>
        /// <param name="argValues">The argument values.</param>
        public Args(IDictionary<string, object?> argValues)
            : base(ComputeArgs(argValues))
        {
            this.MemberBinders = ExpandoMemberBinderKind.InnerDictionary;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Args"/> class.
        /// </summary>
        /// <param name="args">The argument values.</param>
        public Args(IDynamic args)
            : base(ComputeArgs(args.ToDictionary()))
        {
            this.MemberBinders = ExpandoMemberBinderKind.InnerDictionary;
        }

        /// <summary>
        /// Converts this app arguments list to a list of string arguments for use in command lines.
        /// </summary>
        /// <returns>A list of string arguments.</returns>
        public virtual IEnumerable<string> ToCommandArgs()
        {
            return this.ToDictionary().ToCommandArgs();
        }

        /// <summary>
        /// Returns a string that represents the current object.
        /// </summary>
        /// <returns>
        /// A string that represents the current object.
        /// </returns>
        public override string ToString()
        {
            return string.Join(" ", this.ToCommandArgs());
        }

        /// <summary>
        /// Calculates the arguments as a dictionary of values.
        /// </summary>
        /// <param name="args">The arguments.</param>
        /// <returns>
        /// The calculated arguments.
        /// </returns>
        protected static IDictionary<string, object?> ComputeArgs(IDictionary<string, object?> args)
        {
            args = args ?? throw new ArgumentNullException(nameof(args));

            var dictionary = new Dictionary<string, object?>(StringComparer.OrdinalIgnoreCase);
            dictionary.AddRange(args);
            return dictionary;
        }

        /// <summary>
        /// Calculates the arguments as a dictionary of values.
        /// </summary>
        /// <param name="commandLine">The command line.</param>
        /// <returns>
        /// The calculated arguments.
        /// </returns>
        protected static IDictionary<string, object?> ComputeArgs(string commandLine)
        {
            commandLine = commandLine ?? throw new ArgumentNullException(nameof(commandLine));

            var args = commandLine.Split(new[] { ' ', '\r', '\n', '\t' }, new[] { '"' });
            return ComputeArgs(args);
        }

        /// <summary>
        /// Calculates the arguments as a dictionary of values.
        /// </summary>
        /// <param name="args">The application arguments.</param>
        /// <returns>
        /// The calculated arguments.
        /// </returns>
        protected static IDictionary<string, object?> ComputeArgs(IEnumerable<string> args)
        {
            args = args ?? throw new ArgumentNullException(nameof(args));

            var cmdArgs = new Dictionary<string, object?>(StringComparer.OrdinalIgnoreCase);

            var key = string.Empty;
            object? value = null;
            var expectedValue = false;

            using var enumerator = args.GetEnumerator();
            while (enumerator.MoveNext())
            {
                var currentArg = enumerator.Current;
                var keyStartIndex = 0;

                if (currentArg.StartsWith("--"))
                {
                    keyStartIndex = 2;
                }
                else if (currentArg.StartsWith("-"))
                {
                    keyStartIndex = 1;
                }
                else if (currentArg.StartsWith("/"))
                {
                    // "/SomeSwitch" is equivalent to "--SomeSwitch"
                    keyStartIndex = 1;
                }

                // if we received a new argument, but we expected a value, add the previous key with the value "true"
                if (expectedValue)
                {
                    expectedValue = false;

                    if (keyStartIndex > 0)
                    {
                        // set the previous key to true and continue with processing the current arg
                        cmdArgs[key] = true;
                    }
                    else
                    {
                        cmdArgs[key] = Unescape(currentArg);
                        continue;
                    }
                }

                // currentArg starts a new argument
                var separator = currentArg.IndexOf('=');

                if (separator >= 0)
                {
                    // currentArg specifies a key with value
                    key = Unescape(currentArg.Substring(keyStartIndex, separator - keyStartIndex));
                    value = Unescape(currentArg.Substring(separator + 1));
                }
                else
                {
                    // currentArg specifies only a key
                    // If there is no prefix in current argument, consider it as a key with value "true"
                    if (keyStartIndex == 0)
                    {
                        key = Unescape(currentArg);
                        value = true;
                    }
                    else
                    {
                        key = Unescape(currentArg.Substring(keyStartIndex));
                        expectedValue = true;
                    }
                }

                // Override value when key is duplicated. So we always have the last argument win.
                if (!expectedValue)
                {
                    cmdArgs[key] = value;
                }
            }

            if (expectedValue)
            {
                cmdArgs[key] = true;
            }

            return cmdArgs;
        }

        /// <summary>
        /// Attempts to set the value with the given key.
        /// </summary>
        /// <remarks>
        /// First of all, it is tried to set the property value to the inner object, if one is set.
        /// The next try is to set the property value to the expando object itself.
        /// Lastly, if still a property by the provided name cannot be found, the inner dictionary is used to set the value with the provided key.
        /// </remarks>
        /// <param name="key">The key.</param>
        /// <param name="value">The value to set.</param>
        /// <returns>
        /// <c>true</c> if the value could be set, <c>false</c> otherwise.
        /// </returns>
        protected override bool TrySetValue(string key, object? value)
        {
            // check only the dictionary for member
            if (value == null)
            {
                this.InnerDictionary.Remove(key);
            }
            else
            {
                this.InnerDictionary[key] = value;
            }

            return true;
        }

        /// <summary>
        /// Attempts to get the dynamic value with the given key.
        /// </summary>
        /// <remarks>
        /// First of all, it is tried to get a property value from the inner object, if one is set.
        /// The next try is to retrieve the property value from the expando object itself.
        /// Lastly, if still a property by the provided name cannot be found, the inner dictionary is searched by the provided key.
        /// </remarks>
        /// <param name="key">The key.</param>
        /// <param name="value">The value to get.</param>
        /// <returns>
        /// <c>true</c> if a value is found, <c>false</c> otherwise.
        /// </returns>
        protected override bool TryGetValue(string key, out object? value)
        {
            // check only the dictionary for member
            return this.InnerDictionary.TryGetValue(key, out value);
        }

        private static string Unescape(string value)
        {
            if (value.StartsWith('"') && value.EndsWith('"'))
            {
                value = value[1..^1];
                return value.Replace("\\\"", "\"");
            }

            return value;
        }
    }
}
