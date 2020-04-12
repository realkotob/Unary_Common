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
using Unary_Common.Shared;
using Unary_Common.Utils;
using Unary_Common.Abstract;
using Unary_Common.Subsystems;

namespace Unary_Common.Shared
{
	public class ConfigSys : SysObject
	{
		public ConfigManager Server { get; private set; }
		public ConfigManager Client { get; private set; }
		public ConfigManager Shared { get; private set; }

		public override void Init()
		{
			Server = new ConfigManager("Server");
			Client = new ConfigManager("Client");
			Shared = new ConfigManager("Shared");

			Shared.Load("Unary_Common", ".");
		}

		public override void Clear()
		{
			Server.Clear();
			Client.Clear();
			Shared.Clear();
		}

		public override void ClearMod(Mod Mod)
		{
			Server.ClearMod(Mod);
			Client.ClearMod(Mod);
			Shared.ClearMod(Mod);
		}

		private void LoadConfig(string ModID, string Path)
		{
			Server.Load(ModID, Path);
			Client.Load(ModID, Path);
			Shared.Load(ModID, Path);
		}

		public override void InitCore(Mod Mod)
		{
			LoadConfig(Mod.ModID, Mod.Path);
		}

		public override void InitMod(Mod Mod)
		{
			LoadConfig(Mod.ModID, Mod.Path);
		}

		public void Reload()
		{
			Server.Reload();
			Client.Reload();
			Shared.Reload();
		}
	}
}
