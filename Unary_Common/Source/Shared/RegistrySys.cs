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

using Unary_Common.Abstract;
using Unary_Common.Arguments;
using Unary_Common.Structs;
using Unary_Common.Subsystems;

using System;
using System.Collections.Generic;
using System.Linq;

namespace Unary_Common.Shared
{
    public class RegistrySys : SysObject
    {
        public RegistryManager Client { get; private set; }
        public RegistryManager Server { get; private set; }

        public override void Init()
        {
            Client = new RegistryManager();
            Server = new RegistryManager();
        }

        public override void ClearMod(Mod Mod)
        {
            Client.ClearMod(Mod);
            Server.ClearMod(Mod);
        }

        public override void Clear()
        {
            Client.Clear();
            Server.Clear();
        }

        public override void Sync(Args Arguments)
        {
            if(Arguments is RegistrySync Sync)
            {
                Client.Registry = Sync.Registry;
            }
        }

        public override Args Sync()
        {
            return new RegistrySync() { Registry = Server.Registry };
        }
    }
}