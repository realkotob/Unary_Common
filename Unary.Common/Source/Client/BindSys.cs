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

using Unary.Common.Interfaces;
using Unary.Common.Utils;
using Unary.Common.Structs;
using Unary.Common.Abstract;
using Unary.Common.Shared;

using System;
using System.Collections.Generic;
using Godot;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Unary.Common.Client
{
    public class BindSys : SysNode
    {
        private ConsoleSys ConsoleSys;

        private Dictionary<string, Binds> Entries;

        public override void Init()
        {
            ConsoleSys = Shared.Sys.Ref.ConsoleSys;

            Entries = new Dictionary<string, Binds>();

            Load("Unary.Common", ".");
        }

        public override void Clear()
        {
            Entries.Clear();
        }

        public override void ClearMod(Mod Mod)
        {
            if (Entries.ContainsKey(Mod.ModID))
            {
                Entries[Mod.ModID].Save();
                Entries.Remove(Mod.ModID);
            }
        }

        private void Load(string ModID, string Path)
        {
            if(Entries.ContainsKey(ModID))
            {
                ConsoleSys.Error(ModID + " binds are already loaded in the system");
                return; 
            }

            string FullPath = Path + '/' + "Binds.json";

            if(!FilesystemUtil.Sys.FileExists(FullPath))
            {
                ConsoleSys.Error("Tried loading binds at " + Path + " but file is not here");
                return;
            }

            Binds NewBinds = new Binds();
            NewBinds.Init(FullPath);
            Entries[ModID] = NewBinds;
        }

        public void Reload()
        {
            Save();
            Load();
        }

        private void Save()
        {
            foreach(var ModID in Entries)
            {
                ModID.Value.Save();
            }
        }

        private void Load()
        {
            foreach (var ModID in Entries)
            {
                ModID.Value.Load();
            }
        }

        public override void InitCore(Mod Mod)
        {
            Load(Mod.ModID, Mod.Path);
        }

        public override void InitMod(Mod Mod)
        {
            Load(Mod.ModID, Mod.Path);
        }
    }
}