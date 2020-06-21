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

namespace Unary.Common.Utils
{
    public static class StringUtil
    {
        private static List<int> OffsetBuffer = new List<int>();
        private static List<int> LengthBuffer = new List<int>();
        private static int Offset = 0;
        private static int Length = 0;
        private static int BufferCounter = -1;

        private static void AddPartToBuffer(int i)
        {
            OffsetBuffer.Add(Offset);
            LengthBuffer.Add(Length);
            Offset = i + 1;
            Length = 0;
            BufferCounter++;
        }

        private static void BuildBuffer(string Source, char NewTarget)
        {
            OffsetBuffer.Clear();
            LengthBuffer.Clear();
            Offset = 0;
            Length = 0;
            BufferCounter = -1;

            for (int i = 0; i < Source.Length; ++i)
            {
                if (Source[i] == NewTarget)
                {
                    AddPartToBuffer(i);
                }
                else if(i == Source.Length - 1)
                {
                    Length++;
                    AddPartToBuffer(i);
                }
                else
                {
                    Length++;
                }
            }
        }

        public static string GetPart(this string Source, char Target, int Index)
        {
            BuildBuffer(Source, Target);

            if(Index >= 0)
            {
                return Source.Substring(OffsetBuffer[Index], LengthBuffer[Index]);
            }
            else
            {
                return Source.Substring(OffsetBuffer[BufferCounter + Index], LengthBuffer[BufferCounter + Index]);
            }
        }

        public static string GetLastPart(this string Source, char Target)
        {
            BuildBuffer(Source, Target);

            return Source.Substring(OffsetBuffer[BufferCounter], LengthBuffer[BufferCounter]);
        }

        /*    From        To
         *    v           v
         *    Unary.Common.This.Is.A.Test
         */
        public static string GetPartsFromBeginToIndex(this string Source, char Target, int Index)
        {
            BuildBuffer(Source, Target);

            return Source.Substring(0, OffsetBuffer[Index] + LengthBuffer[Index]);
        }

        /*                From       To
         *                v          v
         *    Unary.Common.This.Is.A.Test
         */
        public static string GetPartsFromIndexToEnd(this string Source, char Target, int Index)
        {
            BuildBuffer(Source, Target);

            return Source.Substring(OffsetBuffer[Index]);
        }

        public static string ReplacePart(this string Source, char Target, int Index, string Replace)
        {
            BuildBuffer(Source, Target);

            return Source.Remove(OffsetBuffer[BufferCounter + Index], LengthBuffer[BufferCounter + Index])
            .Insert(OffsetBuffer[BufferCounter + Index], Replace);
        }

        public static int CountChar(this string Source, char Target)
        {
            Length = 0;

            for (int i = 0; i < Source.Length - 1; ++i)
            {
                if (Source[i] == Target)
                {
                    Length++;
                }
            }

            return Length;
        }
    }
}
