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

namespace Unary_Common.Structs
{
    public struct Mod
    {
        public string ModID;
        public Version Version;
        public string Path;

        public override string ToString()
        {
            return ModID + '-' + Version.ToString();
        }

        public override int GetHashCode()
        {
            string Result = ModID + Version.ToString();
            return Result.GetHashCode();
        }

        public static bool operator ==(Mod Current, Mod Target)
        {
            if (Current.ModID == Target.ModID && 
            Current.Version == Target.Version)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public static bool operator !=(Mod Current, Mod Target)
        {
            if (Current.ModID != Target.ModID &&
            Current.Version != Target.Version)
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
            if (obj is Mod Target)
            {
                if (ModID == Target.ModID &&
                    Version == Target.Version)
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
