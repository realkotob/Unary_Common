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

using Unary.Common.Interfaces;
using Unary.Common.UI;
using Unary.Common.Utils;
using Unary.Common.Structs;
using Unary.Common.Enums;

using System;
using System.Collections.Generic;
using System.Linq;

using Godot;

namespace Unary.Common.Shared
{
	public class Sys : Node
	{
		public static Sys Ref { get; private set; }

		public bool Running { get; set; } = true;

		public ConsoleSys ConsoleSys { get; private set; }

		public SysManager Shared { get; private set; }
		public SysManager Server { get; private set; }
		public SysManager Client { get; private set; }

		public SysAppType AppType { get; private set; }

		public List<string> LaunchArguments { get; private set; }

		public override void _Ready()
		{
			string ModTest = "Unary.Common.Lol.Path";

			string ModID = ModIDUtil.ModID(ModTest);
			string ModIDEntry = ModIDUtil.ModIDEntry(ModTest);
			string Target = ModIDUtil.ModIDTarget(ModTest);



			Ref = this;

			Shared = new SysManager();
			Server = new SysManager();
			Client = new SysManager();

			AddChild(Shared);
			AddChild(Server);
			AddChild(Client);

			LaunchArguments = OS.GetCmdlineArgs().ToList();

			AppType = new SysAppType();

			if (LaunchArguments.Contains("server"))
			{
				AppType.SetServer(true);
			}
			else
			{
				AppType.SetClient(true);
			}

			try
			{

				Shared.AddUI(new ConsoleSys(), "Console");

				ConsoleSys = Shared.GetUI<ConsoleSys>();

				Main NewCommon = new Main();
				NewCommon.AddShared();

				Shared.AddObject(new BootSys());

				Shared.GetObject<BootSys>().Add("Unary.Common", NewCommon);

				if (AppType.IsServer())
				{
					Shared.GetObject<BootSys>().AddServer("Unary.Common");
				}
				else
				{
					Shared.GetObject<BootSys>().AddClient("Unary.Common");
				}

				Shared.AddObject(new ModSys());

				Mod CoreMod = Shared.GetObject<ModSys>().Core.Mod;

				Shared.InitCore(CoreMod);

				if (AppType.IsServer())
				{
					Server.InitCore(CoreMod);
				}
				else
				{
					Client.InitCore(CoreMod);
				}

				Shared.GetObject<BootSys>().AddShared(CoreMod.ModID);

				if (AppType.IsServer())
				{
					Shared.GetObject<BootSys>().AddServer(CoreMod.ModID);
				}
				else
				{
					Shared.GetObject<BootSys>().AddClient(CoreMod.ModID);
				}
			}
			catch(Exception Exception)
			{
				ConsoleSys.Panic(Exception.Message);
			}
		}

		public void Quit()
		{
			Running = false;

			Server.ClearMods();
			Client.ClearMods();
			Shared.ClearMods();

			Server.Clear();
			Client.Clear();
			Shared.Clear();

			GetTree().Quit();
		}

		public override void _Notification(int what)
		{
			if (what == MainLoop.NotificationWmQuitRequest)
			{
				Quit();
			}
		}

	}
}
