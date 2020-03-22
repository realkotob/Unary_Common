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

using Unary_Common.Abstract;
using Unary_Common.Arguments;
using Unary_Common.Structs;

using System;
using System.Collections.Generic;
using System.Linq;

namespace Unary_Common.Shared
{
    public class RegistrySys : SysObject
    {
        public bool Synced { get; set; } = false;

        public Dictionary<string, Registry> Registry { get; set; }

        public override void Init()
        {
            Registry = new Dictionary<string, Registry>();
        }

        public override void Clear()
        {
            Registry.Clear();
        }

        public override void ClearMod(Mod Mod)
        {
            foreach(var RegistryManager in Registry.ToList())
            {
                if(RegistryManager.Key.StartsWith(Mod.ModID + '.'))
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
            if(!Registry.ContainsKey(RegistryName))
            {
                Registry[RegistryName] = new Registry();
                Registry[RegistryName].Init();
            }

            return Registry[RegistryName];
        }

        public void AddEntry(string RegistryName, string ModIDEntry)
        {
            if(GetRegistry(RegistryName).Add(ModIDEntry))
            {
                if (Sys.Ref.MultiplayerType == Enums.SysMultiplayer.Server && Synced)
                {
                    Sys.Ref.Server.GetObject<Server.RegistrySys>().AddEntry(RegistryName, ModIDEntry);
                }
            }
        }

        public void RemoveEntry(string RegistryName, string ModIDEntry)
        {
            if (GetRegistry(RegistryName).Remove(ModIDEntry))
            {
                if (Sys.Ref.MultiplayerType == Enums.SysMultiplayer.Server && Synced)
                {
                    Sys.Ref.Server.GetObject<Server.RegistrySys>().RemoveEntry(RegistryName, ModIDEntry);
                }
            }
        }

        public uint GetEntry(string RegistryName, string ModIDEntry)
        {
            return GetRegistry(RegistryName).Get(ModIDEntry);
        }

        public string GetEntry(string RegistryName, uint Index)
        {
            return GetRegistry(RegistryName).Get(Index);
        }
    }
}