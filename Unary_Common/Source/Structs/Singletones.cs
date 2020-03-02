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

using Unary_Common.Utils;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Unary_Common.Structs
{
    public struct Singletones
    {
        private Dictionary<string, object> CoreObjects;
        private Dictionary<string, object> ModObjects;

        public void Init()
        {
            CoreObjects = new Dictionary<string, object>();
            ModObjects = new Dictionary<string, object>();
        }

        public void Clear()
        {
            CoreObjects.Clear();
            ModObjects.Clear();
        }

        public void ClearMods()
        {
            ModObjects.Clear();
        }

        public bool ExistsObject(string ModIDEntry)
        {
            if(ModObjects.ContainsKey(ModIDEntry) || CoreObjects.ContainsKey(ModIDEntry))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public void AddCoreObject(string ModIDEntry, object Object)
        {
            if (!CoreObjects.ContainsKey(ModIDEntry))
            {
                CoreObjects[ModIDEntry] = Object;
            }
        }

        public void AddModObject(string ModIDEntry, object Object)
        {
            ModObjects[ModIDEntry] = Object;
        }

        public object GetObject(string ModIDEntry)
        {
            if (ModObjects.ContainsKey(ModIDEntry))
            {
                return ModObjects[ModIDEntry];
            }
            else if(CoreObjects.ContainsKey(ModIDEntry))
            {
                return CoreObjects[ModIDEntry];
            }
            else
            {
                return default;
            }
        }
    }
}
