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

using Unary.Common.Collections;

namespace Unary.Common.Structs
{
    public struct Registry
    {
        public List<uint> Free { get; set; }
        public BiDictionary<string, uint> Entries;
        private uint Counter;

        public void Init()
        {
            Free = new List<uint>();
            Entries = new BiDictionary<string, uint>();
            Counter = 0;
        }

        public void Clear()
        {
            Free.Clear();
            Entries.Clear();
        }

        public bool Add(string Entry)
        {
            if(!Entries.KeyExists(Entry))
            {
                if (Free.Count != 0)
                {
                    Entries.Set(Entry, Free[0]);
                    Free.RemoveAt(0);
                }
                else
                {
                    Entries.Set(Entry, Counter);
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
            if(Entries.KeyExists(Entry))
            {
                Free.Add(Entries.GetValue(Entry));
                Entries.Remove(Entry);
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool Remove(uint Index)
        {
            if (Entries.ValueExists(Index))
            {
                Free.Add(Index);
                Entries.Remove(Index);
                return true;
            }
            else
            {
                return false;
            }
        }

        public void RemoveIfStartsWith(string Start)
        {
            foreach(var Entry in Entries.GetAllKeys())
            {
                if(Entry.StartsWith(Start))
                {
                    Entries.Remove(Entry);
                }
            }
        }

        public uint Get(string Entry)
        {
            return Entries.GetValue(Entry);
        }

        public string Get(uint Index)
        {
            return Entries.GetKey(Index);
        }

        public void Set(string Key, uint Value)
        {
            Entries.Set(Key, Value);
        }
    }
}