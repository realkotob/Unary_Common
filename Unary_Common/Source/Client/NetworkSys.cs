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

using System;
using System.Collections.Generic;

using Godot;

using Steamworks;
using MessagePack;

namespace Unary_Common.Client
{
    public class NetworkSys : Node, IClient
    {

        public void Init()
        {

        }

        public void Clear()
        {

        }

        public void Start(int Port = 0, int MaxPlayers = 0)
        {
            if (Port == 0)
            {
                Port = Sys.Ref.GetShared<ConfigSys>().GetShared<int>("Unary_Common.Network.Port");
            }

            if (MaxPlayers == 0)
            {
                MaxPlayers = Sys.Ref.GetShared<ConfigSys>().GetShared<int>("Unary_Common.Network.MaxPlayers");
            }

            NetworkedMultiplayerENet NewPeer = new NetworkedMultiplayerENet();
            NewPeer.CreateServer(Port, MaxPlayers);

            NewPeer.Crea

            GetTree().NetworkPeer = NewPeer;
        }
    }
}
