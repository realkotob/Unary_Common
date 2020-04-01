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
using Unary_Common.Utils;
using Unary_Common.Arguments;
using Unary_Common.Shared;
using Unary_Common.Abstract;

using System;
using System.Collections.Generic;
using System.Linq;

using Godot;

using Steamworks;

namespace Unary_Common.Server
{
    public class SteamSys : SysObject
    {
        private EventSys EventSys;
        private Callback<ValidateAuthTicketResponse_t> CallbackValidateAuthTicketResponse;

        private Dictionary<int, ulong> SteamIDs;

        public override void Init()
        {
            EventSys = Sys.Ref.Shared.GetNode<EventSys>();
            CallbackValidateAuthTicketResponse = Callback<ValidateAuthTicketResponse_t>.Create(OnTicketResponse);

            SteamIDs = new Dictionary<int, ulong>();

            EventSys.SubscribeEvent(this, nameof(OnAuth), "Unary_Common.Auth");
            EventSys.SubscribeEvent(this, nameof(OnDisconnected), "Unary_Common.Disconnected");

            SteamIDs[1] = SteamUser.GetSteamID().m_SteamID;
        }

        public override void Clear()
        {
            foreach(var SteamID in SteamIDs.ToList())
            {
                if(SteamID.Key != 1)
                {
                    EndAuthSession(SteamID.Value);
                    SteamIDs.Remove(SteamID.Key);
                }
                else
                {
                    SteamIDs.Remove(1);
                }
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

            foreach(var SteamID in SteamIDs)
            {
                if(SteamID.Value == Arguments.m_SteamID.m_SteamID)
                {
                    NewResponse.ID = SteamID.Key;
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

            EventSys.InvokeEvent("Unary_Common.TicketResponse", NewResponse);
        }
        
        public void OnAuth(SteamPlayer Arguments)
        {
            AuthResponse NewResponse = new AuthResponse();

            bool Exists = false;

            foreach(var Peer in SteamIDs)
            {
                if(Peer.Value == Arguments.SteamID || Peer.Key == Arguments.ID)
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
                SteamIDs[Arguments.ID] = Arguments.SteamID;
            }

            EventSys.InvokeEvent("Unary_Common.AuthResponse", NewResponse);
        }

        public void OnDisconnected(Args Arguments)
        {
            if (SteamIDs.ContainsKey(Arguments.ID))
            {
                if (Arguments.ID != 1)
                {
                    EndAuthSession(SteamIDs[Arguments.ID]);
                    SteamIDs.Remove(Arguments.ID);
                }
                else
                {
                    SteamIDs.Remove(1);
                }
            }
        }

        public string GetNickname(int Peer)
        {
            if(SteamIDs.ContainsKey(Peer))
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
            if(SteamIDs.ContainsKey(Peer))
            {
                return SteamIDs[Peer];
            }
            else
            {
                return 0;
            }
        }
    }
}
