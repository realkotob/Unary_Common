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

using Unary_Common.Abstract;
using Unary_Common.Interfaces;
using Unary_Common.Utils;

using Godot;

namespace Unary_Common.Server
{
    public class ConsoleSys : SysNode, IConsoleSys
    {
        public override void Init()
        {
            if (FilesystemUtil.SystemFileExists("log.old"))
            {
                FilesystemUtil.SystemFileDelete("log.old");
            }

            if (FilesystemUtil.SystemFileExists("log.txt"))
            {
                FilesystemUtil.SystemFileMove("log.txt", "log.old");
            }

            if (!FilesystemUtil.SystemFileExists("log.txt"))
            {
                FilesystemUtil.SystemFileCreate("log.txt");
            }
        }

        private void WriteToLog(string Type, string Text)
        {
            var Time = OS.GetTime();
            string Result = "[" + Time["hour"] + ':' + Time["minute"] + ':' + Time["second"] + "][" + Type + "]: " + Text + System.Environment.NewLine;
            FilesystemUtil.SystemFileAppend("log.txt", Result);
        }

        public override void Clear()
        {

        }

        public void Message(string Text)
        {
            WriteToLog("Message", Text);
            GD.Print(Text);
        }

        public void Warning(string Text)
        {
            WriteToLog("Warning", Text);
            GD.PushWarning(Text);
        }

        public void Error(string Text)
        {
            WriteToLog("Error", Text);
            GD.PushError(Text);
        }

        public void Panic(string Text)
        {
            WriteToLog("Panic", Text);
            GD.PushError(Text);
            Main.Ref.Quit();
        }
    }
}
