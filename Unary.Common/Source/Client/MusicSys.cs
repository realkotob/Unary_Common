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

using Unary.Common.Shared;
using Unary.Common.Interfaces;
using Unary.Common.Utils;
using Unary.Common.Abstract;

using Godot;

using System;
using System.Collections.Generic;

namespace Unary.Common.Client
{
    public class MusicSys : SysNode
    {
        private AudioStreamPlayer Player;

        public override void Init()
        {
            Player = new AudioStreamPlayer();
            Player.Bus = "Music";
        }

        public override void _Ready()
        {
            if (GetChildCount() != 0)
            {
                GetChild(0).QueueFree();
            }

            CallDeferred("add_child", Player);
        }

        public override void Clear()
        {
            Player.Stop();

            if (GetChildCount() != 0)
            {
                GetChild(0).QueueFree();
            }
        }

        public void Play(string ModID, string Path)
        {
            Player.Stream = NodeUtil.NewResource<AudioStream>(ModID, "Music", Path);
            Player.Play();
        }
    }
}