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

namespace Unary.Common.Utils
{
    public static class ModIDUtil
    {
        public static string FromType(this Type Target)
        {
            if(Target.Namespace != null)
            {
                return Target.FullName;
            }
            else
            {
                return null;
            }
        }

        public static bool Validate(string ModIDEntry)
        {
            if(ModIDEntry == null)
            {
                return false;
            }

            if(!ModIDEntry.Contains("."))
            {
                return false;
            }

            string[] Parts = ModIDEntry.Split('.');

            if (Parts.Length < 2)
            {
                return false;
            }

            return true;
        }

        public static string ModID(string ModIDEntry)
        {
            if(!Validate(ModIDEntry))
            {
                return null;
            }
            else
            {
                return ModIDEntry.GetFirstOccurrence(".");
            }
        }

        public static string ModIDEntry(string ModIDEntry)
        {
            if (!Validate(ModIDEntry))
            {
                return null;
            }
            else
            {
                return ModIDEntry.GetAfterFirstOccurrence(".");
            }
        }

        public static string ModIDTarget(string ModIDEntry)
        {
            if (!Validate(ModIDEntry))
            {
                return null;
            }
            else
            {
                return ModIDEntry.GetLastOccurence(".");
            }
        }

        public static List<string> GetCategories(string ModIDEntry)
        {
            List<string> Result = new List<string>();
            
            if(ModIDEntry == null)
            {
                return Result;
            }

            string[] Pieces = ModIDEntry.Split('.');

            if(Pieces.Length < 3)
            {
                return Result;
            }

            for (int i = 0; i < Pieces.Length - 1; ++i)
            {
                string Category = default;

                for(int k = 0; k < i + 1; ++k)
                {
                    Category += Pieces[k] + '.';
                }

                Category = Category.Substring(0, Category.Length - 1);

                Result.Add(Category);
            }

            return Result;
        }
    }
}
