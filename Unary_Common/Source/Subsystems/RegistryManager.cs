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

using Unary_Common.Structs;
using Unary_Common.Subsystems;

namespace Unary_Common.Subsystems
{
    public class RegistryManager
    {
        public Dictionary<string, Registry> Registry { get; set; }

        public RegistryManager()
        {
            Registry = new Dictionary<string, Registry>();
        }

        public void Clear()
        {
            Registry.Clear();
        }

        public void ClearMod(Mod Mod)
        {
            foreach (var RegistryManager in Registry.ToList())
            {
                if (RegistryManager.Key.StartsWith(Mod.ModID + '.'))
                {
                    Registry.Remove(RegistryManager.Key);
                }
                else
                {
                    RegistryManager.Value.RemoveIfStartsWith(Mod.ModID + '.');
                }
            }
        }

        private Registry GetRegistry(string RegistryName)
        {
            if (!Registry.ContainsKey(RegistryName))
            {
                Registry[RegistryName] = new Registry();
                Registry[RegistryName].Init();
            }

            return Registry[RegistryName];
        }

        public void AddEntry(string RegistryName, string ModIDEntry)
        {
            GetRegistry(RegistryName).Add(ModIDEntry);
        }

        public void RemoveEntry(string RegistryName, string ModIDEntry)
        {
            GetRegistry(RegistryName).Remove(ModIDEntry);
        }

        public uint GetEntry(string RegistryName, string ModIDEntry)
        {
            return GetRegistry(RegistryName).Get(ModIDEntry);
        }

        public string GetEntry(string RegistryName, uint Index)
        {
            return GetRegistry(RegistryName).Get(Index);
        }

        public void SetRegistry(string RegistryName, Registry NewRegistry)
        {
            Registry[RegistryName] = NewRegistry;
        }
    }
}
