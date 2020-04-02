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
using System.Text;

using Unary_Common.Abstract;

using LiteNetLib;
using LiteNetLib.Utils;

namespace Unary_Common.Shared
{
    public class RCONSys : SysNode
    {
        private EventBasedNetListener Listener;
        private NetManager Server;

        public override void Init()
        {
            Listener = new EventBasedNetListener();
            Server = new NetManager(Listener);

            Listener.ConnectionRequestEvent += OnConnectionRequest;
            Listener.PeerConnectedEvent += OnPeerConnected;
            Listener.PeerDisconnectedEvent += OnPeerDisconnected;
            Listener.NetworkReceiveEvent += OnNetworkReceive;

            Server.Start(Sys.Ref.Shared.GetObject<ConfigSys>().GetShared<int>("Unary_Common.Network.RCONPort"));
        }

        public override void Clear()
        {
            Server.Stop();
        }

        public override void _Process(float delta)
        {
            Server.PollEvents();
        }

        private void OnNetworkReceive(NetPeer peer, NetPacketReader reader, DeliveryMethod deliveryMethod)
        {
            Sys.Ref.Shared.GetNode<InterpreterSys>().ProcessScript(Encoding.UTF8.GetString(reader.GetRemainingBytes()));
        }

        private void OnPeerDisconnected(NetPeer peer, DisconnectInfo disconnectInfo)
        {
            Sys.Ref.ConsoleSys.Message("RCONPeer " + peer.EndPoint.ToString() + " disconnected");
        }

        private void OnPeerConnected(NetPeer peer)
        {
            Sys.Ref.ConsoleSys.Message("RCONPeer " + peer.EndPoint.ToString() + " connected");
        }

        private void OnConnectionRequest(ConnectionRequest request)
        {
            request.AcceptIfKey(Sys.Ref.Shared.GetObject<ConfigSys>().GetShared<string>("Unary_Common.Network.RCONPassword"));
        }

        public void Send(string Result, ConsoleSys.ConsoleColors Color)
        {
            NetDataWriter Writer = new NetDataWriter();
            Writer.Put((byte)Color);
            Writer.Put(Encoding.UTF8.GetBytes(Result));

            foreach (var Peer in Server.GetPeers(ConnectionState.Any))
            {
                Peer.Send(Writer, DeliveryMethod.ReliableOrdered);
            }
        }
    }
}
