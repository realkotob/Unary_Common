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

using Godot;

using System;
using System.IO;
using System.Collections.Generic;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Unary_Common.Shared
{
	public class ConfigSys : SysObject
	{
		private Dictionary<string, Config> SharedConfigs;
		private Dictionary<string, Config> ClientConfigs;
		private Dictionary<string, Config> ServerConfigs;

		public override void Init()
		{
			SharedConfigs = new Dictionary<string, Config>();
			ClientConfigs = new Dictionary<string, Config>();
			ServerConfigs = new Dictionary<string, Config>();

			LoadConfig("Unary_Common", ".");
		}

		public override void Clear()
		{
			SharedConfigs.Clear();
			ClientConfigs.Clear();
			ServerConfigs.Clear();
		}

		public override void ClearMod(Mod Mod)
		{
			if(ServerConfigs.ContainsKey(Mod.ModID))
			{
				ServerConfigs[Mod.ModID].Save();
				ServerConfigs.Remove(Mod.ModID);
			}

			if (ClientConfigs.ContainsKey(Mod.ModID))
			{
				ClientConfigs[Mod.ModID].Save();
				ClientConfigs.Remove(Mod.ModID);
			}

			if (SharedConfigs.ContainsKey(Mod.ModID))
			{
				SharedConfigs[Mod.ModID].Save();
				SharedConfigs.Remove(Mod.ModID);
			}
		}

		private void LoadConfig(string ModID, string Path)
		{
			string SharedPath = Path + '/' + "Shared.json";
			string ClientPath = Path + '/' + "Client.json";
			string ServerPath = Path + '/' + "Server.json";

			if (FilesystemUtil.SystemFileExists(SharedPath))
			{
				if (!SharedConfigs.ContainsKey(ModID))
				{
					Config NewConfig = new Config();
					NewConfig.Init(SharedPath);
					SharedConfigs[ModID] = NewConfig;
				}
			}

			if (FilesystemUtil.SystemFileExists(ClientPath))
			{
				if (!ClientConfigs.ContainsKey(ModID))
				{
					Config NewConfig = new Config();
					NewConfig.Init(ClientPath);
					ClientConfigs[ModID] = NewConfig;
				}
			}

			if (FilesystemUtil.SystemFileExists(ServerPath))
			{
				if (!ServerConfigs.ContainsKey(ModID))
				{
					Config NewConfig = new Config();
					NewConfig.Init(ServerPath);
					ServerConfigs[ModID] = NewConfig;
				}
			}
		}

		public void SubscribeShared(Godot.Object Target, string MemberName, string ConfigVariable, SubscriberType Type)
		{
			if (!ModIDUtil.Validate(ConfigVariable))
			{
				return;
			}

			string NewModID = ModIDUtil.ModID(ConfigVariable);

			if (SharedConfigs.ContainsKey(NewModID))
			{
				SharedConfigs[NewModID].Subscribe(Target, MemberName, ConfigVariable, Type);
			}
		}

		public void SubscribeClient(Godot.Object Target, string MemberName, string ConfigVariable, SubscriberType Type)
		{
			if (!ModIDUtil.Validate(ConfigVariable))
			{
				return;
			}

			string NewModID = ModIDUtil.ModID(ConfigVariable);

			if (ClientConfigs.ContainsKey(NewModID))
			{
				ClientConfigs[NewModID].Subscribe(Target, MemberName, ConfigVariable, Type);
			}
		}

		public void SubscribeServer(Godot.Object Target, string MemberName, string ConfigVariable, SubscriberType Type)
		{
			if (!ModIDUtil.Validate(ConfigVariable))
			{
				return;
			}

			string NewModID = ModIDUtil.ModID(ConfigVariable);

			if (ServerConfigs.ContainsKey(NewModID))
			{
				ServerConfigs[NewModID].Subscribe(Target, MemberName, ConfigVariable, Type);
			}
		}

		public T GetShared<T>(string ConfigVariable)
		{
			if (!ModIDUtil.Validate(ConfigVariable))
			{
				Sys.Ref.ConsoleSys.Error("Failed to validate " + ConfigVariable);
				return default;
			}

			string NewModID = ModIDUtil.ModID(ConfigVariable);

			if (SharedConfigs.ContainsKey(NewModID))
			{
				return (T)SharedConfigs[NewModID].Get(ConfigVariable);
			}
			else
			{
				Sys.Ref.ConsoleSys.Error("Tried to access non-existing config variable " + ConfigVariable);
				return default;
			}
		}

		public T GetClient<T>(string ConfigVariable)
		{
			if (ModIDUtil.Validate(ConfigVariable))
			{
				Sys.Ref.ConsoleSys.Error("Failed to validate " + ConfigVariable);
				return default;
			}

			string NewModID = ModIDUtil.ModID(ConfigVariable);

			if (ClientConfigs.ContainsKey(NewModID))
			{
				return (T)ClientConfigs[NewModID].Get(ConfigVariable);
			}
			else
			{
				Sys.Ref.ConsoleSys.Error("Tried to access non-existing config variable " + ConfigVariable);
				return default;
			}
		}

		public T GetServer<T>(string ConfigVariable)
		{
			if (!ModIDUtil.Validate(ConfigVariable))
			{
				Sys.Ref.ConsoleSys.Error("Failed to validate " + ConfigVariable);
				return default;
			}

			string NewModID = ModIDUtil.ModID(ConfigVariable);

			if (ServerConfigs.ContainsKey(NewModID))
			{
				return (T)ServerConfigs[NewModID].Get(ConfigVariable);
			}
			else
			{
				Sys.Ref.ConsoleSys.Error("Tried to access non-existing config variable " + ConfigVariable);
				return default;
			}
		}

		public void SetShared(string ConfigVariable, object Value)
		{
			if (!ModIDUtil.Validate(ConfigVariable))
			{
				Sys.Ref.ConsoleSys.Error("Failed to validate " + ConfigVariable);
				return;
			}

			string NewModID = ModIDUtil.ModID(ConfigVariable);

			if (SharedConfigs.ContainsKey(NewModID))
			{
				SharedConfigs[NewModID].Set(ConfigVariable, Value);
			}
			else
			{
				Sys.Ref.ConsoleSys.Error("Tried to access non-existing config variable " + ConfigVariable);
				return;
			}
		}

		public void SetClient(string ConfigVariable, object Value)
		{
			if (!ModIDUtil.Validate(ConfigVariable))
			{
				Sys.Ref.ConsoleSys.Error("Failed to validate " + ConfigVariable);
				return;
			}

			string NewModID = ModIDUtil.ModID(ConfigVariable);

			if (ClientConfigs.ContainsKey(NewModID))
			{
				ClientConfigs[NewModID].Set(ConfigVariable, Value);
			}
			else
			{
				Sys.Ref.ConsoleSys.Error("Tried to access non-existing config variable " + ConfigVariable);
				return;
			}
		}

		public void SetServer(string ConfigVariable, object Value)
		{
			if (!ModIDUtil.Validate(ConfigVariable))
			{
				Sys.Ref.ConsoleSys.Error("Failed to validate " + ConfigVariable);
				return;
			}

			string NewModID = ModIDUtil.ModID(ConfigVariable);

			if (ServerConfigs.ContainsKey(NewModID))
			{
				ServerConfigs[NewModID].Set(ConfigVariable, Value);
			}
			else
			{
				Sys.Ref.ConsoleSys.Error("Tried to access non-existing config variable " + ConfigVariable);
				return;
			}
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
			foreach (var Config in ServerConfigs)
			{
				Config.Value.Load();
			}

			foreach (var Config in ClientConfigs)
			{
				Config.Value.Load();
			}

			foreach (var Config in SharedConfigs)
			{
				Config.Value.Load();
			}
		}
	}
}
