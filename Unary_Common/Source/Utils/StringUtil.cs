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
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Unary_Common.Utils
{
    public static class StringUtil
    {
        public static string GetLastOccurence(this string Source, string Find)
        {
            int Place = Source.LastIndexOf(Find);
            return Source.Substring(Place, Source.Length - Place);
        }

        public static string ReplaceLastOccurrence(this string Source, string Find, string Replace)
        {
            int Place = Source.LastIndexOf(Find);
            string result = Source.Remove(Place, Find.Length).Insert(Place, Replace);
            return result;
        }

        public static string GetFirstOccurrence(this string Source, string Find)
        {
            int Place = Source.IndexOf(Find);
            return Source.Substring(0, Place);
        }

        public static string GetAfterFirstOccurrence(this string Source, string Find)
        {
            int Place = Source.IndexOf(Find);
            Place++;
            return Source.Substring(Place, Source.Length - Place);
        }

        public static string ReplaceFirstOccurrence(this string Source, string Find, string Replace)
        {
            int Place = Source.IndexOf(Find);
            return Replace + Source.Substring(Place, Source.Length - Place);
        }

        public static int CountCharOccurrences(this string Source, char Target)
        {
            int Result = 0;

            for(int i = Source.Length - 1; i >= 0; --i)
            {
                if(Source[i] == Target)
                {
                    Result++;
                }
            }

            return Result;
        }
    }
}
