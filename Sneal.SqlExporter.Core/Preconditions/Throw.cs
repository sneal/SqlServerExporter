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
    /// Static factory for generating argument preconditions.
    /// </summary>
    public static class Throw
    {
        public static void If(bool predicate)
        {
            if (predicate)
            {
                throw new ArgumentException("The argument did not meet the specified precondition");
            }
        }

        public static void If(bool predicate, string argumentName)
        {
            if (predicate)
            {
                throw new ArgumentException(
                    string.Format("The argument {0} did not meet the specified precondition", argumentName));
            }
        }

        public static Precondition<TArgument> If<TArgument>(TArgument argument, string argumentName)
        {
            return new Precondition<TArgument>(argument, argumentName);
        }

        public static Precondition<TArgument> If<TArgument>(TArgument argument)
        {
            return new Precondition<TArgument>(argument, null);
        }

        public static StringPrecondition If(string argument, string argumentName)
        {
            return new StringPrecondition(argument, argumentName);
        }

        public static StringPrecondition If(string argument)
        {
            return new StringPrecondition(argument, null);
        }
    }
}