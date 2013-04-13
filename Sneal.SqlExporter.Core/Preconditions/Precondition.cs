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

namespace Sneal.SqlExporter.Core.Preconditions
{
    /// <summary>
    /// Base class for generic method argument preconditions.
    /// </summary>
    /// <typeparam name="TArgument">The argument type to apply the precondition to.</typeparam>
    public class Precondition<TArgument>
    {
        protected TArgument argument;
        protected string argumentName;

        public Precondition(TArgument argument, string argumentName)
        {
            this.argument = argument;
            this.argumentName = argumentName;
        }

        /// <summary>
        /// Throws an ArgumentNullException if the argument is null.
        /// </summary>
        public void IsNull()
        {
            if (argument == null)
            {
                throw new ArgumentNullException(
                    argumentName,
                    string.Format("The argument {0} cannot be null", argumentName));
            }
        }

        /// <summary>
        /// Throws an ArgumentException if the argument is equal.
        /// </summary>
        /// <param name="equalTo">The instance to compare equality to.</param>
        public void IsEqualTo(TArgument equalTo)
        {
            if (equalTo.Equals(argument))
            {
                throw new ArgumentException(
                    argumentName,
                    string.Format("The argument {0} cannot be equal to {1}", argumentName, equalTo));
            }
        }

        /// <summary>
        /// Throws an ArgumentException if the argument is less than the
        /// comparable instance.
        /// </summary>
        /// <param name="comparer">The instance to compare to.</param>
        public void IsLessThan(IComparable<TArgument> comparer)
        {
            if (comparer.CompareTo(argument) == 1)
            {
                throw new ArgumentException(
                    argumentName,
                    string.Format("The argument {0} cannot be less than {1}", argumentName, comparer));
            }
        }

        /// <summary>
        /// Throws an ArgumentException if the argument is greater than the
        /// comparable instance.
        /// </summary>
        /// <param name="comparer">The instance to compare to.</param>
        public void IsGreaterThan(IComparable<TArgument> comparer)
        {
            if (comparer.CompareTo(argument) == -1)
            {
                throw new ArgumentException(
                    argumentName,
                    string.Format("The argument {0} cannot be greater than {1}", argumentName, comparer));
            }
        }

        /// <summary>
        /// Throws an ArgumentException if the argument is comparable to the
        /// comparer instance.
        /// </summary>
        /// <param name="comparer">The instance to compare to.</param>
        public void IsComparableTo(IComparable<TArgument> comparer)
        {
            if (comparer.CompareTo(argument) == 0)
            {
                throw new ArgumentException(
                    argumentName,
                    string.Format("The argument {0} cannot be comparable to {1}", argumentName, comparer));
            }
        }
    }
}