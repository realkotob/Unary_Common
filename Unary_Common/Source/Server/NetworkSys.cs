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
using Unary_Common.Enums;
using Unary_Common.Arguments.Internal;
using Unary_Common.Arguments.Remote;
using Unary_Common.Abstract;
using Unary_Common.Utils;
using System.Text;

using System;
using System.Collections.Generic;

using Godot;

using LiteNetLib;
using LiteNetLib.Utils;

namespace Unary_Common.Server
{
    public class NetworkSys : SysNode
    {
        private EventSys EventSys;
        private SteamSys SteamSys;
        private RegistrySys RegistrySys;
        private BanSys BanSys;

        private EventBasedNetListener Listener;
        private NetManager Server;

        private Dictionary<int, NetPeer> NonValidatedPeers;
        private Dictionary<int, NetPeer> ValidatedPeers;

        private int MaxPlayers;

        public override void Init()
        {
            EventSys = Sys.Ref.Shared.GetNode<EventSys>();
            SteamSys = Sys.Ref.Server.GetObject<SteamSys>();
            RegistrySys = Sys.Ref.Shared.GetObject<RegistrySys>();
            BanSys = Sys.Ref.Server.GetObject<BanSys>();

            Listener = new EventBasedNetListener();

            Listener.ConnectionRequestEvent += OnConnectionRequest;
            Listener.NetworkErrorEvent += OnNetworkError;
            Listener.NetworkLatencyUpdateEvent += OnNetworkLatencyUpdate;
            Listener.NetworkReceiveEvent += OnNetworkReceive;
            Listener.PeerConnectedEvent += OnPeerConnected;
            Listener.PeerDisconnectedEvent += OnPeerDisconnected;

            Server = new NetManager(Listener);

            NonValidatedPeers = new Dictionary<int, NetPeer>();
            ValidatedPeers = new Dictionary<int, NetPeer>();

            EventSys.Internal.Subscribe(this, nameof(OnAuthResponse), "Unary_Common.Steam.AuthResponse");
            EventSys.Internal.Subscribe(this, nameof(OnTicketResponse), "Unary_Common.Steam.TicketResponse");
        }

        public override void Clear()
        {
            Stop();
            NonValidatedPeers.Clear();
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
                this.MaxPlayers = Sys.Ref.Shared.GetObject<ConfigSys>().Shared.Get<int>("Unary_Common.Network.MaxPlayers");
            }
            else
            {
                this.MaxPlayers = MaxPlayers;
            }

            Server.Start(Port);
            EventSys.Internal.Invoke("Unary_Common.Network.Start", null);
        }

        public void Stop()
        {
            Server.Stop(true);
            EventSys.Internal.Invoke("Unary_Common.Network.Stop", null);
        }

        private void OnConnectionRequest(ConnectionRequest request)
        {
            if(ValidatedPeers.Count == MaxPlayers)
            {
                NetDataWriter NewWriter = new NetDataWriter();
                NewWriter.Put((byte)Enums.DisconnectReason.Full);
                request.Reject(NewWriter);
                return;
            }

            Args Auth = NetworkUtil.Unpack(request.Data.GetRemainingBytes());

            if (Auth is SteamPlayer NewSteamPlayer)
            {
                if(BanSys.IsBanned(NewSteamPlayer.SteamID))
                {
                    Ban Ban = BanSys.GetBan(NewSteamPlayer.SteamID);

                    if(Ban.Time >= DateTimeOffset.UtcNow.ToUnixTimeSeconds())
                    {
                        NetDataWriter NewWriter = new NetDataWriter();
                        NewWriter.Put((byte)Enums.DisconnectReason.Banned);
                        NewWriter.Put(Ban.Time);
                        NewWriter.Put(Encoding.UTF8.GetBytes(Ban.Reason));
                        request.Reject(NewWriter);
                        return;
                    }
                    else
                    {
                        BanSys.Unban(NewSteamPlayer.SteamID);
                    }
                }

                NetPeer NewPeer = request.Accept();
                NonValidatedPeers[NewPeer.Id] = NewPeer;

                EventSys.Internal.Invoke("Unary_Common.Network.Auth", NewSteamPlayer);
            }
            else
            {
                NetDataWriter NewWriter = new NetDataWriter();
                NewWriter.Put((byte)Enums.DisconnectReason.Invalid);
                request.Reject(NewWriter);
                return;
            }
        }

        private void OnNetworkError(System.Net.IPEndPoint endPoint, System.Net.Sockets.SocketError socketError)
        {
            Sys.Ref.ConsoleSys.Warning("Reported NetworkError on " + endPoint.ToString() + " with an error " + EnumUtil.GetStringFromKey(socketError));

            List<int> ResponsiblePeers = new List<int>();

            foreach(var Peer in ValidatedPeers)
            {
                if(Peer.Value.EndPoint == endPoint)
                {
                    ResponsiblePeers.Add(Peer.Key);
                }
            }

            string ResponsibleString = default;

            for(int i = 0; i < ResponsiblePeers.Count - 1; ++i)
            {
                ResponsibleString += ResponsiblePeers[i];

                if(i != ResponsiblePeers.Count - 2)
                {
                    ResponsibleString += ", ";
                }
            }

            Sys.Ref.ConsoleSys.Warning("Potentially responsible peers are : " + ResponsibleString);
        }

        private void OnNetworkLatencyUpdate(NetPeer peer, int latency)
        {
            EventSys.Internal.Invoke("Unary_Common.Network.LatencyUpdate", new LatencyUpdate() { ID = peer.Id, Latency = latency });
        }

        private void OnNetworkReceive(NetPeer peer, NetPacketReader reader, DeliveryMethod deliveryMethod)
        {
            if(ValidatedPeers.ContainsKey(peer.Id))
            {
                uint EventIndex = reader.GetUInt();

            }
            else
            {
                NonValidatedPeers.Remove(peer.Id);
                NetDataWriter NewWriter = new NetDataWriter();
                NewWriter.Put((byte)Enums.DisconnectReason.OutOfTurn);
                peer.Disconnect(NewWriter);
                return;
            }
        }

        private void OnPeerConnected(NetPeer peer)
        {
            Peers[peer.Id] = peer;
            ValidatedPeers[peer.Id] = false;
        }

        private void OnPeerDisconnected(NetPeer peer, DisconnectInfo disconnectInfo)
        {


            if(disconnectInfo.AdditionalData.AvailableBytes != 0)
            {

            }
            else if(disconnectInfo.SocketErrorCode != System.Net.Sockets.SocketError.Success)
            {

            }

            if (ValidatedPeers.ContainsKey(Peer))
            {
                SendAll("Unary_Common.Event.PlayerLeft", new PlayerLeft() { ID = Peer, SteamID = SteamSys.GetSteamID(Peer), Reason = Reason });
            }
            else if (NonValidatedPeers.ContainsKey(Peer))
            {
                NonValidatedPeers[Peer].Disconnect()
            }

            EventSys.Internal.Invoke("Unary_Common.Network.Disconnected", new PeerDisconnected() { ID = peer.Id, Reason  })
        }

        private void DisconnectPeer(int Peer, string Reason)
        {
            
        }

        public void Send(string EventName, int PeerID, Args Arguments)
        {
            NetDataWriter NewWriter = new NetDataWriter();
            NewWriter.Put(Netw)

            NewWriter.Put(RegistrySys.GetEntry("Unary_Common.Events", EventName));
            NewWriter.Put(NetworkUtil.Pack(Arguments));
            ValidatedPeers[PeerID].Send(NewWriter, DeliveryMethod.ReliableOrdered);
        }

        public void SendUnreliable(string EventName, int PeerID, Args Arguments)
        {
            NetDataWriter NewWriter = new NetDataWriter();
            NewWriter.Put(RegistrySys.GetEntry("Unary_Common.Events", EventName));
            NewWriter.Put(NetworkUtil.Pack(Arguments));
            ValidatedPeers[PeerID].Send(NewWriter, DeliveryMethod.Unreliable);
        }

        public void SendAll(string EventName, Args Arguments)
        {
            NetDataWriter NewWriter = new NetDataWriter();
            NewWriter.Put(RegistrySys.GetEntry("Unary_Common.Events", EventName));
            NewWriter.Put(NetworkUtil.Pack(Arguments));

            foreach (var Peer in ValidatedPeers)
            {
                Peer.Value.Send(NewWriter, DeliveryMethod.ReliableOrdered);
            }
        }

        public void SendAlllUnreliable(string EventName, Args Arguments)
        {
            NetDataWriter NewWriter = new NetDataWriter();
            NewWriter.Put(RegistrySys.GetEntry("Unary_Common.Events", EventName));
            NewWriter.Put(NetworkUtil.Pack(Arguments));

            foreach (var Peer in ValidatedPeers)
            {
                Peer.Value.Send(NewWriter, DeliveryMethod.Unreliable);
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

        public void SyncInfo(int Peer)
        {
            foreach (var Entry in Sys.Ref.Server.GetAll())
            {
                string ModIDEntry = ModIDUtil.FromType(Entry.GetType());

                Send("Unary_Common.Sync." + ModIDUtil.ModIDTarget(ModIDEntry), Peer, Entry.Sync());
            }
        }
    }
}
