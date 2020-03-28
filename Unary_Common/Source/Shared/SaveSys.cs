﻿/*
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

using Unary_Common.Interfaces;
using Unary_Common.Utils;
using Unary_Common.Structs;
using Unary_Common.Abstract;

using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Unary_Common.Shared
{
    public class SaveSys : SysObject
    {
        public Dictionary<string, Save> Saves { get; private set; }

        public Save Selected { get; private set; }

        private EntriesSys EntriesSys;

        public override void Init()
        {
            EntriesSys = Sys.Ref.Shared.GetObject<EntriesSys>();

            if (!FilesystemUtil.SystemDirExists("Saves"))
            {
                FilesystemUtil.SystemDirCreate("Saves");
            }

            Saves = new Dictionary<string, Save>();

            foreach(var Dir in FilesystemUtil.SystemDirGetDirsTop("Saves"))
            {
                string Name = System.IO.Path.GetFileName(Dir);

                if (Saves.ContainsKey(Name))
                {
                    continue;
                }

                string ManifestPath = Dir + '/' + "Manifest.json";

                if(FilesystemUtil.SystemFileExists(ManifestPath))
                {
                    string Manifest = FilesystemUtil.SystemFileRead(ManifestPath);

                    if(Manifest == null)
                    {
                        Sys.Ref.ConsoleSys.Error(ManifestPath + " is an invalid manifest");
                        continue;
                    }

                    try
                    {
                        Save NewSave = JsonConvert.DeserializeObject<Save>(Manifest);
                        NewSave.Path = ManifestPath;

                        Saves[Name] = NewSave;
                    }
                    catch(Exception)
                    {
                        Sys.Ref.ConsoleSys.Error("Failed to parse save manifest " + ManifestPath);
                        continue;
                    }
                }
                else
                {
                    Sys.Ref.ConsoleSys.Warning(Dir + " doest not contain Manifest.json");
                }
            }
        }

        public override void Clear()
        {
            Saves.Clear();
        }

        public bool New(string Name, string Description = default)
        {
            if (FilesystemUtil.SystemDirExists("Saves/" + Name) || Saves.ContainsKey(Name))
            {
                return false;
            }

            Dictionary<string, uint> NewRegistry = new Dictionary<string, uint>();

            uint Counter = 0;

            foreach(var Entry in EntriesSys.Entries.GetEntries())
            {
                NewRegistry[Entry] = Counter;
                Counter++;
            }

            Registry Registry = new Registry();
            Registry.Init();

            Save NewSave = new Save
            {
                Description = Description,
                Core = Sys.Ref.Shared.GetObject<ModSys>().Core.Mod,
                Dependency = Sys.Ref.Shared.GetObject<ModSys>().LoadOrder,
                Time = DateTime.Now.ToString(),
                Path = "Saves/" + Name,
                Registry = Registry
            };

            FilesystemUtil.SystemDirCreate("Saves/" + Name);

            try
            {
                FilesystemUtil.SystemFileWrite("Saves/" + Name + "/Manifest.json", JsonConvert.SerializeObject(NewSave));
            }
            catch (Exception)
            {
                return false;
            }

            Saves[Name] = NewSave;

            return true;
        }

        public bool Load(string Name)
        {
            if (!Saves.ContainsKey(Name))
            {
                return false;
            }
            else
            {
                Selected = Saves[Name];
                return true;
            }
        }
    }
}
