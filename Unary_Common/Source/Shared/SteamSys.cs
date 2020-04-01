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
using Unary_Common.Arguments;
using Unary_Common.Utils;
using Unary_Common.Abstract;

using Godot;

using System;
using System.Collections.Generic;

using Steamworks;

namespace Unary_Common.Shared
{
    public class SteamSys : SysNode
    {
        private static ConsoleSys ConsoleSys;

        public List<SteamInstallInfo> WorkshopEntries { get; private set; }

        protected SteamAPIWarningMessageHook_t m_SteamAPIWarningMessageHook;
        protected static void SteamAPIDebugTextHook(int nSeverity, System.Text.StringBuilder pchDebugText)
        {
            ConsoleSys.Warning(pchDebugText.ToString());
        }

        public override void Init()
        {
            ConsoleSys = Sys.Ref.ConsoleSys;

            try
            {
                if (!Packsize.Test())
                {
                    ConsoleSys.Panic("Packsize Test returned false, the wrong version of Steamworks.NET is being run in this platform.");
                }

                if (!DllCheck.Test())
                {
                    ConsoleSys.Panic("DllCheck Test returned false, One or more of the Steamworks binaries seems to be the wrong version.");
                }

                if (SteamAPI.RestartAppIfNecessary(AppId_t.Invalid))
                {
                    ConsoleSys.Panic("Restarting app");
                }
            }
            catch (Exception Exception)
            {
                ConsoleSys.Panic(Exception.Message);
            }

            if (!SteamAPI.Init())
            {
                ConsoleSys.Panic("SteamAPI_Init() failed. Refer to Valve's documentation or the comment above this line for more information.");
            }

            if (m_SteamAPIWarningMessageHook == null)
            {
                m_SteamAPIWarningMessageHook = new SteamAPIWarningMessageHook_t(SteamAPIDebugTextHook);
                SteamClient.SetWarningMessageHook(m_SteamAPIWarningMessageHook);
            }

        }

        public override void Clear()
        {
            SteamAPI.Shutdown();
        }

        public static IEnumerable<PublishedFileId_t> GetAllSubscribed()
        {
            uint SubCount = SteamUGC.GetNumSubscribedItems();

            PublishedFileId_t[] Items = new PublishedFileId_t[SubCount];

            uint Count = SteamUGC.GetSubscribedItems(Items, SubCount);

            for(uint i = 0; i < Count; ++i)
            {
                PublishedFileId_t pfid = Items[i];
                yield return pfid;
            }

            yield break;
        }

        public List<string> GetModFolders()
        {
            List<string> Result = new List<string>();

            foreach(var Subbed in GetAllSubscribed())
            {
                SteamInstallInfo InstallInfo = new SteamInstallInfo();

                if(!SteamUGC.GetItemInstallInfo(Subbed, 
                out InstallInfo.punSizeOnDisk,
                out InstallInfo.pchFolder,
                InstallInfo.cchFolderSize,
                out InstallInfo.punTimeStamp))
                {
                    continue;
                }
                else
                {
                    Result.Add(InstallInfo.pchFolder);
                }
            }

            return Result;
        }

        public override void _Process(float delta)
        {
            SteamAPI.RunCallbacks();
        }
    }
}