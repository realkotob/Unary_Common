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
using System.Collections.Generic;

namespace Unary.Common.UI
{
    class ProfilerNamespace
    {
        private PackedScene PackedEntry;

        public VBoxContainer Container;
        public Label Name;
        public Dictionary<int, ProfilerEntry> Entries;

        public ProfilerNamespace(string NewName, PackedScene NewPackedEntry, Node NewRoot)
        {
            PackedEntry = NewPackedEntry;
            Name = (Label)PackedEntry.Instance();
            Name.Text = NewName + ": ";
            Container = new VBoxContainer();
            Container.AddChild(Name);
            Entries = new Dictionary<int, ProfilerEntry>();
            NewRoot.AddChild(Container);
        }

        ~ProfilerNamespace()
        {
            Entries.Clear();

            Name.QueueFree();
            Container.QueueFree();
        }

        public void Add(string Name)
        {
            int NameHash = Name.GetHashCode();

            if(!Entries.ContainsKey(NameHash))
            {
                Entries[NameHash] = new ProfilerEntry(Name, PackedEntry);
                Container.AddChild(Entries[NameHash].Container);
            }
        }

        public void Remove(string Name)
        {
            int NameHash = Name.GetHashCode();

            if (Entries.ContainsKey(NameHash))
            {
                Entries.Remove(NameHash);
            }
        }

        public ProfilerEntry Get(string Name)
        {
            return Entries[Name.GetHashCode()];
        }

    }
}
