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
using Unary_Common.Abstract;
using Unary_Common.Arguments;
using Unary_Common.Shared;

using System;
using System.Collections.Generic;

namespace Unary_Common.Client
{
    public class RegistrySys : SysObject
    {
        private Shared.RegistrySys RegistrySysShared;
        private EventSys EventSys;

        public override void Init()
        {
            RegistrySysShared = Sys.Ref.Shared.GetObject<Shared.RegistrySys>();
            EventSys = Sys.Ref.Shared.GetNode<EventSys>();

            EventSys.SubscribeRPC(this, nameof(AddEntry), "Unary_Common.RegistrySys.AddEntry");
            EventSys.SubscribeRPC(this, nameof(RemoveEntry), "Unary_Common.RegistrySys.RemoveEntry");
        }

        public override void Clear()
        {
            RegistrySysShared.Synced = false;
        }

        public override void Sync(Args Arguments)
        {
            if (Arguments is RegistrySysSync Sync)
            {
                RegistrySysShared.Registry = Sync.Registry;
            }
        }

        public void AddEntry(Args Args)
        {
            if(Args is RegistrySysEntry Entry)
            {
                RegistrySysShared.AddEntry(Entry.RegistryName, Entry.ModIDEntry);
            }
        }

        public void RemoveEntry(Args Args)
        {
            if (Args is RegistrySysEntry Entry)
            {
                RegistrySysShared.RemoveEntry(Entry.RegistryName, Entry.ModIDEntry);
            }
        }
    }
}