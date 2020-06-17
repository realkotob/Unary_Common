/*
MIT License

Copyright (c) 2020 Unary Incorporated

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.
*/

using System;
using System.Collections.Generic;

namespace Unary.Common.Structs
{
    public struct Version
    {
        public int Major;
        public int Minor;
        public int Patch;

        public override string ToString()
        {
            string Result = default;
            Result += Major + '.' + Minor + '.' + Patch;
            return Result;
        }

        public override int GetHashCode()
        {
            return ToString().GetHashCode();
        }

        public static bool operator< (Version Current, Version Target)
        {
            if(Current.Major < Target.Major &&
               Current.Minor < Target.Minor &&
               Current.Patch < Target.Patch)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public static bool operator> (Version Current, Version Target)
        {
            if (Current.Major > Target.Major &&
               Current.Minor > Target.Minor &&
               Current.Patch > Target.Patch)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public static bool operator== (Version Current, Version Target)
        {
            if (Current.Major == Target.Major &&
               Current.Minor == Target.Minor &&
               Current.Patch == Target.Patch)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public static bool operator!= (Version Current, Version Target)
        {
            if (Current.Major != Target.Major &&
               Current.Minor != Target.Minor &&
               Current.Patch != Target.Patch)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public static bool operator >= (Version Current, Version Target)
        {
            if (Current.Major >= Target.Major &&
               Current.Minor >= Target.Minor &&
               Current.Patch >= Target.Patch)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public static bool operator <= (Version Current, Version Target)
        {
            if (Current.Major <= Target.Major &&
               Current.Minor <= Target.Minor &&
               Current.Patch <= Target.Patch)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public override bool Equals(object obj)
        {
            if (obj is Version Target)
            {
                if (Major == Target.Major &&
                    Minor == Target.Minor &&
                    Patch == Target.Patch)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }
    }
}
