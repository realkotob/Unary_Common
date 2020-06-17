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

using Unary.Common.Utils;
using Unary.Common.Shared;

using Godot;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Newtonsoft.Json;

namespace Unary.Common.Structs
{
    public struct Binds
    {
        public string Path { get; private set; }

        public Dictionary<string, List<string>> Actions;

        private bool Valid;

        public void Init(string FilePath)
        {
            Valid = true;
            Path = FilePath;

            Actions = new Dictionary<string, List<string>>();

            Load();
        }

        public void Load()
        {
            if(!FilesystemUtil.SystemFileExists(Path))
            {
                Valid = false;
                Sys.Ref.ConsoleSys.Error("Tried to load binds at " + Path + " but file is not presented");
                return;
            }

            string BindList = FilesystemUtil.SystemFileRead(Path);

            if (BindList == null)
            {
                Valid = false;
                Sys.Ref.ConsoleSys.Error("Tried to init binds at " + Path + " but file empty");
                return;
            }

            try
            {
                Actions = JsonConvert.DeserializeObject<Dictionary<string, List<string>>>(BindList);
            }
            catch(Exception)
            {
                Valid = false;
                Sys.Ref.ConsoleSys.Error("Tried to load binds at " + Path + " but failed at parsing");
                return;
            }

            foreach (var Entry in Actions)
            {
                AddAction(Entry.Key);

                foreach (var Key in Entry.Value)
                {
                    AddKey(Key, Entry.Key);
                }
            }
        }

        public void Save()
        {
            if(Valid)
            {
                try
                {
                    FilesystemUtil.SystemFileWrite(Path, JsonConvert.SerializeObject(Actions, Formatting.Indented));
                }
                catch (Exception)
                {
                    Sys.Ref.ConsoleSys.Error("Tried saving binds at " + Path + " but failed");
                }

                foreach (var Entry in Actions)
                {
                    foreach (var Key in Entry.Value)
                    {
                        ClearKey(Key, Entry.Key);
                    }

                    ClearAction(Entry.Key);
                }
            }
        }

        private void AddAction(string Action)
        {
            if (!InputMap.HasAction(Action))
            {
                InputMap.AddAction(Action);
            }
        }

        private void ClearAction(string Action)
        {
            if (!InputMap.HasAction(Action))
            {
                InputMap.EraseAction(Action);
            }
        }

        private void AddKey(string Key, string Action)
        {
            InputEventKey EventKey = new InputEventKey();
            EventKey.Scancode = (uint)EnumUtil.GetKeyFromString<int, KeyList>(Key);

            if (InputMap.HasAction(Action))
            {
                if (!InputMap.ActionHasEvent(Action, EventKey))
                {
                    InputMap.ActionAddEvent(Action, EventKey);
                }
            }
        }

        private void ClearKey(string Key, string Action)
        {
            InputEventKey EventKey = new InputEventKey();
            EventKey.Scancode = (uint)EnumUtil.GetKeyFromString<int, KeyList>(Key);

            if (InputMap.HasAction(Action))
            {
                if (InputMap.ActionHasEvent(Action, EventKey))
                {
                    InputMap.ActionEraseEvent(Action, EventKey);
                }
            }
        }

    }
}
