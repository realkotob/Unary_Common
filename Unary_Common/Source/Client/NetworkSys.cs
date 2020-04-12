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
using Unary_Common.Shared;
using Unary_Common.Structs;
using Unary_Common.Abstract;
using Unary_Common.Arguments;

using System;
using System.Collections.Generic;

using Godot;

using Steamworks;
using MessagePack;
using LiteNetLib;

namespace Unary_Common.Client
{
    public class NetworkSys : SysNode
    {
        private EventSys EventSys;
        private SteamSys SteamSys;
        private Shared.RegistrySys RegistrySys;

        public override void Init()
        {
            EventSys = Sys.Ref.Shared.GetNode<EventSys>();
            SteamSys = Sys.Ref.Client.GetObject<SteamSys>();
            RegistrySys = Sys.Ref.Shared.GetObject<RegistrySys>();
        }

        public void Start(string Address = "127.0.0.1", int Port = 0, int MaxPlayers = 0)
        {
            if (Port == 0)
            {
                Port = Sys.Ref.Shared.GetObject<ConfigSys>().Shared.Get<int>("Unary_Common.Network.Port");
            }
            
            NetworkedMultiplayerENet NewPeer = new NetworkedMultiplayerENet();
            NewPeer.CreateClient(Address, Port, MaxPlayers);
            NewPeer.TransferMode = NetworkedMultiplayerPeer.TransferModeEnum.Reliable;
            GetTree().NetworkPeer = NewPeer;
            GetTree().Connect("connected_to_server", this, nameof(OnConnectedToServer));
            GetTree().Connect("connection_failed", this, nameof(OnConnectionFailed));
            GetTree().Connect("server_disconnected", this, nameof(OnServerDisconnected));
        }

        public void Stop()
        {
            GetTree().NetworkPeer = null;
            GetTree().Disconnect("connected_to_server", this, nameof(OnConnectedToServer));
            GetTree().Disconnect("connection_failed", this, nameof(OnConnectionFailed));
            GetTree().Disconnect("server_disconnected", this, nameof(OnServerDisconnected));
        }

        public void OnConnectedToServer()
        {
            EventSys.Internal.Invoke("Unary_Common.Connected", null);
        }

        public void OnConnectionFailed()
        {
            EventSys.Internal.Invoke("Unary_Common.ConnectionFailed", null);
        }

        public void OnServerDisconnected()
        {
            EventSys.Internal.Invoke("Unary_Common.Disconnected", null);
        }

        public void RPC(string EventName, Args Arguments)
        {
            Rpc("S", RegistrySys.Client.GetEntry("Unary_Common.Events", EventName), Arguments);
        }

        public void RPCUnreliable(string EventName, Args Arguments)
        {
            RpcUnreliable("S", RegistrySys.Client.GetEntry("Unary_Common.Events", EventName), Arguments);
        }

        [Remote]
        public void C(uint EventIndex, Args Arguments)
        {
            string EventName = RegistrySys.Client.GetEntry("Unary_Common.Events", EventIndex);
            Arguments.ID = Multiplayer.GetRpcSenderId();
            EventSys.Remote.Invoke(EventName, Arguments);
        }
    }
}
