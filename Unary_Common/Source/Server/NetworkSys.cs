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
using Unary_Common.Structs;
using Unary_Common.Arguments;
using Unary_Common.Abstract;
using Unary_Common.Utils;

using System;
using System.Collections.Generic;

using Godot;

using LiteNetLib;

namespace Unary_Common.Server
{
    public class NetworkSys : SysNode
    {
        private EventBasedNetListener Listener;
        private NetManager Server;

        private EventSys EventSys;
        private SteamSys SteamSys;
        private RegistrySys RegistrySys;

        private Dictionary<int, NetPeer> Peers;
        private Dictionary<int, bool> ValidatedPeers;

        public override void Init()
        {
            Listener = new EventBasedNetListener();

            Listener.ConnectionRequestEvent += OnConnectionRequest;
            Listener.NetworkErrorEvent += OnNetworkError;
            Listener.NetworkLatencyUpdateEvent += OnNetworkLatencyUpdate;
            Listener.NetworkReceiveEvent += OnNetworkReceive;
            Listener.PeerConnectedEvent += OnPeerConnected;
            Listener.PeerDisconnectedEvent += OnPeerDisconnected;

            Server = new NetManager(Listener);

            EventSys = Sys.Ref.Shared.GetNode<EventSys>();
            SteamSys = Sys.Ref.Server.GetObject<SteamSys>();
            RegistrySys = Sys.Ref.Shared.GetObject<RegistrySys>();

            Peers = new Dictionary<int, NetPeer>();
            ValidatedPeers = new Dictionary<int, bool>();

            EventSys.Internal.Subscribe(this, nameof(OnAuthResponse), "Unary_Common.AuthResponse");
            EventSys.Internal.Subscribe(this, nameof(OnTicketResponse), "Unary_Common.TicketResponse");
        }

        public override void Clear()
        {
            Stop();
            Peers.Clear();
            ValidatedPeers.Clear();
        }

        public void Start(int Port = 0, int MaxPlayers = 0)
        {
            if(Port == 0)
            {
                Port = Sys.Ref.Shared.GetObject<ConfigSys>().Shared.Get<int>("Unary_Common.Network.Port");
            }

            if(MaxPlayers == 0)
            {
                MaxPlayers = Sys.Ref.Shared.GetObject<ConfigSys>().Shared.Get<int>("Unary_Common.Network.MaxPlayers");
            }

            Server.Start(Port);
        }

        private void OnConnectionRequest(ConnectionRequest request)
        {
            //request.
        }

        private void OnNetworkError(System.Net.IPEndPoint endPoint, System.Net.Sockets.SocketError socketError)
        {

        }

        private void OnNetworkLatencyUpdate(NetPeer peer, int latency)
        {
            
        }

        private void OnNetworkReceive(NetPeer peer, NetPacketReader reader, DeliveryMethod deliveryMethod)
        {

        }

        private void OnPeerConnected(NetPeer peer)
        {

        }

        private void OnPeerDisconnected(NetPeer peer, DisconnectInfo disconnectInfo)
        {

        }

        public void Stop()
        {
            Server.Stop(true);
        }

        public void RPCID(string EventName, int PeerID, Args Arguments)
        {
            Server.ConnectedPeerList[PeerID].Send(NetworkUtil.Pack(Arguments), DeliveryMethod.ReliableOrdered);

            if(Sys.Ref.AppType.IsHost())
            {
                EventSys.Remote.Invoke(EventName, Arguments);
            }
        }

        public void RPCIDUnreliable(string EventName, int PeerID, Args Arguments)
        {
            Server.ConnectedPeerList[PeerID].Send(NetworkUtil.Pack(Arguments), DeliveryMethod.Unreliable);

            if (Sys.Ref.AppType.IsHost())
            {
                EventSys.Remote.Invoke(EventName, Arguments);
            }
        }

        public void RPCIDAll(string EventName, Args Arguments)
        {
            foreach(var Peer in Server.ConnectedPeerList)
            {
                Peer.Send(NetworkUtil.Pack(Arguments), DeliveryMethod.ReliableOrdered);
            }

            if (Sys.Ref.AppType.IsHost())
            {
                EventSys.Remote.Invoke(EventName, Arguments);
            }
        }

        public void RPCIDAllUnreliable(string EventName, Args Arguments)
        {
            foreach (var Peer in Server.ConnectedPeerList)
            {
                Peer.Send(NetworkUtil.Pack(Arguments), DeliveryMethod.Unreliable);
            }

            if (Sys.Ref.AppType.IsHost())
            {
                EventSys.Remote.Invoke(EventName, Arguments);
            }
        }

        public void OnAuthResponse(AuthResponse Arguments)
        {

        }

        public void OnTicketResponse(TicketResponse Arguments)
        {
            
        }

        private void RemovePeer(int Peer, string Reason)
        {
            //Server.DisconnectPeer()
        }

        public void ConnectionKick(int ID, string Reason)
        {
            RPCID("Unary_Common.Disconnected", ID, new PlayerDisconnected
            {
                ID = ID,
                Reason = Reason
            });
            RemovePeer(ID, Reason);
        }

        public void ConnectionBan(int ID, string Reason)
        {
            RPCID("Unary_Common.Disconnected", ID, new PlayerDisconnected
            {
                ID = ID,
                Reason = Reason
            });
            RemovePeer(ID, Reason);
        }

        public void Kick(int ID, string Reason)
        {
            RPCIDAll("Unary_Common.Left", new PlayerLeft
            {
                ID = ID,
                Reason = Reason
            });
            RemovePeer(ID, Reason);
        }

        public void Ban(int ID, string Reason)
        {
            RPCIDAll("Unary_Common.Left", new PlayerLeft
            {
                ID = ID,
                Reason = Reason
            });
            RemovePeer(ID, Reason);
        }

        public void SyncInfo(int Peer)
        {
            foreach (var Entry in Sys.Ref.Server.GetAll())
            {
                string ModIDEntry = ModIDUtil.FromType(Entry.GetType());

                RPCID("Unary_Common.Sync." + ModIDUtil.ModIDTarget(ModIDEntry), Peer, Entry.Sync());
            }
        }

        [Remote]
        public void S(uint EventIndex, Args Arguments)
        {
            string EventName = RegistrySys.GetEntry("Unary_Common.Events", EventIndex);

            if (ValidatedPeers[Multiplayer.GetRpcSenderId()] == true)
            {
                Arguments.ID = Multiplayer.GetRpcSenderId();
                EventSys.Remote.Invoke(EventName, Arguments);
            }
            else
            {
                if(EventName == "Unary_Common.Auth" && Arguments is SteamPlayer)
                {
                    EventSys.Internal.Invoke("Unary_Common.Auth", Arguments);
                }
                else
                {
                    ConnectionKick(Multiplayer.GetRpcSenderId(), "ByServer");
                }
            }
        }
    }
}
