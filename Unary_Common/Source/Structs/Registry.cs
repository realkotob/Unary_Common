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
        public Dictionary<string, uint> Busy { get; set; }

        [JsonIgnore]
        [MessagePack.IgnoreMember]
        private uint Counter;

        public void Init()
        {
            Counter = 0;

            if (Busy.Count != 0)
            {
                foreach(var Index in Busy)
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
            Busy.Clear();
            Free.Clear();
        }

        public bool Add(string Entry)
        {
            if(!Busy.ContainsKey(Entry))
            {
                if (Free.Count != 0)
                {
                    Busy[Entry] = Free[0];
                    Free.RemoveAt(0);
                }
                else
                {
                    Busy[Entry] = Counter;
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
            if (Busy.ContainsKey(Entry))
            {
                Free.Add(Busy[Entry]);
                Busy.Remove(Entry);
                return true;
            }
            else
            {
                return false;
            }
        }

        public void RemoveIfStartsWith(string Start)
        {
            foreach(var Entry in Busy.ToList())
            {
                if(Entry.Key.StartsWith(Start))
                {
                    Remove(Entry.Key);
                }
            }
        }

        public uint Get(string Entry)
        {
            if(Busy.ContainsKey(Entry))
            {
                return Busy[Entry];
            }
            else
            {
                return uint.MaxValue;
            }
        }
    }
}