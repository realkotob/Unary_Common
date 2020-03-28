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

using Unary_Common.Structs;
using Unary_Common.Interfaces;
using Unary_Common.Utils;
using Unary_Common.Abstract;

using System.Collections.Generic;

using Godot;

using Newtonsoft.Json;

namespace Unary_Common.Shared
{
	public class ModSys : SysObject
	{
		public ModManifest Core { get; private set; }

		public Dictionary<Mod, ModManifest> ExistingMods { get; set; }
		public List<Mod> LoadOrder { get; set; }

		private IConsoleSys ConsoleSys;
		private SteamSys SteamSys;

		public override void Init()
		{
			ExistingMods = new Dictionary<Mod, ModManifest>();
			LoadOrder = new List<Mod>();

			ConsoleSys = Sys.Ref.ConsoleSys;
			SteamSys = Sys.Ref.Shared.GetNode<SteamSys>();

			List<string> ModFolders = new List<string>();
			
			ModFolders.AddRange(SteamSys.GetModFolders());

			ModFolders.AddRange(FilesystemUtil.SystemDirGetDirsTop("Mods"));

			foreach (var Folder in ModFolders)
			{
				ParseMod(Folder.Replace('\\', '/'));
			}

			if (Core.Mod.ModID == null)
			{
				ConsoleSys.Panic("We could not find any mod assigned as a Core. Quitting...");
				return;
			}
		}

		private void ParseMod(string Path)
		{
			ModManifest NewMod;

			try
			{
				string Manifest = FilesystemUtil.SystemFileRead(Path + '/' + "Manifest.json");
				NewMod = JsonConvert.DeserializeObject<ModManifest>(Manifest);
				NewMod.Path = Path;
			}
			catch (System.Exception)
			{
				ConsoleSys.Error("Failed to parse " + Path + '/' + "Manifest.json");
				return;
			}

			if (NewMod.Mod.ModID == null)
			{
				ConsoleSys.Error("Failed to parse " + Path + '/' + "Manifest.json");
				return;
			}

			if (ExistingMods.ContainsKey(NewMod.Mod))
			{
				ConsoleSys.Error("Duplicate mod " + NewMod.Name);
				return;
			}

			if(NewMod.Core)
			{
				if(Core.Mod.ModID == null)
				{
					Core = NewMod;
				}
				else
				{
					ConsoleSys.Error("Duplicated Core ModID of " + Core.Name + " and " + NewMod.Name);
				}
			}
			else
			{
				ExistingMods[NewMod.Mod] = NewMod;
			}
		}

		public override void Clear()
		{
			ExistingMods.Clear();
			LoadOrder.Clear();
		}

		public override void ClearMod(Mod Mod)
		{
			for(int i = LoadOrder.Count - 1; i >= 0; --i)
			{
				if(LoadOrder[i] == Mod)
				{
					LoadOrder.RemoveAt(i);
				}
			}
		}

		public ModManifest GetManifest(Mod Mod)
		{
			if(ExistingMods.ContainsKey(Mod))
			{
				return ExistingMods[Mod];
			}
			else
			{
				return default;
			}
		}
	}
}
