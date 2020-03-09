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

using System;
using System.Collections.Generic;

using Godot;

namespace Unary_Common.Server
{
    public class NetworkSys : SysNode
    {
        private EventSys EventSys;
        private SteamSys SteamSys;

        private Dictionary<int, bool> ValidatedPeers;

        public override void Init()
        {
            EventSys = Sys.Ref.Shared.GetNode<EventSys>();
            SteamSys = Sys.Ref.Server.GetObject<SteamSys>();

            ValidatedPeers = new Dictionary<int, bool>();

            EventSys.SubscribeEvent(this, nameof(OnAuthResponse), "Unary_Common.AuthResponse");
            EventSys.SubscribeEvent(this, nameof(OnTicketResponse), "Unary_Common.TicketResponse");
        }

        public void OnPlayerConnected(int PeerID)
        {
            if(!ValidatedPeers.ContainsKey(PeerID))
            {
                ValidatedPeers[PeerID] = false;
            }

            EventSys.InvokeEvent("Unary_Common.Connected", new Args
            {
                Peer = PeerID
            });
        }

        public void OnPlayerDisconnected(int PeerID)
        {
            if (ValidatedPeers.ContainsKey(PeerID))
            {
                ValidatedPeers.Remove(PeerID);
            }

            EventSys.InvokeEvent("Unary_Common.Left", new Args
            {
                Peer = PeerID
            });

            Kick(PeerID, "ByUser");
        }

        public override void Clear()
        {
            Stop();
        }

        public void Start(int Port = 0, int MaxPlayers = 0)
        {
            if(Port == 0)
            {
                Port = Sys.Ref.Shared.GetObject<ConfigSys>().GetShared<int>("Unary_Common.Network.Port");
            }

            if(MaxPlayers == 0)
            {
                MaxPlayers = Sys.Ref.Shared.GetObject<ConfigSys>().GetShared<int>("Unary_Common.Network.MaxPlayers");
            }

            NetworkedMultiplayerENet NewPeer = new NetworkedMultiplayerENet();
            NewPeer.CreateServer(Port, MaxPlayers);

            GetTree().NetworkPeer = NewPeer;
            GetTree().Connect("network_peer_connected", this, nameof(OnPlayerConnected));
            GetTree().Connect("network_peer_disconnected", this, nameof(OnPlayerDisconnected));
            ValidatedPeers[1] = true;
        }

        public void Stop()
        {
            GetTree().NetworkPeer = null;
            GetTree().Disconnect("network_peer_connected", this, nameof(OnPlayerConnected));
            GetTree().Disconnect("network_peer_disconnected", this, nameof(OnPlayerDisconnected));
            ValidatedPeers.Remove(1);
        }

        public void RPCID(string EventName, int PeerID, Args Arguments)
        {
            if(PeerID == 1)
            {
                EventSys.InvokeRPC(EventName, Arguments);
            }
            else
            {
                RpcId(PeerID, "C", EventName, Arguments);
            }
        }

        public void RPCIDUnreliable(string EventName, int PeerID, Args Arguments)
        {
            if (PeerID == 1)
            {
                EventSys.InvokeRPC(EventName, Arguments);
            }
            else
            {
                RpcUnreliableId(PeerID, "C", EventName, Arguments);
            }
        }

        public void RPCIDAll(string EventName, Args Arguments)
        {
            int[] Peers = Multiplayer.GetNetworkConnectedPeers();

            foreach(var PeerID in Peers)
            {
                if (PeerID == 1)
                {
                    EventSys.InvokeRPC(EventName, Arguments);
                }
                else
                {
                    RpcId(PeerID, "C", EventName, Arguments);
                }
            }
        }

        public void RPCIDAllUnreliable(string EventName, Args Arguments)
        {
            int[] Peers = Multiplayer.GetNetworkConnectedPeers();

            foreach (var PeerID in Peers)
            {
                if (PeerID == 1)
                {
                    EventSys.InvokeRPC(EventName, Arguments);
                }
                else
                {
                    RpcUnreliableId(PeerID, "C", EventName, Arguments);
                }
            }
        }

        public void OnAuthResponse(AuthResponse Arguments)
        {
            if(Arguments.Response != "OK")
            {
                ConnectionKick(Arguments.Peer, Arguments.Response);
            }
        }

        public void OnTicketResponse(TicketResponse Arguments)
        {
            if (Arguments.Response != "OK")
            {
                ConnectionKick(Arguments.Peer, Arguments.Response);
            }
            else
            {
                ValidatedPeers[Arguments.Peer] = true;

                RPCIDAll("Unary_Common.Join", new PlayerJoined
                {
                    Peer = Arguments.Peer,
                    Nickname = SteamSys.GetNickname(Arguments.Peer),
                    SteamID = SteamSys.GetSteamID(Arguments.Peer)
                });
            }
        }

        private void RemovePeer(int Peer)
        {
            NetworkedMultiplayerENet NetworkPeer = GetTree().NetworkPeer as NetworkedMultiplayerENet;
            NetworkPeer.DisconnectPeer(Peer, true);
        }

        public void ConnectionKick(int Peer, string Reason)
        {
            RPCID("Unary_Common.Disconnected", Peer, new PlayerDisconnected
            {
                Peer = Peer,
                Reason = Reason
            });
            RemovePeer(Peer);
        }

        public void ConnectionBan(int Peer, string Reason)
        {
            RPCID("Unary_Common.Disconnected", Peer, new PlayerDisconnected
            {
                Peer = Peer,
                Reason = Reason
            });
            RemovePeer(Peer);
        }

        public void Kick(int Peer, string Reason)
        {
            RPCIDAll("Unary_Common.Left", new PlayerLeft
            {
                Peer = Peer,
                Reason = Reason
            });
            RemovePeer(Peer);
        }

        public void Ban(int Peer, string Reason)
        {
            RPCIDAll("Unary_Common.Left", new PlayerLeft
            {
                Peer = Peer,
                Reason = Reason
            });
            RemovePeer(Peer);
        }

        [Remote]
        public void S(string EventName, Args Arguments)
        {
            if(ValidatedPeers[Multiplayer.GetRpcSenderId()] == true)
            {
                Arguments.Peer = Multiplayer.GetRpcSenderId();
                EventSys.InvokeRPC(EventName, Arguments);
            }
            else
            {
                if(EventName == "Unary_Common.Auth" && Arguments is SteamPlayer)
                {
                    EventSys.InvokeEvent("Unary_Common.Auth", Arguments);
                }
                else
                {
                    ConnectionKick(Multiplayer.GetRpcSenderId(), "ByServer");
                }
            }
        }
    }
}
