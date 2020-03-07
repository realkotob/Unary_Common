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

using Unary_Common.Interfaces;
using Unary_Common.Utils;
using Unary_Common.Shared;
using Unary_Common.Structs;

using Godot;

using System;
using System.Collections.Generic;
using System.Linq;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using MessagePack;

namespace Unary_Common.Shared
{
    public class EntriesSys : Godot.Object, IShared
    {
        public Entries Entries { get; private set; }
        public Categories Categories { get; private set; }

        private ConsoleSys ConsoleSys;
        private AssemblySys AssemblySys;

        public void Init()
        {
            Entries = new Entries();
            Entries.Init();
            Categories = new Categories();
            Categories.Init();

            ConsoleSys = Sys.Ref.GetSharedNode<ConsoleSys>();
            AssemblySys = Sys.Ref.GetShared<AssemblySys>();
        }

        public void Clear()
        {
            Entries.Clear();
            Categories.Clear();
        }

        public void ClearMod(Mod Mod)
        {
            
        }

        public void ClearedMods()
        {
            Entries.ClearMods();
            Categories.ClearMods();
        }

        private void AddCoreEntry(string ModIDEntry, Type Type, object Entry)
        {
            //Entries
            Entries.AddCoreEntry(ModIDEntry, MessagePackSerializer.Typeless.Serialize(Entry));

            //Categories
            List<string> NewCategories = ModIDUtil.GetCategories(ModIDEntry);

            foreach(var Category in NewCategories)
            {
                Categories.AddCoreEntry(Category, ModIDEntry);
            }
        }

        private void AddModEntry(string ModIDEntry, Type Type, object Entry)
        {
            //Entries
            Entries.AddModEntry(ModIDEntry, MessagePackSerializer.Typeless.Serialize(Entry));

            //Categories
            List<string> NewCategories = ModIDUtil.GetCategories(ModIDEntry);

            foreach (var Category in NewCategories)
            {
                Categories.AddModEntry(Category, ModIDEntry);
            }
        }

        private void LoadEntries(string ModID, bool Core)
        {
            string EntriesDir = "res://" + ModID + "/Entries";

            if (!FilesystemUtil.GodotDirExists(EntriesDir))
            {
                ConsoleSys.Error("Failed to load entries at " + EntriesDir + " because folder does not exist");
                return;
            }

            List<string> EntriesFiles = FilesystemUtil.GodotDirGetFiles(EntriesDir);

            for (int i = EntriesFiles.Count - 1; i >= 0; --i)
            {
                if (!EntriesFiles[i].EndsWith(".json"))
                {
                    EntriesFiles.RemoveAt(i);
                }
            }

            for (int i = EntriesFiles.Count - 1; i >= 0; --i)
            {
                string EntryManifest = FilesystemUtil.GodotFileRead(EntriesFiles[i]);

                try
                {
                    var Entries = JsonConvert.DeserializeObject<Dictionary<string, Dictionary<string, JObject>>>(EntryManifest);

                    foreach (var TypeName in Entries)
                    {
                        if (!ModIDUtil.Validate(TypeName.Key))
                        {
                            ConsoleSys.Error("Entry " + TypeName.Key + " is not a valid ModIDEntry");
                            continue;
                        }

                        string NewModID = ModIDUtil.ModID(TypeName.Key) + ".Entries." + ModIDUtil.ModIDEntry(TypeName.Key);

                        Type Type = AssemblySys.GetType(NewModID);

                        if (Type == null)
                        {
                            ConsoleSys.Error("Type " + TypeName.Key + " is not a valid type");
                            continue;
                        }

                        foreach(var Entry in TypeName.Value)
                        {
                            if(Core)
                            {
                                AddCoreEntry(NewModID, Type, JsonConvert.DeserializeObject(Entry.Value.ToString(Formatting.None), Type));
                            }
                            else
                            {
                                AddModEntry(NewModID, Type, JsonConvert.DeserializeObject(Entry.Value.ToString(Formatting.None), Type));
                            }
                        }
                    }
                }
                catch (Exception Exception)
                {
                    ConsoleSys.Error("Failed to parse entries manifest at " + EntriesFiles[i]);
                    ConsoleSys.Error(Exception.Message);
                    continue;
                }
            }
        }

        public void InitCore(Mod Mod)
        {
            LoadEntries(Mod.ModID, true);
        }

        public void InitMod(Mod Mod)
        {
            LoadEntries(Mod.ModID, false);
        }
    }
}
