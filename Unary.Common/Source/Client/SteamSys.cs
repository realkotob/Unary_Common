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
using Unary.Common.Arguments;
using Unary.Common.Arguments.Internal;
using Unary.Common.Shared;
using Unary.Common.Abstract;

using System;

using Steamworks;

namespace Unary.Common.Client
{
    public class SteamSys : SysObject
    {
        private EventSys EventSys;

        public int AuthTicketSize { get; set; } = 1024;
        public HAuthTicket AuthTicket { get; set; }

        private Callback<GameOverlayActivated_t> CallbackGameOverlayActivated;

        public override void Init()
        {
            EventSys = Sys.Ref.Shared.GetNode<EventSys>();
            CallbackGameOverlayActivated = Callback<GameOverlayActivated_t>.Create(OnOverlayActivated);

            EventSys.Internal.Subscribe(this, nameof(OnDisconnected), "Unary.Common.Network.Disconnected");
        }

        private void OnOverlayActivated(GameOverlayActivated_t Callback)
        {
            OverlayActivated NewResponse = new OverlayActivated
            {
                Active = Callback.m_bActive
            };

            EventSys.Internal.Invoke("Unary.Common.Steam.OverlayActivated", NewResponse);
        }

        public string GetNickname()
        {
            return SteamFriends.GetPersonaName();
        }

        public ulong GetSteamID()
        {
            return SteamUser.GetSteamID().m_SteamID;
        }

        public byte[] GetAuthTicket()
        {
            byte[] TempTicket = new byte[AuthTicketSize];

            uint TicketSize;

            AuthTicket = SteamUser.GetAuthSessionTicket(TempTicket, AuthTicketSize, out TicketSize);

            byte[] ResultTicket = new byte[TicketSize];

            Array.Copy(TempTicket, 0, ResultTicket, 0, TicketSize);

            return ResultTicket;
        }

        public void OnDisconnected()
        {
            SteamUser.CancelAuthTicket(AuthTicket);
        }
    }
}
