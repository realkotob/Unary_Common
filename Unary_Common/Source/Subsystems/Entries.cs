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

using Unary_Common.Utils;

using System;
using System.Collections.Generic;
using System.Linq;

using MessagePack;

namespace Unary_Common.Structs
{
    public class Entries
    {
        private Dictionary<string, byte[]> CoreEntries = new Dictionary<string, byte[]>();
        private Dictionary<string, byte[]> ModEntries = new Dictionary<string, byte[]>();

        private Singletones Singletones;

        public Entries()
        {
            Singletones = new Singletones();
        }

        public void Clear()
        {
            ModEntries.Clear();
            CoreEntries.Clear();
            Singletones.Clear();
        }

        public void ClearMods()
        {
            ModEntries.Clear();
            Singletones.ClearMods();
        }

        public void AddCoreEntry(string ModIDEntry, byte[] Entry)
        {
            if(!CoreEntries.ContainsKey(ModIDEntry))
            {
                CoreEntries[ModIDEntry] = Entry;
            }
        }

        public void AddModEntry(string ModIDEntry, byte[] Entry)
        {
            if(CoreEntries.ContainsKey(ModIDEntry))
            {
                CoreEntries[ModIDEntry] = Entry;
            }
            else
            {
                ModEntries[ModIDEntry] = Entry;
            }
        }

        public T GetSingletone<T>(string ModIDEntry)
        {
            if(Singletones.ExistsObject(ModIDEntry))
            {
                return (T)Singletones.GetObject(ModIDEntry);
            }
            else
            {
                byte[] Bytecode = null;
                bool Core;

                if(ModEntries.ContainsKey(ModIDEntry))
                {
                    Bytecode = ModEntries[ModIDEntry];
                    Core = false;
                }
                else if(CoreEntries.ContainsKey(ModIDEntry))
                {
                    Bytecode = CoreEntries[ModIDEntry];
                    Core = true;
                }
                else
                {
                    Core = false;
                }

                T NewInstance;

                if (Bytecode == null)
                {
                    NewInstance = default;
                }
                else
                {
                    NewInstance = (T)MessagePackSerializer.Typeless.Deserialize(Bytecode);
                }

                if(Core)
                {
                    Singletones.AddCoreObject(ModIDEntry, NewInstance);
                }
                else
                {
                    Singletones.AddModObject(ModIDEntry, NewInstance);
                }

                return NewInstance;
            }
        }

        public T GetEntry<T>(string ModIDEntry)
        {
            if(!ModIDUtil.Validate(ModIDEntry))
            {
                return default;
            }

            if(ModEntries.ContainsKey(ModIDEntry))
            {
                return (T)MessagePackSerializer.Typeless.Deserialize(ModEntries[ModIDEntry]);
            }
            else if(CoreEntries.ContainsKey(ModIDEntry))
            {
                return (T)MessagePackSerializer.Typeless.Deserialize(CoreEntries[ModIDEntry]);
            }
            else
            {
                return default;
            }
        }

        public List<string> GetEntries()
        {
            List<string> Result = new List<string>();

            foreach(var Entry in CoreEntries)
            {
                Result.Add(Entry.Key);
            }

            foreach (var Entry in ModEntries)
            {
                Result.Add(Entry.Key);
            }

            return Result;
        }
    }
}
