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
using Unary_Common.Utils;
using Unary_Common.Structs;

using Godot;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Newtonsoft.Json;

namespace Unary_Common.Shared
{
	class FilesystemSys : Godot.Object, IShared
	{
		private Dictionary<string, List<string>> ModIDFiles;
		private string CorePath;

		public void Init()
		{
			ModIDFiles = new Dictionary<string, List<string>>();

			#if DEBUG
				if (!FilesystemUtil.GodotPackPCK("Unary_Common", ".", new List<string>() { }))
				{
					Sys.Ref.GetShared<ConsoleSys>().Error("Failed to pack Unary_Common");
				}
			#endif

			if (!ProjectSettings.LoadResourcePack("Unary_Common.pck"))
			{
				Sys.Ref.GetShared<ConsoleSys>().Error("Failed to load package of Unary_Common");
				return;
			}
		}

		public void Clear()
		{
			ModIDFiles.Clear();
		}

		public void ClearMod(Mod Mod)
		{
			if (ModIDFiles.ContainsKey(Mod.ModID))
			{
				foreach (var File in ModIDFiles[Mod.ModID])
				{
					FilesystemUtil.GodotFileRemove(File);
				}
			}

			FilesystemUtil.GodotDirRemove("res://" + Mod.ModID);
			ModIDFiles.Remove(Mod.ModID);
		}

		public void ClearedMods()
		{
			ProjectSettings.LoadResourcePack("Unary_Common.pck");
			ProjectSettings.LoadResourcePack(CorePath);
		}

		public void InitCore(Mod Mod)
		{
			if(ModIDFiles.ContainsKey(Mod.ModID))
			{
				return;
			}

			ModSys ModSys = Sys.Ref.GetShared<ModSys>();

			string PCKPath = Mod.Path + '/' + Mod.ModID + ".pck";
			string PCKManifest = Mod.Path + '/' + Mod.ModID + ".json";

			#if DEBUG
				if (!FilesystemUtil.GodotPackPCK(Mod.ModID, Mod.Path, new List<string>() { }))
				{
					Sys.Ref.GetShared<ConsoleSys>().Error("Failed to repack " + Mod.ModID);
					return;
				}
			#endif

			if (!FilesystemUtil.SystemFileExists(PCKPath))
			{
				Sys.Ref.GetShared<ConsoleSys>().Error(Mod.ModID + " is not providing a package to load from");
				return;
			}

			if (!FilesystemUtil.SystemFileExists(PCKManifest))
			{
				Sys.Ref.GetShared<ConsoleSys>().Error(Mod.ModID + " is not providing a package manifest to load from");
				return;
			}

			string Manifest = FilesystemUtil.SystemFileRead(PCKManifest);

			if (Manifest == null)
			{
				Sys.Ref.GetShared<ConsoleSys>().Error(Mod.ModID + " is providing a package manifest but it is empty");
				return;
			}

			try
			{
				List<string> Files = JsonConvert.DeserializeObject<List<string>>(Manifest);
				ModIDFiles[Mod.ModID] = Files;
				CorePath = Mod.Path;
			}
			catch (Exception)
			{
				return;
			}

			if (!ProjectSettings.LoadResourcePack(PCKPath))
			{
				Sys.Ref.GetShared<ConsoleSys>().Error("Failed to load package of " + Mod.ModID);
				return;
			}
		}

		public void InitMod(Mod Mod)
		{
			if (ModIDFiles.ContainsKey(Mod.ModID))
			{
				return;
			}

			ModSys ModSys = Sys.Ref.GetShared<ModSys>();

			if (!FilesystemUtil.GodotDirContainsFiles(Mod.Path, Mod.ModID + ".pck"))
			{
				return;
			}

			#if DEBUG
				List<string> TargetFolders = new List<string>();
				TargetFolders.Add(Mod.ModID);

				if (ModSys.GetManifest(Mod).Overrides != null)
				{
					TargetFolders.AddRange(ModSys.GetManifest(Mod).Overrides);
				}

				TargetFolders = TargetFolders.Distinct().ToList();

				FilesystemUtil.GodotPackPCK(Mod.ModID, Mod.Path, TargetFolders);
			#endif

			string Manifest = FilesystemUtil.SystemFileRead(Mod.Path + '/' + Mod.ModID + ".json");

			if (Manifest == null)
			{
				return;
			}

			try
			{
				List<string> Files = JsonConvert.DeserializeObject<List<string>>(Manifest);
				ModIDFiles[Mod.ModID] = Files;
			}
			catch (Exception)
			{
				return;
			}

			if (!ProjectSettings.LoadResourcePack(Mod.Path + '/' + Mod.ModID + ".pck"))
			{
				return;
			}
		}
	}
}
