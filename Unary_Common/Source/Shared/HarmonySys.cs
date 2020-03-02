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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Harmony;
using System.Reflection;

namespace Unary_Common.Shared
{
	public class HarmonySys : Godot.Object, IShared
	{
		private AssemblySys AssemblySys;
		private Dictionary<string, HarmonyInstance> Instances;

		public void Init()
		{
			AssemblySys = Sys.Ref.GetShared<AssemblySys>();
			Instances = new Dictionary<string, HarmonyInstance>();
		}

		public void Clear()
		{
			Instances.Clear();
		}

		public void ClearMod(string ModID)
		{
			if (Instances.ContainsKey(ModID))
			{
				Instances[ModID].UnpatchAll();
				Instances.Remove(ModID);
			}
		}

		public void ClearedMods()
		{

		}

		public void InitCore(string ModID, string Path)
		{
			if(!Instances.ContainsKey(ModID))
			{
				Assembly Target = AssemblySys.GetAssembly(ModID);

				if(Target != null)
				{
					HarmonyInstance NewInstance = HarmonyInstance.Create(ModID);
					NewInstance.PatchAll(Target);
					Instances[ModID] = NewInstance;
				}
			}
			else
			{
				Sys.Ref.GetSharedNode<ConsoleSys>().Error(ModID + " is already presented for initialization");
			}
		}

		public void InitMod(string ModID, string Path)
		{
			if (!Instances.ContainsKey(ModID))
			{
				Assembly Target = AssemblySys.GetAssembly(ModID);

				if (Target != null)
				{
					HarmonyInstance NewInstance = HarmonyInstance.Create(ModID);
					NewInstance.PatchAll(Target);
					Instances[ModID] = NewInstance;
				}
			}
		}
	}
}
