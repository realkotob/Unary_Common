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

using Unary_Common.Interfaces;
using Unary_Common.Utils;

using System;
using System.Collections.Generic;
using System.Reflection;

namespace Unary_Common.Shared
{
    class AssemblySys : Godot.Object, IShared
    {
        private ConsoleSys ConsoleSys;

        Dictionary<string, Assembly> Assemblies;
        Dictionary<string, Type> NamedTypes;
        Dictionary<Type, string> ActualTypes;

        public void Init()
        {
            ConsoleSys = Sys.Ref.GetSharedNode<ConsoleSys>();

            Assemblies = new Dictionary<string, Assembly>();

            NamedTypes = new Dictionary<string, Type>();
            ActualTypes = new Dictionary<Type, string>();

            Assembly CommonAssembly = Assembly.GetCallingAssembly();
            Assembly[] NewAssemblies = AppDomain.CurrentDomain.GetAssemblies();

            Assemblies["Common"] = CommonAssembly;
            AddTypes(Assemblies["Common"]);

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

        public void Clear()
        {
            Assemblies.Clear();
            NamedTypes.Clear();
            ActualTypes.Clear();
        }

        public void ClearMod(string ModID)
        {
            if(Assemblies.ContainsKey(ModID))
            {
                Assemblies.Remove(ModID);
            }

            foreach(var NamedType in NamedTypes)
            {
                if(NamedType.Key.StartsWith(ModID + "."))
                {
                    ActualTypes.Remove(NamedType.Value);
                    NamedTypes.Remove(NamedType.Key);
                }
            }
        }

        public void ClearedMods()
        {

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
                if (Type.Namespace != null && Type.IsClass || (Type.IsValueType && !Type.IsEnum))
                {
                    string Key = ModIDUtil.FromType(Type);

                    if (!ModIDUtil.Validate(Key))
                    {
                        continue;
                    }

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

        public void InitCore(string ModID, string Path)
        {
            if(!FilesystemUtil.SystemDirContainsFiles(Path, ModID + ".dll"))
            {
                ConsoleSys.Panic("Core mod cant exist without assembly");
            }
            else
            {
                if (!Assemblies.ContainsKey(ModID))
                {
                    try
                    {
                        Assemblies[ModID] = Assembly.LoadFrom(Path + '/' + ModID + ".dll");
                    }
                    catch (Exception Exception)
                    {
                        ConsoleSys.Error(Exception.Message);
                    }

                    AddTypes(Assemblies[ModID]);
                }
                else
                {
                    ConsoleSys.Error("Tried to init Core twice in AssemblySys");
                }
            }
        }

        public void InitMod(string ModID, string Path)
        {
            if (FilesystemUtil.SystemDirContainsFiles(Path, ModID + ".dll"))
            {
                try
                {
                    Assemblies[ModID] = Assembly.LoadFrom(Path + '/' + ModID + ".dll");
                }
                catch (Exception Exception)
                {
                    ConsoleSys.Error(Exception.Message);
                }

                AddTypes(Assemblies[ModID]);
            }
        }
    }
}
