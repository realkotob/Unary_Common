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
using Unary_Common.UI;
using Unary_Common.Utils;
using Unary_Common.Structs;
using Unary_Common.Server;
using Unary_Common.Enums;

using System;
using System.Collections.Generic;
using System.Linq;

using Godot;

namespace Unary_Common.Shared
{
	public class Sys : Node
	{
		public static Sys Ref { get; private set; }

		public ConsoleSys ConsoleSys { get; private set; }

		public SysManager Shared { get; private set; }
		public SysManager Server { get; private set; }
		public SysManager Client { get; private set; }

		public SysAppType AppType { get; private set; }

		public List<string> LaunchArguments { get; private set; }

		public override void _Ready()
		{
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

			Shared.AddUI(new ConsoleSys(), "Console");

			ConsoleSys = Shared.GetUI<ConsoleSys>();

			Common NewCommon = new Common();
			NewCommon.AddShared();

			Shared.AddObject(new BootSys());

			Shared.GetObject<BootSys>().Add("Unary_Common", NewCommon);

			if(AppType.IsServer())
			{
				Shared.GetObject<BootSys>().AddServer("Unary_Common");
			}
			else
			{
				Shared.GetObject<BootSys>().AddClient("Unary_Common");
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

		public void Quit()
		{
			Server.ClearMods();
			Client.ClearMods();
			Shared.ClearMods();

			Server.Clear();
			Client.Clear();
			Shared.Clear();

			ConsoleSys = null;

			Ref = null;
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
