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
using Unary_Common.Utils;
using Unary_Common.Interfaces;
using Unary_Common.Abstract;
using Unary_Common.Enums;

using Godot;

using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Linq;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Unary_Common.Shared
{
	public class InterpreterSys : SysNode
	{
		private IConsoleSys ConsoleSys;

		private List<string> ModIDs;

		public override void Init()
		{
			ConsoleSys = Sys.Ref.ConsoleSys;

			ModIDs = new List<string>();
			ModIDs.Add("Unary_Common");

			string ScriptPath = "Autoexec.usl";

			if (FilesystemUtil.SystemFileExists(ScriptPath))
			{
				ProcessScript(ScriptPath);
			}
		}

		public override void Clear()
		{
			ModIDs.Clear();
		}

		public override void ClearMod(Mod Mod)
		{
			if(ModIDs.Contains(Mod.ModID))
			{
				ModIDs.Remove(Mod.ModID);
			}
		}

		private string GetArguments(ParameterInfo[] Parameters)
		{
			string Result = "Required arguments: \n";

			foreach (var Parameter in Parameters)
			{
				Result += ModIDUtil.FromType(Parameter.ParameterType) + ", ";
			}

			Result = Result.Substring(0, Result.Length - 2);

			return Result;
		}

		private string GetMethods(MethodInfo[] Methods)
		{
			string Result = "Available methods: \n";

			foreach (var Method in Methods)
			{
				Result += Method.Name + ", ";
			}

			Result = Result.Substring(0, Result.Length - 2);

			return Result;
		}

		private string GetSystems(string SystemType, List<string> Systems)
		{
			string Result = "Available " + SystemType + " systems: \n";

			foreach (var ModID in Systems)
			{
				Result += ModID + ", ";
			}

			Result = Result.Substring(0, Result.Length - 2);

			return Result;
		}

		private string GetSystemTypes()
		{
			return "Available Types: Shared, Server, Client";
		}

		private string GetModIDs()
		{
			string Result = "Available ModID's: \n";

			foreach(var ModID in ModIDs)
			{
				Result += ModID + ", ";
			}

			Result = Result.Substring(0, Result.Length - 2);

			return Result;
		}

		public void ProcessCommand(string Command)
		{
			if(!Command.StartsWith("Unary_Common.Shared.ConsoleSys.Message") &&
			   !Command.StartsWith("Unary_Common.Shared.ConsoleSys.Warning") &&
			   !Command.StartsWith("Unary_Common.Shared.ConsoleSys.Error") &&
			   !Command.StartsWith("Unary_Common.Shared.ConsoleSys.Panic"))
			{
				ConsoleSys.Message("> " + Command);
			}

			if (Command.ToLower() == "help")
			{
				string Result = "list modid" + '\n' +
					"list type" + '\n' +
					"list system [type]";
				ConsoleSys.Message(Result);
				return;
			}
			else if(Command.ToLower().StartsWith("list "))
			{
				string NewCommand = Command.ToLower();

				if(NewCommand == "list modid")
				{
					ConsoleSys.Message(GetModIDs());
				}
				else if(NewCommand == "list type")
				{
					ConsoleSys.Message(GetSystemTypes());
				}
				else if(NewCommand.StartsWith("list system"))
				{
					NewCommand = NewCommand.Replace("list system ", "");

					List<string> Systems;

					if(NewCommand == "shared")
					{
						Systems = Sys.Ref.Shared.Order;
					}
					else if(NewCommand == "server")
					{
						Systems = Sys.Ref.Server.Order;
					}
					else if(NewCommand == "client")
					{
						Systems = Sys.Ref.Client.Order;
					}
					else
					{
						ConsoleSys.Error("Invalid system type");
						ConsoleSys.Message(GetSystemTypes());
						return;
					}

					ConsoleSys.Message(GetSystems(char.ToUpper(NewCommand[0]) + NewCommand.Substring(1), Systems));
				}
				else
				{
					ConsoleSys.Error("Invalid list command usage");
				}

				return;
			}

			Regex Regex = new Regex(@"(.*?)\.(.*?)\.(.*?)\.(.*?)\((.*?)\)");

			MatchCollection Matches = Regex.Matches(Command);

			if(Matches.Count == 0)
			{
				ConsoleSys.Error("Unrecognized command");
				ConsoleSys.Error("Have you tried using \"help\"?");
				return;
			}

			foreach(Match Match in Matches)
			{
				GroupCollection Group = Match.Groups;

				string ModID;

				if(!ModIDs.Contains(Group[1].Value))
				{
					ConsoleSys.Error(Group[1].Value + " is an invalid ModID");
					ConsoleSys.Error(GetModIDs());
					return;
				}
				else
				{
					ModID = Group[1].Value;
				}
				
				List<string> Systems;

				if (Group[2].Value == "Shared")
				{
					Systems = Sys.Ref.Shared.Order;
				}
				else if (Group[2].Value == "Client")
				{
					Systems = Sys.Ref.Client.Order;
				}
				else if(Group[2].Value == "Server")
				{
					Systems = Sys.Ref.Server.Order;
				}
				else
				{
					ConsoleSys.Error(Group[2].Value + " is an invalid System type");
					ConsoleSys.Error(GetSystemTypes());
					return;
				}

				string ModIDEntry;
				SysType Type;

				if(Systems.Contains(Group[1].Value + '.' + Group[2].Value + '.' + Group[3].Value))
				{
					ModIDEntry = Group[1].Value + '.' + Group[2].Value + '.' + Group[3].Value;
				}
				else
				{
					ConsoleSys.Error(Group[3].Value + " is an invalid System ModIDEntry");
					ConsoleSys.Error(GetSystems(Group[2].Value, Systems));
					return;
				}

				Godot.Object TargetSystem = default;

				if (Group[2].Value == "Shared")
				{
					Type = Sys.Ref.Shared.GetType(ModIDEntry);

					switch(Type)
					{
						case SysType.Object:
							TargetSystem = Sys.Ref.Shared.GetObject(ModIDEntry);
							break;
						case SysType.Node:
							TargetSystem = Sys.Ref.Shared.GetNode(ModIDEntry);
							break;
						case SysType.UI:
							TargetSystem = Sys.Ref.Shared.GetUI(ModIDEntry);
							break;
					}
				}
				else if (Group[2].Value == "Client")
				{
					Type = Sys.Ref.Client.GetType(ModIDEntry);

					switch (Type)
					{
						case SysType.Object:
							TargetSystem = Sys.Ref.Client.GetObject(ModIDEntry);
							break;
						case SysType.Node:
							TargetSystem = Sys.Ref.Client.GetNode(ModIDEntry);
							break;
						case SysType.UI:
							TargetSystem = Sys.Ref.Client.GetUI(ModIDEntry);
							break;
					}
				}
				else if (Group[2].Value == "Server")
				{
					Type = Sys.Ref.Server.GetType(ModIDEntry);

					switch (Type)
					{
						case SysType.Object:
							TargetSystem = Sys.Ref.Server.GetObject(ModIDEntry);
							break;
						case SysType.Node:
							TargetSystem = Sys.Ref.Server.GetNode(ModIDEntry);
							break;
						case SysType.UI:
							TargetSystem = Sys.Ref.Server.GetUI(ModIDEntry);
							break;
					}
				}

				Type TargetType = TargetSystem.GetType();

				MethodInfo[] Methods = TargetType.GetMethods().Where(m => m.DeclaringType == TargetType).ToArray();

				MethodInfo TargetMethod = null;

				foreach(var Method in Methods)
				{
					if(Method.Name == Group[4].Value)
					{
						TargetMethod = Method;
						break;
					}
				}

				if(TargetMethod == null)
				{
					ConsoleSys.Error(Group[4].Value + " is an invalid method");
					ConsoleSys.Error(GetMethods(Methods));
					return;
				}

				ParameterInfo[] ArgumentsInfo = TargetMethod.GetParameters();
				string ArgumentValues = '[' + Group[5].Value + ']';
				JArray Array;

				try
				{
					Array = JArray.Parse(ArgumentValues);
				}
				catch(Exception)
				{
					ConsoleSys.Error(Group[5].Value + " is an invalid set of arguments");
					ConsoleSys.Error(GetArguments(ArgumentsInfo));
					return;
				}

				if(Array.Count != ArgumentsInfo.Length)
				{
					ConsoleSys.Error("Invalid ammount of arguments");
					ConsoleSys.Error(GetArguments(ArgumentsInfo));
					return;
				}

				List<object> FinalArguments = new List<object>();

				for(int i = 0; i < Array.Count; ++i)
				{
					try
					{
						object Argument = JsonConvert.DeserializeObject(Array[i].ToString(Formatting.None),  ArgumentsInfo[i].ParameterType);
						FinalArguments.Add(Argument);
					}
					catch(Exception)
					{
						ConsoleSys.Error(Array[i].ToString() + " is not matching an argument type of " + ArgumentsInfo[i].ParameterType);
						ConsoleSys.Error(GetArguments(ArgumentsInfo));
						return;
					}
				}

				object CallResult = TargetSystem.Call(Group[4].Value, FinalArguments.ToArray());

				if(CallResult != null)
				{
					ConsoleSys.Message(CallResult.ToString());
				}
			}
		}

		public void ProcessScript(string Path)
		{
			string File = FilesystemUtil.SystemFileRead(Path);

			string[] Lines = File.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None);

			foreach(var Line in Lines)
			{
				if(Line.StartsWith("//"))
				{
					continue;
				}
				else
				{
					ProcessCommand(Line);
				}
			}
		}

		public override void InitCore(Mod Mod)
		{
			if(!ModIDs.Contains(Mod.ModID))
			{
				ModIDs.Add(Mod.ModID);
			}

			string ScriptPath = Mod.Path + '/' + "Autoexec.usl";

			if (FilesystemUtil.SystemFileExists(ScriptPath))
			{
				ProcessScript(ScriptPath);
			}
		}

		public override void InitMod(Mod Mod)
		{
			if (!ModIDs.Contains(Mod.ModID))
			{
				ModIDs.Add(Mod.ModID);
			}

			string ScriptPath = Mod.Path + '/' + "Autoexec.usl";

			if (FilesystemUtil.SystemFileExists(ScriptPath))
			{
				ProcessScript(ScriptPath);
			}
		}
	}
}
