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
using Unary_Common.Structs;

using Godot;
using System.Collections.Generic;

namespace Unary_Common.Shared
{
    public class OSSys : Node, IShared
    {
        public void Init()
        {
            ConfigSys ConfigSys = Sys.Ref.GetShared<ConfigSys>();

            ConfigSys.SubscribeShared(this, nameof(SetUseVsync), 
            "Unary_Common.Window.VSync", SubscriberType.Method);
            ConfigSys.SubscribeShared(this, nameof(SetWindowFullscreen), 
            "Unary_Common.Window.Fullscreen", SubscriberType.Method);
            ConfigSys.SubscribeShared(this, nameof(SetWindowTitle), 
            "Unary_Common.Window.Title", SubscriberType.Method);
            ConfigSys.SubscribeShared(this, nameof(SetWindowSize), 
            "Unary_Common.Window.Size", SubscriberType.Method);
        }

        public void Clear()
        {

        }

        public void ClearMod(Mod Mod)
        {

        }

        public void ClearedMods()
        {

        }

        public void SetUseVsync(bool Value)
        {
            OS.VsyncEnabled = Value;
        }

        public void SetWindowFullscreen(bool Value)
        {
            OS.WindowFullscreen = Value;
        }

        public void SetWindowTitle(string Value)
        {
            OS.SetWindowTitle(Value);
        }

        public void SetWindowSize(Vector2 NewSize)
        {
            OS.WindowSize = NewSize;
        }

        public void InitCore(Mod Mod)
        {
            
        }

        public void InitMod(Mod Mod)
        {
            
        }
    }
}
