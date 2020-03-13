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

using Unary_Common.Shared;
using Unary_Common.Enums;
using Unary_Common.Interfaces;
using Unary_Common.Abstract;
using Unary_Common.Utils;

using System.Collections.Generic;
using System.Linq;
using System;

using Godot;

namespace Unary_Common.Structs
{
    public struct SysManager
    {
        private Dictionary<string, SysObject> Objects;
        private Dictionary<string, NodeID> Nodes;
        private Dictionary<string, SysType> Types;

        public List<string> Order { get; private set; }

        public void Init()
        {
            Objects = new Dictionary<string, SysObject>();
            Nodes = new Dictionary<string, NodeID>();
            Types = new Dictionary<string, SysType>();
            Order = new List<string>();
        }

        public void Clear()
        {
            for (int i = Order.Count - 1; i >= 0; --i)
            {
                Clear(Order[i]);
                Order.RemoveAt(i);
            }
        }

        public List<ISysEntry> GetAll()
        {
            List<ISysEntry> Result = new List<ISysEntry>();

            foreach (var OrderEntry in Order)
            {
                if(Types[OrderEntry] == SysType.Object)
                {
                    Result.Add(Objects[OrderEntry]);
                }
                else
                {
                    Result.Add(Sys.Ref.GetChild<ISysEntry>(Nodes[OrderEntry].ID));
                }
            }

            return Result;
        }

        public SysType GetType<T>() where T : ISysEntry
        {
            return GetType(ModIDUtil.FromType(typeof(T)));
        }

        public SysType GetType(string ModIDEntry)
        {
            if(Types.ContainsKey(ModIDEntry))
            {
                return Types[ModIDEntry];
            }
            else
            {
                Sys.Ref.ConsoleSys.Error("Failed to get type of an unregistered system " + ModIDEntry);
                return default;
            }
        }

        public void Add(ISysEntry NewSystem, string LoadPath = null)
        {
            string ModIDEntry = ModIDUtil.FromType(NewSystem.GetType());

            if (!ModIDUtil.Validate(ModIDEntry))
            {
                Sys.Ref.ConsoleSys.Error("Failed to validate " + ModIDEntry);
                return;
            }

            if (Types.ContainsKey(ModIDEntry))
            {
                Sys.Ref.ConsoleSys.Error(ModIDEntry + " is already registered.");
                return;
            }

            if (NewSystem is SysObject Object)
            {
                Object.Init();
                Objects[ModIDEntry] = Object;
                Types[ModIDEntry] = SysType.Object;
                Order.Add(ModIDEntry);
            }
            else if (NewSystem is SysNode Node)
            {
                if (LoadPath != null)
                {
                    Node = NodeUtil.NewNode<SysNode>(ModIDUtil.ModID(ModIDEntry), LoadPath);
                }

                Node.Init();
                Node.Name = ModIDEntry;
                Sys.Ref.AddChild(Node, true);

                Nodes[ModIDEntry] = new NodeID()
                {
                    ID = Sys.Ref.GetChildCount() - 1
                };

                Types[ModIDEntry] = SysType.Node;
                Order.Add(ModIDEntry);
            }
            else if(NewSystem is SysUI UI)
            {
                if (LoadPath != null)
                {
                    UI = NodeUtil.NewNode<SysUI>(ModIDUtil.ModID(ModIDEntry), LoadPath);
                }

                UI.Init();
                UI.Name = ModIDEntry;
                Sys.Ref.AddChild(UI, true);

                Nodes[ModIDEntry] = new NodeID()
                {
                    ID = Sys.Ref.GetChildCount() - 1
                };

                Types[ModIDEntry] = SysType.UI;
                Order.Add(ModIDEntry);
            }
            else
            {
                Sys.Ref.ConsoleSys.Error("Failed to register " + ModIDEntry);
            }

        }

        public ISysEntry Get<T>() where T : ISysEntry
        {
            return Get(ModIDUtil.FromType(typeof(T)));
        }

        public ISysEntry Get(string ModIDEntry)
        {
            if (!ModIDUtil.Validate(ModIDEntry))
            {
                Sys.Ref.ConsoleSys.Error("Failed to verify " + ModIDEntry);
                return default;
            }

            if(Types.ContainsKey(ModIDEntry))
            {
                if(Types[ModIDEntry] == SysType.Object)
                {
                    return Objects[ModIDEntry];
                }
                else
                {
                    return Sys.Ref.GetChild<ISysEntry>(Nodes[ModIDEntry].ID);
                }
            }
            else
            {
                Sys.Ref.ConsoleSys.Error("Failed to get type of " + ModIDEntry);
                return default;
            }
        }

        public T GetObject<T>() where T : SysObject
        {
            return (T)GetObject(ModIDUtil.FromType(typeof(T)));
        }

        public SysObject GetObject(string ModIDEntry)
        {
            if (!ModIDUtil.Validate(ModIDEntry))
            {
                Sys.Ref.ConsoleSys.Error("Failed to verify " + ModIDEntry);
                return default;
            }

            if (Objects.ContainsKey(ModIDEntry))
            {
                return Objects[ModIDEntry];
            }
            else
            {
                Sys.Ref.ConsoleSys.Error("Failed to find " + ModIDEntry + " as an object");
                return default;
            }
        }

        public T GetNode<T>() where T : SysNode
        {
            return (T)GetNode(ModIDUtil.FromType(typeof(T)));
        }

        public SysNode GetNode(string ModIDEntry)
        {
            if (!ModIDUtil.Validate(ModIDEntry))
            {
                Sys.Ref.ConsoleSys.Error("Failed to verify " + ModIDEntry);
                return default;
            }

            if (Nodes.ContainsKey(ModIDEntry))
            {
                return Sys.Ref.GetChild<SysNode>(Nodes[ModIDEntry].ID);
            }
            else
            {
                Sys.Ref.ConsoleSys.Error("Failed to find " + ModIDEntry + " as a node");
                return default;
            }
        }

        public T GetUI<T>() where T : SysUI
        {
            return (T)GetUI(ModIDUtil.FromType(typeof(T)));
        }

        public SysUI GetUI(string ModIDEntry)
        {
            if (!ModIDUtil.Validate(ModIDEntry))
            {
                Sys.Ref.ConsoleSys.Error("Failed to verify " + ModIDEntry);
                return default;
            }

            if (Nodes.ContainsKey(ModIDEntry))
            {
                return Sys.Ref.GetChild<SysUI>(Nodes[ModIDEntry].ID);
            }
            else
            {
                Sys.Ref.ConsoleSys.Error("Failed to find " + ModIDEntry + " as an UI");
                return default;
            }
        }

        public void InitCore(Mod Mod)
        {
            foreach(var SysEntry in GetAll())
            {
                SysEntry.InitCore(Mod);
            }
        }

        public void InitMod(Mod Mod)
        {
            foreach (var SysEntry in GetAll())
            {
                SysEntry.InitMod(Mod);
            }
        }

        public void InitMods()
        {
            foreach (var Mod in Sys.Ref.Shared.GetObject<ModSys>().LoadOrder)
            {
                InitMod(Mod);
            }
        }

        public void Reload()
        {
            foreach(var OrderEntry in Order)
            {
                SysType Type = Types[OrderEntry];

                if (Type != SysType.Object)
                {
                    Sys.Ref.GetChild(Nodes[OrderEntry].ID).
                    ReplaceBy(NodeUtil.ReloadNode(Nodes[OrderEntry].Path, Sys.Ref.GetChild(Nodes[OrderEntry].ID)), false);

                    Sys.Ref.GetChild(Nodes[OrderEntry].ID)._Ready();
                }
            }
        }

        public void Clear<T>() where T : ISysEntry
        {
            Clear(ModIDUtil.FromType(typeof(T)));
        }

        //Invoked in order to clear a system
        public void Clear(string ModIDEntry)
        {
            if (Objects.ContainsKey(ModIDEntry))
            {
                Objects[ModIDEntry].Clear();
                Objects[ModIDEntry].Free();
                Objects.Remove(ModIDEntry);
                Types.Remove(ModIDEntry);
            }
            else if (Nodes.ContainsKey(ModIDEntry))
            {
                Sys.Ref.GetChild<ISysEntry>(Nodes[ModIDEntry].ID).Clear();
                Sys.Ref.GetChild(Nodes[ModIDEntry].ID).QueueFree();
                Nodes.Remove(ModIDEntry);
                Types.Remove(ModIDEntry);
            }
            else
            {
                Sys.Ref.ConsoleSys.Error("Failed to clear " + ModIDEntry);
            }
        }

        //Invoked in order to actually clear mod
        public void ClearMod(string ModIDEntry)
        {
            for (int i = Order.Count - 1; i >= 0; --i)
            {
                if (Order[i].BeginsWith(ModIDEntry + '.'))
                {
                    Clear(ModIDEntry);
                    Order.RemoveAt(i);
                }
            }
        }

        //Invoked in order to notify that a mod got cleared
        public void ClearMod(Mod Mod)
        {
            foreach (var SysEntry in GetAll())
            {
                SysEntry.ClearMod(Mod);
            }
        }

        //Invoked in order to notify that we finished clearing
        public void ClearedMods()
        {
            foreach (var SysEntry in GetAll())
            {
                SysEntry.ClearedMods();
            }
        }

        public void ClearMods()
        {
            ModSys ModSys = Sys.Ref.Shared.GetObject<ModSys>();

            for (int m = ModSys.LoadOrder.Count - 1; m >= 0; --m)
            {
                Mod TargetMod = ModSys.LoadOrder[m];

                ClearMod(TargetMod.ModID);

                ClearMod(TargetMod);
            }

            ClearedMods();
        }
    }
}
