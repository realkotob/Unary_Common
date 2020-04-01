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

using System;
using System.Collections.Generic;
using System.Linq;

namespace Unary_Common.Structs
{
    public class Categories
    {
        private Dictionary<string, List<string>> CoreCategories = new Dictionary<string, List<string>>();
        private Dictionary<string, List<string>> ModCategories = new Dictionary<string, List<string>>();

        public void Clear()
        {
            ModCategories.Clear();
            CoreCategories.Clear();
        }

        public void ClearMods()
        {
            ModCategories.Clear();
        }

        public void AddCoreEntry(string Category, string ModIDEntry)
        {
            if(!CoreCategories.ContainsKey(Category))
            {
                CoreCategories[Category] = new List<string>();
            }

            if (!CoreCategories[Category].Contains(ModIDEntry))
            {
                CoreCategories[Category].Add(ModIDEntry);
            }
        }

        public void AddModEntry(string Category, string ModIDEntry)
        {
            if (CoreCategories.ContainsKey(Category))
            {
                CoreCategories[Category].Add(ModIDEntry);
            }
            else
            {
                if (!ModCategories.ContainsKey(Category))
                {
                    ModCategories[Category] = new List<string>();
                }

                if (!ModCategories[Category].Contains(ModIDEntry))
                {
                    ModCategories[Category].Add(ModIDEntry);
                }
            }
        }

        public List<string> GetEntries(string Category)
        {
            List<string> Result = new List<string>();

            if(CoreCategories.ContainsKey(Category))
            {
                Result.AddRange(CoreCategories[Category]);
            }
            
            if(ModCategories.ContainsKey(Category))
            {
                Result.AddRange(ModCategories[Category]);
            }

            return Result;
        }

        public List<string> GetNames()
        {
            List<string> Result = new List<string>();

            foreach(var Category in CoreCategories)
            {
                Result.Add(Category.Key);
            }

            foreach (var Category in ModCategories)
            {
                Result.Add(Category.Key);
            }

            return Result;
        }
    }
}
