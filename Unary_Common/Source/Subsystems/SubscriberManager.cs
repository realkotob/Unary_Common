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

using Unary_Common.Utils;
using Unary_Common.Structs;
using Unary_Common.Shared;
using Unary_Common.Arguments;

namespace Unary_Common.Subsystems
{
    public class SubscriberManager
    {
        private AssemblySys AssemblySys;
        private RegistrySys RegistrySys;
        private Dictionary<string, List<Subscriber>> Subscribers;

        public SubscriberManager()
        {
            AssemblySys = Sys.Ref.Shared.GetNode<AssemblySys>();
            RegistrySys = Sys.Ref.Shared.GetObject<RegistrySys>();
            Subscribers = new Dictionary<string, List<Subscriber>>();
        }

        public void Clear()
        {
            Subscribers.Clear();
        }

        public void ClearMod(Mod Mod)
        {
            foreach (var Name in Subscribers.ToList())
            {
                if (Name.Key.StartsWith(Mod.ModID + '.'))
                {
                    Subscribers.Remove(Name.Key);
                }
            }
        }

        public void Register(string EventName)
        {
            if(Sys.Ref.AppType.IsServer())
            {
                if (!Subscribers.ContainsKey(EventName))
                {
                    Subscribers[EventName] = new List<Subscriber>();
                    RegistrySys.Server.AddEntry("Unary_Common.Events", EventName);
                }
            }
            else
            {
                Sys.Ref.ConsoleSys.Error("Tried registering " + EventName + " event as a client.");
            }
        }

        public void Subscribe(Godot.Object Target, string MemberName, string EventName)
        {
            if (!Subscribers.ContainsKey(EventName))
            {
                Subscribers[EventName] = new List<Subscriber>();
            }

            Subscriber NewSubscriber = new Subscriber
            {
                Target = Target,
                MemberName = MemberName,
                Type = SubscriberType.Method
            };

            Subscribers[EventName].Add(NewSubscriber);
        }

        public void Invoke(string EventName, Args Arguments)
        {
            if (Subscribers.ContainsKey(EventName))
            {
                for (int i = Subscribers[EventName].Count - 1; i >= 0; --i)
                {
                    Subscriber Subscriber = Subscribers[EventName][i];

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
                        Subscribers[EventName].RemoveAt(i);
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