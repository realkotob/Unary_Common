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

using Unary.Common.Structs;
using Unary.Common.Arguments;

namespace Unary.Common.Abstract
{
    public class SysUI : Godot.CanvasLayer, ISysEntry
    {
        // Executed instead of a constructor for Object/Node inherited systems
        public virtual void Init()
        {

        }

        // Executed instead of a deconstuctor for Node base systems
        public virtual void Clear()
        {

        }

        // Executed when requested to clear specified ModID
        public virtual void ClearMod(Mod Mod)
        {

        }

        // Executed after all the mods have been cleaned
        public virtual void ClearedMods()
        {

        }

        // Executed when requested to implement Core namespace
        public virtual void InitCore(Mod Mod)
        {

        }

        //Executed when requested to implement Mod namespace
        public virtual void InitMod(Mod Mod)
        {

        }

        //Executed when requesting a system to provide sync info to client
        public virtual Args Sync()
        {
            return null;
        }

        //Executed when system is provided with sync info from client
        public virtual void Sync(Args Arguments)
        {

        }
    }
}
