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
using Unary_Common.Shared;
using Unary_Common.Utils;

using System.Collections.Generic;
using System.IO;
using System;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Unary_Common.Structs
{
    public struct Config
    {
        public string Path { get; private set; }

        private Dictionary<string, Variable> Properties;
        private Dictionary<string, List<Subscriber>> Subscribers;

        private bool Valid;

        public void Init(string FilePath)
        {
            Valid = true;
            Path = FilePath;

            Properties = new Dictionary<string, Variable>();
            Subscribers = new Dictionary<string, List<Subscriber>>();

            Load();
        }

        public void Load()
        {
            if(!FilesystemUtil.SystemFileExists(Path))
            {
                Valid = false;
                Sys.Ref.GetSharedNode<ConsoleSys>().Error("Tried to init config at " + Path + " but it is not presented");
                return;
            }
            
            string Config = FilesystemUtil.SystemFileRead(Path);

            if (Config == null)
            {
                Valid = false;
                Sys.Ref.GetSharedNode<ConsoleSys>().Error("Tried to init config at " + Path + " but it was empty");
                return;
            }

            Dictionary<string, JObject> Entries;

            try
            {
                Entries = JsonConvert.DeserializeObject<Dictionary<string, JObject>>(Config);
            }
            catch(Exception Exception)
            {
                Valid = false;
                Sys.Ref.GetSharedNode<ConsoleSys>().Error("Failed to load config at " + Path);
                Sys.Ref.GetSharedNode<ConsoleSys>().Error(Exception.Message);
                return;
            }

            AssemblySys AssemblySys = Sys.Ref.GetShared<AssemblySys>();

            foreach (var Entry in Entries)
            {
                string TypeName = Entry.Value["Type"].ToString();

                Type Type = AssemblySys.GetType(TypeName);

                Type Another = AssemblySys.GetType("System.Boolean");

                Type TestType = typeof(bool);

                if (Type == null)
                {
                    Sys.Ref.GetSharedNode<ConsoleSys>().Error(TypeName + " is not a valid type");
                    continue;
                }

                try
                {
                    object Test = JsonConvert.DeserializeObject(Entry.Value["Value"].ToString(Formatting.None), Type);
                    Set(Entry.Key, Test);
                }
                catch (Exception Exception)
                {
                    Sys.Ref.GetSharedNode<ConsoleSys>().Error("Failed at parse of variable " + Entry.Key + " at " + Path);
                    Sys.Ref.GetSharedNode<ConsoleSys>().Error(Exception.Message);
                    continue;
                }
            }
        }

        public void Save()
        {
            if(Valid)
            {
                try
                {
                    FilesystemUtil.SystemFileWrite(Path, JsonConvert.SerializeObject(Properties, Formatting.Indented));
                }
                catch (Exception)
                {
                    Sys.Ref.GetSharedNode<ConsoleSys>().Error("Tried saving config at " + Path + " but failed");
                }
            }
        }

        public void Set(string Property, object Value)
        {
            if(!ModIDUtil.Validate(Property))
            {
                return;
            }

            string TypeName = ModIDUtil.FromType(Value.GetType());

            if (!ModIDUtil.Validate(TypeName))
            {
                return;
            }

            Properties[Property] = new Variable
            {
                Type = TypeName,
                Value = Value
            };

            if(Subscribers.ContainsKey(Property))
            {
                for(int i = Subscribers[Property].Count - 1; i >= 0; --i)
                {
                    var Target = Subscribers[Property][i];

                    if (Godot.Object.IsInstanceValid(Target.Target))
                    {
                        if(Target.Type == SubscriberType.Member)
                        {
                            Target.Target.Set(Target.MemberName, Value);
                        }
                        else
                        {
                            Target.Target.Call(Target.MemberName, Value);
                        }
                    }
                    else
                    {
                        Subscribers[Property].RemoveAt(i);
                    }
                }
            }
        }

        public object Get(string Property)
        {
            if (!ModIDUtil.Validate(Property))
            {
                return null;
            }

            if (Properties.ContainsKey(Property))
            {
                return Properties[Property].Value;
            }
            else
            {
                return null;
            }
        }

        public void Subscribe(Godot.Object Target, string MemberName, string Property, SubscriberType Type)
        {
            if (!ModIDUtil.Validate(Property))
            {
                return;
            }

            if(!Properties.ContainsKey(Property))
            {
                return;
            }

            if (!Subscribers.ContainsKey(Property))
            {
                Subscribers[Property] = new List<Subscriber>();
            }

            Subscriber NewSubscriber = new Subscriber
            {
                Target = Target,
                MemberName = MemberName,
                Type = Type
            };

            Subscribers[Property].Add(NewSubscriber);

            if (Type == SubscriberType.Member)
            {
                Target.Set(MemberName, Get(Property));
            }
            else
            {
                Target.Call(MemberName, Get(Property));
            }
        }
    }
}
