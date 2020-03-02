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
using Unary_Common.Utils;
using Unary_Common.Structs;

using System;
using System.Collections.Generic;

namespace Unary_Common.Client
{
    public class DownloadsSys : Godot.Object, IClient
    {
        private EventSys EventSys;

        private bool IsProcessing = false;

        private byte[] Buffer;

        public void Init()
        {
            EventSys = Sys.Ref.GetSharedNode<EventSys>();

            Buffer = new byte[65500];

            EventSys.SubscribeRPC(this, nameof(Start), "Unary_Common.DownloadsSys.Start");
            EventSys.SubscribeRPC(this, nameof(Process), "Unary_Common.DownloadsSys.Process");
            EventSys.SubscribeRPC(this, nameof(End), "Unary_Common.DownloadsSys.End");
            EventSys.SubscribeRPC(this, nameof(EndAll), "Unary_Common.DownloadsSys.EndAll");
        }

        public void Clear()
        {

        }

        public void Start(Arguments Arguments)
        {

        }

        public void Process(Arguments Arguments)
        {
            //Arguments.Get<byte[]>("Unary_Common.Data")
        }

        public void End(Arguments Arguments)
        {

        }

        public void EndAll(Arguments Arguments)
        {

        }
    }
}
