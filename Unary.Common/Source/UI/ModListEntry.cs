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

using Godot;

using Unary.Common.Shared;

namespace Unary.Common.UI
{
    public class ModListEntry : HBoxContainer
    {
        /*
        private ModsList ModsList;

        private TextureRect Logo;
        private Button ModName;
        private Button Up;
        private Button Down;
        private Button Enabled;

        //MoveChild()
        //GetIndex()

        public override void _Ready()
        {
            ModsList = ModsList.Instance;

            Logo = GetNode<TextureRect>("Logo");
            ModName = GetNode<Button>("Name");
            ModName.Connect("pressed", this, nameof(OnClick));
            Up = GetNode<Button>("Up");
            Up.Connect("pressed", this, nameof(OnUp));
            Down = GetNode<Button>("Down");
            Down.Connect("pressed", this, nameof(OnDown));
            Enabled = GetNode<Button>("Enabled");
        }

        public void Set(Mod NewMod, bool IsEnabled)
        {
            Logo.Texture = NewMod.Images.Logo;
            ModName.Text = NewMod.Name;
            Enabled.Pressed = IsEnabled;
        }

        private void OnClick()
        {
            
        }

        private void OnUp()
        {

        }

        private void OnDown()
        {

        }
        */
    }
}
