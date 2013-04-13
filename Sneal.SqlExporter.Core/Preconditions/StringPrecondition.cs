#region license
// Copyright 2008 Shawn Neal (neal.shawn@gmail.com)
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//     http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
#endregion

using System;
using System.Text.RegularExpressions;

namespace Sneal.SqlExporter.Core.Preconditions
{
    /// <summary>
    /// Precondition class just for string types.  This class has precondition
    /// checks specific to string types.
    /// </summary>
    public class StringPrecondition : Precondition<string>
    {
        public StringPrecondition(string argument, string argumentName)
            : base(argument, argumentName)
        {
        }

        /// <summary>
        /// Throws an ArgumentException if the argument is an empty string.
        /// </summary>
        public void IsEmpty()
        {
            if (argument == string.Empty)
            {
                throw new ArgumentException(
                    string.Format("The string argument {0} cannot be empty", argumentName),
                    argumentName);
            }
        }

        /// <summary>
        /// Throws an ArgumentException if the argument is an empty string, or
        /// an ArgumentNullException if the argument is null.
        /// </summary>
        public void IsNullOrEmpty()
        {
            IsNull();
            IsEmpty();
        }

        /// <summary>
        /// Throws an ArgumentException if the argument matches the specified
        /// regular expression.
        /// </summary>
        /// <param name="regexExpression">The regular expression to match on.</param>
        public void Matches(string regexExpression)
        {
            Throw.If(regexExpression, "regexExpression").IsNullOrEmpty();

            Regex regex = new Regex(regexExpression);
            if (regex.Match(argument).Success)
            {
                string msg = string.Format(
                    "The string argument {0} cannot match the regular expression {1}",
                    argumentName,
                    regexExpression);

                throw new ArgumentException(msg, argumentName);
            }
        }

        /// <summary>
        /// Throws an ArgumentException if the argument does not match the specified
        /// regular expression.
        /// </summary>
        /// <param name="regexExpression">The regular expression to match on.</param>
        public void DoesNotMatch(string regexExpression)
        {
            Throw.If(regexExpression, "regexExpression").IsNullOrEmpty();

            Regex regex = new Regex(regexExpression);
            if (!regex.Match(argument).Success)
            {
                string msg = string.Format(
                    "The string argument {0} must match the regular expression {1}",
                    argumentName,
                    regexExpression);

                throw new ArgumentException(msg, argumentName);
            }
        }

        /// <summary>
        /// Throws an ArgumentException if the string argument is null or if the
        /// length is less than the specified length.
        /// </summary>
        /// <param name="length">The minimum length.</param>
        public void LengthIsLessThan(int length)
        {
            Throw.If(argument, argumentName).IsNull();

            if (argument.Length < length)
            {
                string msg = string.Format(
                    "The string argument {0} length must not be less than {1}",
                    argumentName,
                    length);

                throw new ArgumentException(msg, argumentName);
            }
        }

        /// <summary>
        /// Throws an ArgumentException if the string argument is null or if the
        /// length is less than or equal to the specified length.
        /// </summary>
        /// <param name="length">The minimum length (inclusive).</param>
        public void LengthIsLessThanOrEqualTo(int length)
        {
            Throw.If(argument, argumentName).IsNull();

            if (argument.Length <= length)
            {
                string msg = string.Format(
                    "The string argument {0} length must not be less than or equal to {1}",
                    argumentName,
                    length);

                throw new ArgumentException(msg, argumentName);
            }
        }

        /// <summary>
        /// Throws an ArgumentException if the string argument is null or if the
        /// length is greater than the specified length.
        /// </summary>
        /// <param name="length">The maximum length.</param>
        public void LengthIsGreaterThan(int length)
        {
            Throw.If(argument, argumentName).IsNull();

            if (argument.Length > length)
            {
                string msg = string.Format(
                    "The string argument {0} length must not be greater than {1}",
                    argumentName,
                    length);

                throw new ArgumentException(msg, argumentName);
            }
        }

        /// <summary>
        /// Throws an ArgumentException if the string argument is null or if the
        /// length is greater than or equal to the specified length.
        /// </summary>
        /// <param name="length">The maximum length (inclusive).</param>
        public void LengthIsGreaterThanOrEqualTo(int length)
        {
            Throw.If(argument, argumentName).IsNull();

            if (argument.Length >= length)
            {
                string msg = string.Format(
                    "The string argument {0} length must not be greater than or equal to {1}",
                    argumentName,
                    length);

                throw new ArgumentException(msg, argumentName);
            }
        }
    }
}