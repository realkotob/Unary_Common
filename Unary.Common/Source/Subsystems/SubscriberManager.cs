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
using System.Linq;

using Godot;

using Unary.Common.Utils;
using Unary.Common.Structs;
using Unary.Common.Shared;
using Unary.Common.Arguments;

namespace Unary.Common.Subsystems
{
    public class SubscriberManager
    {
        private AssemblySys AssemblySys;
        private RegistrySys RegistrySys;
        private Dictionary<string, Dictionary<IntPtr, Subscriber>> Subscribers;
        private Dictionary<string, List<IntPtr>> SubscribeOrder;

        public SubscriberManager()
        {
            AssemblySys = Sys.Ref.Shared.GetNode<AssemblySys>();
            RegistrySys = Sys.Ref.Shared.GetObject<RegistrySys>();

            Subscribers = new Dictionary<string, Dictionary<IntPtr, Subscriber>>();
            SubscribeOrder = new Dictionary<string, List<IntPtr>>();
        }

        public void Clear()
        {
            Subscribers.Clear();
            SubscribeOrder.Clear();
        }

        public void ClearMod(Mod Mod)
        {
            foreach (var Name in Subscribers.ToList())
            {
                if (Name.Key.StartsWith(Mod.ModID + '.'))
                {
                    Subscribers.Remove(Name.Key);
                    SubscribeOrder.Remove(Name.Key);
                }
            }
        }

        public void Register(string EventName)
        {
            if (!Subscribers.ContainsKey(EventName))
            {
                Subscribers[EventName] = new Dictionary<IntPtr, Subscriber>();
                SubscribeOrder[EventName] = new List<IntPtr>();
                RegistrySys.AddEntry("Unary.Common.Events", EventName);
            }
        }

        public void Subscribe(Godot.Object Target, string MemberName, string EventName)
        {
            if (!Subscribers.ContainsKey(EventName))
            {
                Subscribers[EventName] = new Dictionary<IntPtr, Subscriber>();
                SubscribeOrder[EventName] = new List<IntPtr>();
            }

            if(!Subscribers[EventName].ContainsKey(Target.NativeInstance))
            {
                Subscriber NewSubscriber = new Subscriber
                {
                    Target = Target,
                    MemberName = MemberName,
                    Type = SubscriberType.Method
                };

                Subscribers[EventName][Target.NativeInstance] = NewSubscriber;
                SubscribeOrder[EventName].Add(Target.NativeInstance);
            }
            else
            {
                Sys.Ref.ConsoleSys.Error("You really should not subscribe to the same event " + EventName + " multiple times from the same object.");
            }
        }

        public void Unsubscribe(Godot.Object Target, string EventName)
        {
            if(!Subscribers.ContainsKey(EventName))
            {
                Sys.Ref.ConsoleSys.Error("Tried unsubscribing from event" + EventName + " with no subscribers.");
                return;
            }

            if (Subscribers[EventName].ContainsKey(Target.NativeInstance))
            {
                Subscribers[EventName].Remove(Target.NativeInstance);
                SubscribeOrder[EventName].Remove(Target.NativeInstance);
            }
            else
            {
                Sys.Ref.ConsoleSys.Error("Tried removing non-existing subscriber from event " + EventName);
            }
        }

        public void Invoke(string EventName, Args Arguments)
        {
            if (Subscribers.ContainsKey(EventName))
            {
                for (int i = SubscribeOrder[EventName].Count - 1; i >= 0; --i)
                {
                    Subscriber Subscriber = Subscribers[EventName][SubscribeOrder[EventName][i]];

                    if (AssemblySys.IsInstanceValid(Subscriber.Target))
                    {
                        object Result;

                        if (Arguments == null)
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
                        Subscribers[EventName].Remove(SubscribeOrder[EventName][i]);
                        SubscribeOrder[EventName].RemoveAt(i);
                    }
                }
            }
            else
            {
                Sys.Ref.ConsoleSys.Warning("Tried invoking event with no subscribers");
            }
        }
    }
}