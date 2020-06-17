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

using Unary.Common.Interfaces;
using Unary.Common.Utils;
using Unary.Common.Arguments;
using Unary.Common.Arguments.Internal;
using Unary.Common.Arguments.Remote;
using Unary.Common.Shared;
using Unary.Common.Abstract;
using Unary.Common.Collections;

using System;
using System.Collections.Generic;
using System.Linq;

using Godot;

using Steamworks;

namespace Unary.Common.Server
{
    public class SteamSys : SysObject
    {
        [Flags]
        public enum ServerFlags : uint
        {
            None = 0x00,
            Active = 0x01,
            Secure = 0x02,
            Dedicated = 0x04,
            Linux = 0x08,
            Passworded = 0x10,
            Private = 0x20
        }

        private EventSys EventSys;
        private ConfigSys ConfigSys;
        private Shared.SteamSys SharedSteamSys;
        private Callback<ValidateAuthTicketResponse_t> CallbackValidateAuthTicketResponse;

        private BiDictionary<int, ulong> SteamIDs;

        public override void Init()
        {
            EventSys = Sys.Ref.Shared.GetNode<EventSys>();
            ConfigSys = Sys.Ref.Shared.GetObject<ConfigSys>();
            SharedSteamSys = Sys.Ref.Shared.GetNode<Shared.SteamSys>();
            CallbackValidateAuthTicketResponse = Callback<ValidateAuthTicketResponse_t>.Create(OnTicketResponse);

            SteamIDs = new BiDictionary<int, ulong>();

            EventSys.Internal.Subscribe(this, nameof(OnAuth), "Unary.Common.Network.Auth");
            EventSys.Internal.Subscribe(this, nameof(OnDisconnected), "Unary.Common.Network.Disconnected");
            EventSys.Internal.Subscribe(this, nameof(OnStart), "Unary.Common.Network.Start");
        }

        public override void Clear()
        {
            foreach(var SteamID in SteamIDs.GetAll())
            {
                EndAuthSession(SteamID.Item2);
                SteamIDs.Remove(SteamID.Item1);
            }

            if(SteamGameServer.BLoggedOn())
            {
                SteamGameServer.LogOff();
            }
        }

        public string BeginAuthSession(SteamPlayer Arguments)
        {
            return EnumUtil.GetStringFromKey(
            SteamUser.BeginAuthSession(Arguments.Ticket, 
            Arguments.Ticket.Length, new CSteamID(Arguments.SteamID))).Replace("k_EBeginAuthSessionResult", "");
        }

        public void EndAuthSession(ulong SteamID)
        {
            SteamUser.EndAuthSession(new CSteamID(SteamID));
        }

        private void OnTicketResponse(ValidateAuthTicketResponse_t Arguments)
        {
            TicketResponse NewResponse = new TicketResponse();

            foreach(var SteamID in SteamIDs.GetAll())
            {
                if(SteamID.Item2 == Arguments.m_SteamID.m_SteamID)
                {
                    NewResponse.ID = SteamID.Item1;
                }
            }

            if (Arguments.m_SteamID != Arguments.m_OwnerSteamID)
            {
                NewResponse.Response = "NoLicenseOrExpired";
            }
            else
            {
                NewResponse.Response = EnumUtil.GetStringFromKey(Arguments.m_eAuthSessionResponse).Replace("k_EAuthSessionResponse", "");
            }

            if(NewResponse.Response != "OK")
            {
                EndAuthSession(SteamIDs[NewResponse.ID]);
                SteamIDs.Remove(NewResponse.ID);
            }

            EventSys.Internal.Invoke("Unary.Common.Steam.TicketResponse", NewResponse);
        }
        
        public void OnAuth(SteamPlayer Arguments)
        {
            AuthResponse NewResponse = new AuthResponse();

            bool Exists = false;

            foreach(var Peer in SteamIDs.GetAll())
            {
                if(Peer.Item2 == Arguments.SteamID)
                {
                    Exists = true;
                }
            }

            if(Exists)
            {
                NewResponse.Response = EnumUtil.GetStringFromKey(EBeginAuthSessionResult.k_EBeginAuthSessionResultDuplicateRequest);
            }
            else
            {
                NewResponse.Response = BeginAuthSession(Arguments);
            }

            if(NewResponse.Response == "OK")
            {
                SteamIDs.Set(Arguments.ID, Arguments.SteamID);
            }

            EventSys.Internal.Invoke("Unary.Common.Steam.AuthResponse", NewResponse);
        }

        public void OnDisconnected(Args Arguments)
        {
            if (SteamIDs.KeyExists(Arguments.ID))
            {
                EndAuthSession(SteamIDs[Arguments.ID]);
                SteamIDs.Remove(Arguments.ID);
            }
        }

        public string GetNickname(int Peer)
        {
            if(SteamIDs.KeyExists(Peer))
            {
                return SteamFriends.GetPlayerNickname(new CSteamID(SteamIDs[Peer]));
            }
            else
            {
                return "";
            }
        }

        public ulong GetSteamID(int Peer)
        {
            if(SteamIDs.KeyExists(Peer))
            {
                return SteamIDs[Peer];
            }
            else
            {
                return 0;
            }
        }

        private bool OnStart()
        {
            SteamGameServer.SetModDir("Unary.Common");
            SteamGameServer.SetProduct(SharedSteamSys.AppID.ToString());
            SteamGameServer.SetGameDescription("Unary.Common");
            
            if (Sys.Ref.AppType.IsHost())
            {
                SteamGameServer.SetServerName(Sys.Ref.Client.GetObject<Client.SteamSys>().GetNickname() + "'s Server");
                SteamGameServer.SetDedicatedServer(false);
            }
            else if(Sys.Ref.AppType.IsServer())
            {
                SteamGameServer.SetServerName(ConfigSys.Shared.Get<string>("Unary.Common.Network.Name"));
                SteamGameServer.SetDedicatedServer(true);
            }

            SteamGameServer.LogOnAnonymous();

            return SteamGameServer.InitGameServer(0x7f000001,
            (ushort)ConfigSys.Shared.Get<int>("Unary.Common.Network.GamePort"),
            (ushort)ConfigSys.Shared.Get<int>("Unary.Common.Network.QueryPort"),
            0, new AppId_t(SharedSteamSys.AppID), Sys.Ref.Shared.GetObject<ModSys>().Core.Mod.Version.ToString());
        }
    }
}