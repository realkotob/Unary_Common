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

namespace Unary_Common.Shared
{
    class DataSys : Godot.Object, IShared
    {
        private const string FolderPath = "Data/";
        private Dictionary<string, string> DataPaths;

        public void Init()
        {
            DataPaths = new Dictionary<string, string>();

            List<string> DataFolders = FilesystemUtil.SystemDirGetDirs(FolderPath);

            foreach(var Folder in DataFolders)
            {
                string ModID = System.IO.Path.GetFileNameWithoutExtension(Folder);
                DataPaths[ModID] = Folder;
            }
        }

        public void Clear()
        {
            DataPaths.Clear();
        }

        public void ClearMod(string ModID)
        {
            if (DataPaths.ContainsKey(ModID))
            {
                DataPaths.Remove(ModID);
            }
        }

        public void ClearedMods()
        {

        }

        public void InitCore(string ModID, string Path)
        {
            if(!FilesystemUtil.SystemDirExists(FolderPath + ModID))
            {
                FilesystemUtil.SystemDirCreate(FolderPath + ModID);
                DataPaths[ModID] = FolderPath + ModID;
            }
        }

        public void InitMod(string ModID, string Path)
        {
            if (!FilesystemUtil.SystemDirExists(FolderPath + ModID))
            {
                FilesystemUtil.SystemDirCreate(FolderPath + ModID);
                DataPaths[ModID] = FolderPath + ModID;
            }
        }

        public string GetPath(string ModID)
        {
            if(DataPaths.ContainsKey(ModID))
            {
                return DataPaths[ModID];
            }
            else
            {
                return default;
            }
        }
    }
}
