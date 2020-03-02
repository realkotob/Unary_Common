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
using Unary_Common.Structs;

using Godot;

using System;
using System.Collections.Generic;

namespace Unary_Common.Shared
{
    public class BootSys : Godot.Object, IShared
    {
        private Dictionary<string, IBoot> Bootables;

        public void Init()
        {
            Bootables = new Dictionary<string, IBoot>();
        }

        public void Clear()
        {
            Bootables.Clear();
        }

        public void ClearMod(Mod Mod)
        {
            if (Bootables.ContainsKey(Mod.ModID))
            {
                Bootables.Remove(Mod.ModID);
            }
        }

        public void ClearedMods()
        {

        }

        public void InitCore(Mod Mod)
        {
            if(Bootables.ContainsKey(Mod.ModID))
            {
                Sys.Ref.GetSharedNode<ConsoleSys>().Panic("Tried to init Core twice");
                return;
            }

            ModSys ModSys = Sys.Ref.GetShared<ModSys>();
            AssemblySys AssemblySys = Sys.Ref.GetShared<AssemblySys>();

            string BootTarget = ModSys.Core.Boot;

            if(BootTarget == null)
            {
                Sys.Ref.GetSharedNode<ConsoleSys>().Panic("Tried to init Core without Boot target");
                return;
            }

            Type BootType = AssemblySys.GetType(BootTarget);

            if (BootType == null)
            {
                Sys.Ref.GetSharedNode<ConsoleSys>().Panic("Tried to init Core with invalid Boot target");
                return;
            }

            IBoot NewBoot = (IBoot)Activator.CreateInstance(BootType);
            Bootables[Mod.ModID] = NewBoot;
            Bootables[Mod.ModID].AddShared();
        }

        public void InitMod(Mod Mod)
        {
            if (Bootables.ContainsKey(Mod.ModID))
            {
                Sys.Ref.GetSharedNode<ConsoleSys>().Error("Tried to init twice " + Mod.ModID);
                return;
            }

            ModSys ModSys = Sys.Ref.GetShared<ModSys>();
            AssemblySys AssemblySys = Sys.Ref.GetShared<AssemblySys>();

            string BootTarget = ModSys.GetManifest(Mod).Boot;

            if (BootTarget == null)
            {
                Sys.Ref.GetSharedNode<ConsoleSys>().Error("Tried to init " + Mod.ModID + " without Boot target");
                return;
            }

            Type BootType = AssemblySys.GetType(BootTarget);

            if (BootType == null)
            {
                Sys.Ref.GetSharedNode<ConsoleSys>().Error("Tried to init " + Mod.ModID + " with invalid Boot target");
                return;
            }

            IBoot NewBoot = (IBoot)Activator.CreateInstance(BootType);
            Bootables[Mod.ModID] = NewBoot;
            Bootables[Mod.ModID].AddShared();
        }

        public void AddShared(string ModID)
        {
            if (Bootables.ContainsKey(ModID))
            {
                Bootables[ModID].AddShared();
            }
        }

        public void AddClient(string ModID)
        {
            if (Bootables.ContainsKey(ModID))
            {
                Bootables[ModID].AddClient();
            }
        }

        public void AddServer(string ModID)
        {
            if (Bootables.ContainsKey(ModID))
            {
                Bootables[ModID].AddServer();
            }
        }

        public void Add(string ModID, IBoot Bootable)
        {
            if(!Bootables.ContainsKey(ModID))
            {
                Bootables[ModID] = Bootable;
            }
            else
            {
                Sys.Ref.GetShared<ConsoleSys>().Error("Tried to add already registered bootable " + ModID);
            }
        }

        public new IBoot Get(string ModID)
        {
            if(Bootables.ContainsKey(ModID))
            {
                return Bootables[ModID];
            }
            else
            {
                Sys.Ref.GetShared<ConsoleSys>().Error("Tried to get boot of " + ModID + " but it is not presented");
                return default;
            }
        }
    }
}
