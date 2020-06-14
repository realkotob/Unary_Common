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

using Unary_Common.Interfaces;
using Unary_Common.Abstract;
using Unary_Common.Structs;
using Unary_Common.Shared;
using Unary_Common.Utils;

using Newtonsoft.Json;

namespace Unary_Common.Server
{
    public class BanSys : SysObject
    {
        private DataSys DataSys;
        private Dictionary<ulong, Ban> Bans;
        private string BanFilePath;

        public override void Init()
        {
            DataSys = Sys.Ref.Shared.GetObject<DataSys>();
            BanFilePath = DataSys.GetPath("Unary_Common") + "/Bans.json";

            if(FilesystemUtil.Sys.FileExists(BanFilePath))
            {
                Bans = JsonConvert.DeserializeObject<Dictionary<ulong, Ban>>(FilesystemUtil.Sys.FileRead(BanFilePath));
            }
            else
            {
                Bans = new Dictionary<ulong, Ban>();
            }
        }

        public override void Clear()
        {
            FilesystemUtil.Sys.FileWrite(BanFilePath, JsonConvert.SerializeObject(Bans));
        }

        public void Ban(ulong SteamID, string Reason, long Seconds)
        {
            if(Bans.ContainsKey(SteamID))
            {
                Sys.Ref.ConsoleSys.Error("SteamID" + SteamID + " is already banned.");
            }
            else
            {
                Ban NewBan = new Ban()
                {
                    Reason = Reason,
                    Time = DateTimeOffset.UtcNow.ToUnixTimeSeconds() + Seconds
                };

                Bans[SteamID] = NewBan;
            }
        }

        public void Unban(ulong SteamID)
        {
            if (Bans.ContainsKey(SteamID))
            {
                Bans.Remove(SteamID);
            }
            else
            {
                Sys.Ref.ConsoleSys.Error("SteamID" + SteamID + " is not banned.");
            }
        }

        public bool IsBanned(ulong SteamID)
        {
            return Bans.ContainsKey(SteamID);
        }

        public Ban GetBan(ulong SteamID)
        {
            if(Bans.ContainsKey(SteamID))
            {
                return Bans[SteamID];
            }
            else
            {
                return default;
            }
        }
    }
}
