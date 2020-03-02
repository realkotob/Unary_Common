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

using Godot;

using Unary_Common.Shared;

namespace Unary_Common.UI
{
    public class ProfilerManager : WindowDialog
    {
        /*
        private static bool Debug;

        private Timer Timer;

        private VBoxContainer RootContainer;

        private PackedScene ProfilerEntryPacked;

        private bool NoEntriesPresented;
        private Label NoEntriesText;

        private Dictionary<int, ProfilerNamespace> Namespaces;
        private Dictionary<int, Dictionary<int, Profiler>> Subscribers;

        private bool CurrentlyVisible;

        //Thank you, C#
        private static ProfilerManager RealInstance;
        public static ProfilerManager Instance 
        {
            get
            {
                if(!Debug)
                {
                    Console.Instance.Warning("ProfilerManager is presented in a production code!");
                }

                return RealInstance;
            }

            set
            {
                RealInstance = value;
            }
        }

        public override void _Ready()
        {
            Instance = this;
            Debug = ConfigSystem.Instance.GetConfig("Common").GetValue<bool>("Debug");
            ProfilerEntryPacked = GD.Load<PackedScene>("Unary_Common/Nodes/UI/ProfilerEntry.tscn");

            Namespaces = new Dictionary<int, ProfilerNamespace>();
            Subscribers = new Dictionary<int, Dictionary<int, Profiler>>();

            RootContainer = GetNode<VBoxContainer>("Scroll/Namespaces");

            NoEntriesPresented = true;
            NoEntriesText = (Label)ProfilerEntryPacked.Instance();
            NoEntriesText.Text = "There are no entries presented.";
            RootContainer.AddChild(NoEntriesText);

            Timer = new Timer();
            AddChild(Timer);
            ConfigSystem.Instance.GetConfig("Common").Subscribe(Timer, nameof(Timer.WaitTime), "ProfilerTime");
            Timer.Connect("timeout", this, nameof(UpdateAll));
            Timer.Start();

            Hide();
            CurrentlyVisible = false;
            Timer.Paused = true;
        }

        public override void _Input(InputEvent @event)
        {
            if (@event.IsActionReleased("ui_profiler"))
            {
                if (CurrentlyVisible)
                {
                    Hide();
                    CurrentlyVisible = false;
                    Timer.Paused = true;
                }
                else
                {
                    Popup_();
                    CurrentlyVisible = true;
                    Timer.Paused = false;
                }
            }
        }

        public void Subscribe(Profiler Profiler, string Namespace, string Name)
        {
            int NamespaceHash = Namespace.GetHashCode();
            int NameHash = Name.GetHashCode();

            if(!Subscribers.ContainsKey(NamespaceHash))
            {
                Subscribers[NamespaceHash] = new Dictionary<int, Profiler>();
                Namespaces[NamespaceHash] = new ProfilerNamespace(Namespace, ProfilerEntryPacked, RootContainer);

                if (!Subscribers[NamespaceHash].ContainsKey(NameHash))
                {
                    if (NoEntriesPresented)
                    {
                        NoEntriesText.QueueFree();
                        NoEntriesPresented = false;
                    }

                    Namespaces[NamespaceHash].Add(Name);
                    Subscribers[NamespaceHash][NameHash] = Profiler;
                }
            }
        }

        public void Unsubscribe(string Namespace, string Name)
        {
            int NamespaceHash = Namespace.GetHashCode();
            int NameHash = Name.GetHashCode();

            if (Subscribers.ContainsKey(NamespaceHash))
            {
                if (Subscribers[NamespaceHash].ContainsKey(NameHash))
                {
                    Namespaces[NamespaceHash].Remove(Name);
                    Subscribers[NamespaceHash].Remove(NameHash);
                }
            }

            if(Namespaces[NamespaceHash].Entries.Count == 0)
            {
                Namespaces.Remove(NamespaceHash);
            }
        }

        public void UpdateAll()
        {
            foreach(var Namespace in Subscribers)
            {
                foreach(var ProfilerEntry in Namespace.Value)
                {
                    var TargetProfiler = ProfilerEntry.Value;

                    List<Profiler.Entry> NewEntryList = TargetProfiler.Get();

                    int NamespaceHash = TargetProfiler.Namespace.GetHashCode();
                    int NameHash = TargetProfiler.Name.GetHashCode();

                    foreach (var Entry in NewEntryList)
                    {
                        Namespaces[NamespaceHash].Get(TargetProfiler.Name).Set(Entry.Name, Entry.Value);
                    }
                }
            }
        }
        */
    }
}
