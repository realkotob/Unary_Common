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
using Unary_Common.Interfaces;

namespace Unary_Common.Utils
{
    public class TreeStateUtil
    {
        private struct StateEntry
        {
            public string Name;
            public string ParentName;
            public List<string> Children;
            public ITreeStateEntry Target;
        }

        private bool Root = false;

        private string Selected = null;

        private Dictionary<string, StateEntry> Entries;

        public TreeStateUtil()
        {
            Entries = new Dictionary<string, StateEntry>();
        }

        public void AddRoot(string Name, ITreeStateEntry Target)
        {
            if(!Root)
            {
                if (!Entries.ContainsKey(Name))
                {
                    StateEntry NewEntry = new StateEntry
                    {
                        Name = Name,
                        ParentName = null,
                        Children = new List<string>(),
                        Target = Target
                    };

                    Entries[Name] = NewEntry;
                    Root = true;
                }
            }
        }

        public void AddEntry(string Name, string Parent, ITreeStateEntry Target)
        {
            if (Root)
            {
                if (!Entries.ContainsKey(Name) && Entries.ContainsKey(Parent) && !Entries[Parent].Children.Contains(Name))
                {
                    StateEntry NewEntry = new StateEntry
                    {
                        Name = Name,
                        ParentName = Parent,
                        Children = new List<string>(),
                        Target = Target
                    };

                    Entries[Name] = NewEntry;

                    Entries[Parent].Children.Add(Name);
                }
            }
        }

        public void Move(string Name)
        {
            if(Entries.ContainsKey(Name))
            {
                if (Selected == null)
                {
                    Selected = Name;
                    Entries[Name].Target.Enter();
                }
                else
                {
                    if(Entries[Selected].ParentName == Name || Entries[Selected].Children.Contains(Name))
                    {
                        Entries[Selected].Target.Leave();
                        Selected = Name;
                        Entries[Selected].Target.Enter();
                    }
                }
            }
        }
    }
}
