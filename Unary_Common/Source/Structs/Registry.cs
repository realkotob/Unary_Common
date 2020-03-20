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

using System.Collections.Generic;
using System.Linq;

using Newtonsoft.Json;

namespace Unary_Common.Structs
{
    public struct Registry
    {
        public List<uint> Free { get; set; }
        public Dictionary<string, uint> StringToUInt { get; set; }
        public Dictionary<uint, string> UIntToString { get; set; }

        [JsonIgnore]
        [MessagePack.IgnoreMember]
        private uint Counter;

        public void Init()
        {
            Counter = 0;

            if (StringToUInt.Count != 0)
            {
                foreach(var Index in StringToUInt)
                {
                    if(Index.Value > Counter)
                    {
                        Counter = Index.Value;
                    }
                }
            }
        }

        public void Clear()
        {
            StringToUInt.Clear();
            UIntToString.Clear();
            Free.Clear();
        }

        public bool Add(string Entry)
        {
            if(!StringToUInt.ContainsKey(Entry))
            {
                if (Free.Count != 0)
                {
                    StringToUInt[Entry] = Free[0];
                    UIntToString[Free[0]] = Entry;
                    Free.RemoveAt(0);
                }
                else
                {
                    StringToUInt[Entry] = Counter;
                    UIntToString[Counter] = Entry;
                    Counter++;
                }

                return true;
            }
            else
            {
                return false;
            }
        }

        public bool Remove(string Entry)
        {
            if (StringToUInt.ContainsKey(Entry))
            {
                Free.Add(StringToUInt[Entry]);
                UIntToString.Remove(StringToUInt[Entry]);
                StringToUInt.Remove(Entry);
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool Remove(uint Index)
        {
            if (UIntToString.ContainsKey(Index))
            {
                Free.Add(Index);
                StringToUInt.Remove(UIntToString[Index]);
                UIntToString.Remove(Index);
                return true;
            }
            else
            {
                return false;
            }
        }

        public void RemoveIfStartsWith(string Start)
        {
            foreach(var Entry in StringToUInt.ToList())
            {
                if(Entry.Key.StartsWith(Start))
                {
                    Remove(Entry.Key);
                }
            }
        }

        public uint Get(string Entry)
        {
            if(StringToUInt.ContainsKey(Entry))
            {
                return StringToUInt[Entry];
            }
            else
            {
                return uint.MaxValue;
            }
        }

        public string Get(uint Index)
        {
            if (UIntToString.ContainsKey(Index))
            {
                return UIntToString[Index];
            }
            else
            {
                return default;
            }
        }
    }
}