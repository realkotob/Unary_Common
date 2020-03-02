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

using Unary_Common.Utils;
using Unary_Common.Interfaces;
using Unary_Common.Structs;

using Godot;
using System.Collections.Generic;

namespace Unary_Common.Shared
{
	public class SceneSys : Node, IShared
	{
		private string ModID;
		private string Path;

		public Node GetScene()
		{
			return GetChild(0);
		}

		public void Init()
		{

		}

		public override void _Ready()
		{
			if (ModID != null && Path != null)
			{
				Node NewNode = NodeUtil.NewNode(ModID, Path);

				if (GetChildCount() != 0)
				{
					GetChild(0).QueueFree();
				}

				AddChild(NewNode, true);
			}
		}

		public void Clear()
		{
			if(GetChildCount() != 0)
			{
				GetChild(0).QueueFree();
			}
		}

		public void ClearMod(Mod Mod)
		{

		}

		public void ClearedMods()
		{

		}

		public void ChangeScene<T>(string ModID, string Path) where T : Node
		{
			T NewNode = NodeUtil.NewNode<T>(ModID, Path);

			if (GetChildCount() != 0)
			{
				GetChild(0).QueueFree();
			}

			AddChild(NewNode, true);

			this.ModID = ModID;
			this.Path = Path;
		}

		public void InitCore(Mod Mod)
		{
			
		}

		public void InitMod(Mod Mod)
		{
			
		}
	}
}
