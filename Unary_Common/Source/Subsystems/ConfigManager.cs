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

using Unary_Common.Shared;
using Unary_Common.Structs;
using Unary_Common.Utils;

namespace Unary_Common.Subsystems
{
    public class ConfigManager
    {
        private Dictionary<string, Config> Configs;
        private string FileName;

        public ConfigManager(string Path)
        {
            Configs = new Dictionary<string, Config>();
            FileName = Path;
        }

        public void Clear()
        {
            Configs.Clear();
        }

        public void ClearMod(Mod Mod)
        {
            if(Configs.ContainsKey(Mod.ModID))
            {
                Configs[Mod.ModID].Save();
                Configs.Remove(Mod.ModID);
            }
        }

        public void Load(string ModID, string Path)
        {
            string FilePath = Path + '/' + FileName + ".json";

            if (FilesystemUtil.SystemFileExists(FilePath))
            {
                if (!Configs.ContainsKey(ModID))
                {
                    Config NewConfig = new Config();
                    NewConfig.Init(FilePath);
                    Configs[ModID] = NewConfig;
                }
            }
        }

        public void Subscribe(Godot.Object Target, string MemberName, string VariableName, SubscriberType Type)
        {
            if (!ModIDUtil.Validate(VariableName))
            {
                return;
            }

            string ModID = ModIDUtil.ModID(VariableName);

            if(Configs.ContainsKey(ModID))
            {
                Configs[ModID].Subscribe(Target, MemberName, VariableName, Type);
            }
        }

        public T Get<T>(string VariableName)
        {
            if (!ModIDUtil.Validate(VariableName))
            {
                Sys.Ref.ConsoleSys.Error("Failed to validate " + VariableName);
                return default;
            }

            string ModID = ModIDUtil.ModID(VariableName);

            if (Configs.ContainsKey(ModID))
            {
                return (T)Configs[ModID].Get(VariableName);
            }
            else
            {
                Sys.Ref.ConsoleSys.Error("Tried to access non-existing config variable " + VariableName);
                return default;
            }
        }

        public void Set(string VariableName, object Value)
        {
            if (!ModIDUtil.Validate(VariableName))
            {
                Sys.Ref.ConsoleSys.Error("Failed to validate " + VariableName);
                return;
            }

            string ModID = ModIDUtil.ModID(VariableName);

            if (Configs.ContainsKey(ModID))
            {
                Configs[ModID].Set(VariableName, Value);
            }
            else
            {
                Sys.Ref.ConsoleSys.Error("Tried to access non-existing config variable " + VariableName);
                return;
            }
        }

        public void Reload()
        {
            foreach(var Config in Configs)
            {
                Config.Value.Load();
            }
        }
    }
}