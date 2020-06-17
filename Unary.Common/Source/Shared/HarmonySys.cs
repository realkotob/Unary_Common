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
using Unary.Common.Structs;
using Unary.Common.Abstract;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Harmony;
using System.Reflection;

namespace Unary.Common.Shared
{
	public class HarmonySys : SysObject
	{
		private AssemblySys AssemblySys;
		private Dictionary<string, HarmonyInstance> Instances;

		public override void Init()
		{
			AssemblySys = Sys.Ref.Shared.GetNode<AssemblySys>();
			Instances = new Dictionary<string, HarmonyInstance>();
		}

		public override void Clear()
		{
			Instances.Clear();
		}

		public override void ClearMod(Mod Mod)
		{
			if (Instances.ContainsKey(Mod.ModID))
			{
				Instances[Mod.ModID].UnpatchAll();
				Instances.Remove(Mod.ModID);
			}
		}

		public override void InitCore(Mod Mod)
		{
			if(!Instances.ContainsKey(Mod.ModID))
			{
				Assembly Target = AssemblySys.GetAssembly(Mod.ModID);

				if(Target != null)
				{
					HarmonyInstance NewInstance = HarmonyInstance.Create(Mod.ModID);
					NewInstance.PatchAll(Target);
					Instances[Mod.ModID] = NewInstance;
				}
			}
			else
			{
				Sys.Ref.ConsoleSys.Error(Mod.ModID + " is already presented for initialization");
			}
		}

		public override void InitMod(Mod Mod)
		{
			if (!Instances.ContainsKey(Mod.ModID))
			{
				Assembly Target = AssemblySys.GetAssembly(Mod.ModID);

				if (Target != null)
				{
					HarmonyInstance NewInstance = HarmonyInstance.Create(Mod.ModID);
					NewInstance.PatchAll(Target);
					Instances[Mod.ModID] = NewInstance;
				}
			}
		}
	}
}
