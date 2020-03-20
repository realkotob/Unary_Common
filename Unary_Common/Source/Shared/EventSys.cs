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
using Unary_Common.Utils;
using Unary_Common.Arguments;
using Unary_Common.Abstract;

using Godot;

using System;
using System.Collections.Generic;
using System.Linq;

namespace Unary_Common.Shared
{
    public class EventSys : SysNode
    {

        private RegistrySys RegistrySys;

        private Dictionary<string, List<Subscriber>> EventSubscribers;
        private Dictionary<string, List<Subscriber>> RPCSubscribers;

        public override void Init()
        {
            RegistrySys = Sys.Ref.Shared.GetObject<RegistrySys>();

            EventSubscribers = new Dictionary<string, List<Subscriber>>();
            RPCSubscribers = new Dictionary<string, List<Subscriber>>();
        }

        public override void Clear()
        {
            EventSubscribers.Clear();
            RPCSubscribers.Clear();
        }

        public override void ClearMod(Mod Mod)
        {
            foreach (var Name in EventSubscribers.ToList())
            {
                if(Name.Key.BeginsWith(Mod.ModID + '.'))
                {
                    EventSubscribers.Remove(Name.Key);
                }
            }

            foreach (var Name in RPCSubscribers.ToList())
            {
                if (Name.Key.BeginsWith(Mod.ModID + '.'))
                {
                    RPCSubscribers.Remove(Name.Key);
                }
            }
        }

        public void SubscribeEvent(Godot.Object Target, string MemberName, string EventName)
        {
            if(!EventSubscribers.ContainsKey(EventName))
            {
                EventSubscribers[EventName] = new List<Subscriber>();
            }

            Subscriber NewSubscriber = new Subscriber
            {
                Target = Target,
                MemberName = MemberName,
                Type = SubscriberType.Method
            };

            EventSubscribers[EventName].Add(NewSubscriber);
        }

        public void SubscribeRPC(Godot.Object Target, string MemberName, string EventName)
        {
            if (!RPCSubscribers.ContainsKey(EventName))
            {
                RPCSubscribers[EventName] = new List<Subscriber>();
                RegistrySys.AddEntry("Unary_Common.Events", EventName);
            }

            Subscriber NewSubscriber = new Subscriber
            {
                Target = Target,
                MemberName = MemberName,
                Type = SubscriberType.Method
            };

            RPCSubscribers[EventName].Add(NewSubscriber);
        }

        private void Invoke(string EventName, Args Arguments, ref Dictionary<string, List<Subscriber>> Subscribers)
        {
            if (Subscribers.ContainsKey(EventName))
            {
                for (int i = Subscribers[EventName].Count - 1; i >= 0; --i)
                {
                    Subscriber Subscriber = Subscribers[EventName][i];

                    if (IsInstanceValid(Subscriber.Target))
                    {
                        object Result;

                        if(Arguments == null)
                        {
                            Result = Subscriber.Target.Call(Subscriber.MemberName);
                        }
                        else
                        {
                            Result = Subscriber.Target.Call(Subscriber.MemberName, Arguments);
                        }

                        if (Result == null)
                        {
                            continue;
                        }
                        else
                        {
                            Arguments = (Args)Result;

                            if (Arguments.Canceled)
                            {
                                break;
                            }
                        }
                    }
                    else
                    {
                        Subscribers[EventName].RemoveAt(i);
                    }
                }
            }
            else
            {
                Sys.Ref.ConsoleSys.Warning("Tried invoking event with no subscribers");
            }
        }

        public void InvokeEvent(string Name, Args Arguments)
        {
            if(EventSubscribers.ContainsKey(Name))
            {
                Invoke(Name, Arguments, ref EventSubscribers);
            }
        }
        
        public void InvokeRPC(string Name, Args Arguments)
        {
            RegistrySys.AddEntry("Unary_Common.Events", Name);
            if (RPCSubscribers.ContainsKey(Name))
            {
                Invoke(Name, Arguments, ref RPCSubscribers);
            }
        }
    }
}