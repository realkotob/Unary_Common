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
using Unary.Common.Structs;
using Unary.Common.Arguments;
using Unary.Common.Arguments.Internal;
using Unary.Common.Utils;
using Unary.Common.Abstract;

using System;
using System.Collections.Generic;

using Steamworks;

namespace Unary.Common.Server
{
    public class SteamSys : SysNode
    {
        private Callback<P2PSessionRequest_t> RequestCallback;
        private Callback<P2PSessionConnectFail_t> RequestFailCallback;

        public override void Init()
        {
            RequestCallback = Callback<P2PSessionRequest_t>.Create(OnSessionRequest);
            RequestFailCallback = Callback<P2PSessionConnectFail_t>.Create(OnSessionRequestFail);
        }

        public override void Clear()
        {

        }

        public void OnSessionRequest(P2PSessionRequest_t Args)
        {

        }

        public void OnSessionRequestFail(P2PSessionConnectFail_t Args)
        {

        }
    }
}
