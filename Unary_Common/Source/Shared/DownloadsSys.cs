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
using Unary_Common.Structs;

using System;
using System.Collections.Generic;

namespace Unary_Common.Shared
{
    public class DownloadsSys : Godot.Object, IShared
    {
        private string FolderPath = "Downloads/";
        private string TempPath;

        private List<string> DownloadPaths;

        private EventSys EventSys;

        public void Init()
        {
            TempPath = Sys.Ref.GetShared<TempSys>().Get();

            DownloadPaths = new List<string>();

            EventSys = Sys.Ref.GetSharedNode<EventSys>();

            if (!FilesystemUtil.SystemDirExists("Downloads"))
            {
                FilesystemUtil.SystemDirCreate("Downloads");
            }

            foreach (var Dir in FilesystemUtil.SystemDirGetDirsTop("Downloads"))
            {
                Add(Dir);
            }
        }

        public void Clear()
        {
            DownloadPaths.Clear();
        }

        public void Add(string Path)
        {
            if(!DownloadPaths.Contains(Path))
            {
                DownloadPaths.Add(Path);
            }
        }

        public void ClearMod(string ModID)
        {

        }

        public void ClearedMods()
        {

        }

        public void InitCore(string ModID, string Path)
        {

        }

        public void InitMod(string ModID, string Path)
        {

        }
    }
}
