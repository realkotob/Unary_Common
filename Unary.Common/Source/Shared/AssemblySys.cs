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

using Godot;

using Unary.Common.Interfaces;
using Unary.Common.Utils;
using Unary.Common.Structs;
using Unary.Common.Abstract;

using System;
using System.Collections.Generic;
using System.Reflection;

namespace Unary.Common.Shared
{
    class AssemblySys : SysNode
    {
        private ConsoleSys ConsoleSys;

        Dictionary<string, Assembly> Assemblies;
        Dictionary<string, Type> NamedTypes;
        Dictionary<Type, string> ActualTypes;

        public override void Init()
        {
            ConsoleSys = Sys.Ref.ConsoleSys;

            Assemblies = new Dictionary<string, Assembly>();

            NamedTypes = new Dictionary<string, Type>();
            ActualTypes = new Dictionary<Type, string>();

            Assembly CommonAssembly = Assembly.GetCallingAssembly();
            Assembly[] NewAssemblies = AppDomain.CurrentDomain.GetAssemblies();

            Assemblies["Unary.Common"] = CommonAssembly;
            AddTypes(Assemblies["Unary.Common"]);

            foreach (var Entry in NewAssemblies)
            {
                string CurrentName = Entry.GetName().Name;

                if (CurrentName.BeginsWith("System") || CurrentName.BeginsWith("Godot") || CurrentName.BeginsWith("mscorlib"))
                {
                    Assemblies[CurrentName] = Entry;
                    AddTypes(Assemblies[CurrentName]);
                }
            }
        }

        public override void Clear()
        {
            Assemblies.Clear();
            NamedTypes.Clear();
            ActualTypes.Clear();
        }

        public override void ClearMod(Mod Mod)
        {
            if(Assemblies.ContainsKey(Mod.ModID))
            {
                Assemblies.Remove(Mod.ModID);
            }

            foreach(var NamedType in NamedTypes)
            {
                if(NamedType.Key.StartsWith(Mod.ModID + "."))
                {
                    ActualTypes.Remove(NamedType.Value);
                    NamedTypes.Remove(NamedType.Key);
                }
            }
        }

        public Assembly GetAssembly(string ModID)
        {
            if(Assemblies.ContainsKey(ModID))
            {
                return Assemblies[ModID];
            }
            else
            {
                return null;
            }
        }

        public string GetType(Type Target)
        {
            if(ActualTypes.ContainsKey(Target))
            {
                return ActualTypes[Target];
            }
            else
            {
                return null;
            }
        }

        public Type GetType(string Target)
        {
            if (NamedTypes.ContainsKey(Target))
            {
                return NamedTypes[Target];
            }
            else
            {
                return null;
            }
        }

        private void AddTypes(Assembly TargetAssembly)
        {
            foreach (var Type in TargetAssembly.GetTypes())
            {
                if (Type.Namespace != null && (Type.IsClass || (Type.IsValueType && !Type.IsEnum)))
                {
                    string Key = ModIDUtil.FromType(Type);

                    if (!NamedTypes.ContainsKey(Key))
                    {
                        NamedTypes[Key] = Type;
                    }

                    if (!ActualTypes.ContainsKey(Type))
                    {
                        ActualTypes[Type] = Key;
                    }
                }
            }
        }

        public new bool IsInstanceValid(Godot.Object Target)
        {
            return IsInstanceValid(Target);
        }

        public override void InitCore(Mod Mod)
        {
            if(!FilesystemUtil.Sys.DirContainsFiles(Mod.Path, Mod.ModID + ".dll"))
            {
                ConsoleSys.Panic("Core mod cant exist without assembly");
            }
            else
            {
                if (!Assemblies.ContainsKey(Mod.ModID))
                {
                    try
                    {
                        Assemblies[Mod.ModID] = Assembly.LoadFrom(Mod.Path + '/' + Mod.ModID + ".dll");
                    }
                    catch (Exception Exception)
                    {
                        ConsoleSys.Error(Exception.Message);
                    }

                    AddTypes(Assemblies[Mod.ModID]);
                }
                else
                {
                    ConsoleSys.Error("Tried to init Core twice in AssemblySys");
                }
            }
        }

        public override void InitMod(Mod Mod)
        {
            if (FilesystemUtil.Sys.DirContainsFiles(Mod.Path, Mod.ModID + ".dll"))
            {
                try
                {
                    Assemblies[Mod.ModID] = Assembly.LoadFrom(Mod.Path + '/' + Mod.ModID + ".dll");
                }
                catch (Exception Exception)
                {
                    ConsoleSys.Error(Exception.Message);
                }

                AddTypes(Assemblies[Mod.ModID]);
            }
        }
    }
}
