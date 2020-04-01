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

using Unary_Common;
using Unary_Common.Interfaces;
using Unary_Common.Shared;
using Unary_Common.Abstract;

using System;
using System.Collections.Generic;

using Godot;

namespace Unary_Common.Client
{
    public class AlertSys : SysUI
    {
        private LocaleSys LocaleSys;
        private WindowDialog Window;
        private RichTextLabel Text;
        private Button Button;

        public override void Init()
        {
            LocaleSys = Sys.Ref.Client.GetObject<LocaleSys>();
        }

        public override void _Ready()
        {
            Window = GetNode<WindowDialog>("Window");
            Text = GetNode<RichTextLabel>("Window/Container/Text");
            Button = GetNode<Button>("Window/Container/Button");
            Button.Connect("pressed", this, nameof(OnClose));
        }

        public override void Clear()
        {
            Button.Disconnect("pressed", this, nameof(OnClose));
        }

        private void OnClose()
        {
            Window.Hide();
        }

        public void Invoke(string NewText = null, string NewButtonText = null, string NewTitle = null)
        {
            if(NewTitle == null)
            {
                Window.WindowTitle = LocaleSys.Translate("Unary_Common.UI.Alert.Title");
            }
            else
            {
                Window.WindowTitle = NewTitle;
            }

            if (NewText == null)
            {
                Text.Text = LocaleSys.Translate("Unary_Common.UI.Alert.Text");
            }
            else
            {
                Text.Text = NewText;
            }

            if (NewButtonText == null)
            {
                Button.Text = LocaleSys.Translate("Unary_Common.UI.Alert.Button");
            }
            else
            {
                Button.Text = NewButtonText;
            }
        }
    }
}