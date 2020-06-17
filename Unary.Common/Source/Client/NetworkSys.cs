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
using Unary.Common.Shared;
using Unary.Common.Structs;
using Unary.Common.Enums;
using Unary.Common.Abstract;
using Unary.Common.Arguments;
using Unary.Common.Arguments.Internal;
using Unary.Common.Arguments.Remote;

using System;
using System.Collections.Generic;
using System.Text;

using Godot;

using Steamworks;
using MessagePack;
using LiteNetLib;
using LiteNetLib.Utils;

namespace Unary.Common.Client
{
    public class NetworkSys : SysNode
    {
        private EventSys EventSys;
        private SteamSys SteamSys;
        private RegistrySys RegistrySys;
        private LocaleSys LocaleSys;

        private EventBasedNetListener Listener;
        private NetManager Client;

        public override void Init()
        {
            EventSys = Sys.Ref.Shared.GetNode<EventSys>();
            SteamSys = Sys.Ref.Client.GetObject<SteamSys>();
            RegistrySys = Sys.Ref.Shared.GetObject<RegistrySys>();
            LocaleSys = Sys.Ref.Client.GetObject<LocaleSys>();

            Listener = new EventBasedNetListener();

            Listener.NetworkErrorEvent += OnNetworkError;
            Listener.NetworkLatencyUpdateEvent += OnNetworkLatencyUpdate;
            Listener.NetworkReceiveEvent += OnNetworkReceive;
            Listener.PeerDisconnectedEvent += OnPeerDisconnected;
        }

        public void Connect(string Address = "127.0.0.1", int Port = 0)
        {
            if (Port == 0)
            {
                Port = Sys.Ref.Shared.GetObject<ConfigSys>().Shared.Get<int>("Unary.Common.Network.Port");
            }

            NetDataWriter NewWriter = new NetDataWriter();

            SteamPlayer NewPlayer = new SteamPlayer
            {
                SteamID = SteamSys.GetSteamID(),
                Ticket = SteamSys.GetAuthTicket()
            };

            NewWriter.Put(NetworkUtil.Pack("Unary.Common.Network.AuthPlayer", NewPlayer));
            
            if(Client.Connect(Address, Port, NewWriter) != null)
            {
                EventSys.Internal.Invoke("Unary.Common.Network.Connected", NewPlayer);
            }
            else
            {
                EventSys.Internal.Invoke("Unary.Common.Network.FailedToConnect", NewPlayer);
            }
        }

        public void Disconnect()
        {
            Client.Stop(true);
            EventSys.Internal.Invoke("Unary.Common.Network.Disconnected", null);
        }

        private void OnNetworkError(System.Net.IPEndPoint endPoint, System.Net.Sockets.SocketError socketError)
        {
            Disconnect();
            EventSys.Internal.Invoke("Unary.Common.Network.Disconnected",
            new PeerDisconnected() { ID = Client.FirstPeer.Id, Reason = "SocketError" });
        }

        private void OnNetworkLatencyUpdate(NetPeer peer, int latency)
        {
            EventSys.Internal.Invoke("Unary.Common.Network.LatencyUpdate", new LatencyUpdate() { ID = peer.Id, Latency = latency });
        }

        private void OnNetworkReceive(NetPeer peer, NetPacketReader reader, DeliveryMethod deliveryMethod)
        {
            string EventName = RegistrySys.GetEntry("Unary.Common.Events", reader.GetUInt());
            Args NewArgs = NetworkUtil.Unpack(reader.GetRemainingBytes());
            NewArgs.ID = peer.Id;
            EventSys.Remote.Invoke(EventName, NewArgs);
        }

        private void OnPeerDisconnected(NetPeer peer, DisconnectInfo disconnectInfo)
        {
            Disconnect();

            PeerDisconnected Disconnected = new PeerDisconnected
            {
                ID = Client.FirstPeer.Id,
            };

            if(disconnectInfo.AdditionalData.AvailableBytes != 0)
            {
                Enums.DisconnectReason Reason = (Enums.DisconnectReason)disconnectInfo.AdditionalData.GetByte();

                switch (Reason)
                {
                    case Enums.DisconnectReason.Full:
                        Disconnected.Reason = LocaleSys.Translate("Unary.Common.Left.Full");
                        break;
                    case Enums.DisconnectReason.Invalid:
                        Disconnected.Reason = LocaleSys.Translate("Unary.Common.Left.Invalid");
                        break;
                    case Enums.DisconnectReason.Banned:
                        Disconnected.Reason = string.Format(LocaleSys.Translate("Unary.Common.Left.Invalid"),
                        new DateTime(disconnectInfo.AdditionalData.GetLong()).ToString(),
                        Encoding.UTF8.GetString(disconnectInfo.AdditionalData.GetRemainingBytes()));
                        break;
                    case Enums.DisconnectReason.Kicked:
                        Disconnected.Reason = string.Format(LocaleSys.Translate("Unary.Common.Left.Kicked"),
                        Encoding.UTF8.GetString(disconnectInfo.AdditionalData.GetRemainingBytes()));
                        break;
                    case Enums.DisconnectReason.OutOfTurn:
                        Disconnected.Reason = LocaleSys.Translate("Unary.Common.Left.OutOfTurn");
                        break;
                }
            }
            else if (disconnectInfo.SocketErrorCode != System.Net.Sockets.SocketError.Success)
            {
                Disconnected.Reason = LocaleSys.Translate("Unary.Common.Left.SocketError");
            }
            else
            {
                Disconnected.Reason = EnumUtil.GetStringFromKey(disconnectInfo.Reason);
            }

            EventSys.Internal.Invoke("Unary.Common.Network.Disconnected", Disconnected);
        }

        public void Send(string EventName, Args Arguments)
        {
            NetDataWriter NewWriter = new NetDataWriter();
            NewWriter.Put(RegistrySys.GetEntry("Unary.Common.Events", EventName));
            NewWriter.Put(NetworkUtil.Pack(Arguments));
            Client.FirstPeer.Send(NewWriter, DeliveryMethod.ReliableOrdered);
        }

        public void SendUnreliable(string EventName, Args Arguments)
        {
            NetDataWriter NewWriter = new NetDataWriter();
            NewWriter.Put(RegistrySys.GetEntry("Unary.Common.Events", EventName));
            NewWriter.Put(NetworkUtil.Pack(Arguments));
            Client.FirstPeer.Send(NewWriter, DeliveryMethod.Unreliable);
        }
    }
}
