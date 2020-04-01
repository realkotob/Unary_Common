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
		private ConsoleSys ConsoleSys;

		private Dictionary<string, List<Command>> RegisteredCommands;
		private Dictionary<int, List<BoundPath>> Paths;

		private int AutofillLimit;

		private static readonly string CommandsPath = "/Commands.json";
		private static readonly string ScriptPath = "/Autoexec.usl";

		public override void Init()
		{
			ConsoleSys = Sys.Ref.ConsoleSys;

			RegisteredCommands = new Dictionary<string, List<Command>>();
			Paths = new Dictionary<int, List<BoundPath>>();

			AutofillLimit = Sys.Ref.Shared.GetObject<ConfigSys>().GetShared<int>("Unary_Common.Console.AutofillLimit");

			ProcessFiles("Unary_Common", ".");
		}

		public override void _Process(float delta)
		{
			if(Sys.Ref.AppType.IsClient())
			{
				foreach (var Bound in Paths)
				{
					if (Input.IsKeyPressed(Bound.Key))
					{
						foreach (var Path in Bound.Value)
						{
							ProcessScriptFile(Path.Path);
						}
					}
				}
			}
		}

		public void Help()
		{
			string Result = "ListModID() \n" +
					"ListSystemTypes() \n" +
					"ListSystems(%Type%)";
			ConsoleSys.Message(Result);
		}

		public void ListModID()
		{
			ConsoleSys.Message(GetModIDs());
		}

		public void ListSystemTypes()
		{
			ConsoleSys.Message(GetSystemTypes());
		}

		public void ListSystems(string Type)
		{
			List<string> Systems;

			Type = Type.ToLower();

			if (Type == "shared")
			{
				Systems = Sys.Ref.Shared.Order;
			}
			else if (Type == "server")
			{
				Systems = Sys.Ref.Server.Order;
			}
			else if (Type == "client")
			{
				Systems = Sys.Ref.Client.Order;
			}
			else
			{
				ConsoleSys.Error("Invalid system type");
				ConsoleSys.Message(GetSystemTypes());
				return;
			}

			ConsoleSys.Message(GetSystems(char.ToUpper(Type[0]) + Type.Substring(1), Systems));
		}

		public void BindFile(string ModID, string Key, string FilePath)
		{
			if (Sys.Ref.AppType.IsClient())
			{
				BoundPath NewBoundPath = new BoundPath()
				{
					ModID = ModID,
					Path = FilePath
				};

				int Scancode = EnumUtil.GetKeyFromString<int, KeyList>(Key);

				if(Paths.ContainsKey(Scancode))
				{
					if(Paths[Scancode].Contains(NewBoundPath))
					{
						ConsoleSys.Error("Tried binding file " + FilePath + " to the same file from the same namespace");
					}
					else
					{
						Paths[Scancode].Add(NewBoundPath);
					}
				}
				else
				{
					Paths[Scancode] = new List<BoundPath>
					{
						NewBoundPath
					};
				}
			}
		}

		public override void Clear()
		{
			RegisteredCommands.Clear();
			Paths.Clear();
		}

		public override void ClearMod(Mod Mod)
		{
			if(RegisteredCommands.ContainsKey(Mod.ModID))
			{
				RegisteredCommands.Remove(Mod.ModID);
			}

			foreach(var Key in Paths)
			{
				for(int i = Key.Value.Count - 1; i >= 0; --i)
				{
					if(Key.Value[i].ModID == Mod.ModID)
					{
						Key.Value.RemoveAt(i);
					}
				}
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

			foreach(var ModID in RegisteredCommands)
			{
				Result += ModID.Key + ", ";
			}

			Result = Result.Substring(0, Result.Length - 2);

			return Result;
		}

		public void ProcessCommandFile(string Path, string ModID)
		{
			try
			{
				if(RegisteredCommands.ContainsKey(ModID))
				{
					return;
				}
				else
				{
					RegisteredCommands[ModID] = new List<Command>();
				}

				List<Command> Commands = JsonConvert.DeserializeObject<List<Command>>(FilesystemUtil.SystemFileRead(Path));

				foreach (var CommandEntry in Commands)
				{
					bool Duped = false;

					foreach (var RegisteredModID in RegisteredCommands)
					{
						foreach (var RegisteredCommand in RegisteredModID.Value)
						{
							if (CommandEntry.Alias == RegisteredCommand.Alias)
							{
								ConsoleSys.Error(ModID + " tried declaring already taken command alias " + CommandEntry.Alias);
								Duped = true;
							}
						}
					}

					if(!Duped)
					{
						RegisteredCommands[ModID].Add(CommandEntry);
					}
				}

				
			}
			catch(Exception)
			{
				return;
			}
		}

		private string ProcessCommandBinding(string Command)
		{
			foreach (var TargetModID in RegisteredCommands)
			{
				foreach (var BoundCommand in TargetModID.Value)
				{
					if (Command == BoundCommand.Alias)
					{
						return BoundCommand.Target;
					}
				}
			}

			return null;
		}

		public void ProcessScript(string Command)
		{
			List<string> CommandParts;
			string Arguments = null;

			if (Command.Contains('(') && Command.Contains(')') && Command[Command.Length - 1] == ')')
			{
				string TargetCommand = default;

				for (int i = 0; i < Command.Length - 1; ++i)
				{
					if (Command[i] == '(')
					{
						TargetCommand = Command.Substring(0, i);

						if (i + 1 != Command.Length - 1)
						{
							Arguments = Command.Substring(i + 1, Command.Length - ( i + 2 ));
						}

						break;
					}
				}

				// Check for command bind
				string ResultCommand = ProcessCommandBinding(TargetCommand);

				if(ResultCommand != null)
				{
					TargetCommand = ResultCommand;
				}

				CommandParts = TargetCommand.Split('.').ToList();

				if (CommandParts.Count != 4)
				{
					ConsoleSys.Error("Invalid command syntax");
					ConsoleSys.Error("Have you tried using \"Help()\"?");
					return;
				}

				if (TargetCommand != "Unary_Common.Shared.ConsoleSys.Message" &&
					TargetCommand != "Unary_Common.Shared.ConsoleSys.Warning" &&
					TargetCommand != "Unary_Common.Shared.ConsoleSys.Error" &&
					TargetCommand != "Unary_Common.Shared.ConsoleSys.Panic")
				{
					ConsoleSys.Message("> " + Command);
				}
			}
			else
			{
				ConsoleSys.Error("Invalid command syntax");
				ConsoleSys.Error("Have you tried using \"Help()\"?");
				return;
			}

			string ModID;

			if (!RegisteredCommands.ContainsKey(CommandParts[0]))
			{
				ConsoleSys.Error(CommandParts[0] + " is an invalid ModID");
				ConsoleSys.Error(GetModIDs());
				return;
			}
			else
			{
				ModID = CommandParts[0];
			}

			List<string> Systems;

			if (CommandParts[1] == "Shared")
			{
				Systems = Sys.Ref.Shared.Order;
			}
			else if (CommandParts[1] == "Client")
			{
				Systems = Sys.Ref.Client.Order;
			}
			else if (CommandParts[1] == "Server")
			{
				Systems = Sys.Ref.Server.Order;
			}
			else
			{
				ConsoleSys.Error(CommandParts[1] + " is an invalid System type");
				ConsoleSys.Error(GetSystemTypes());
				return;
			}

			string ModIDEntry;
			SysType Type;

			if (Systems.Contains(CommandParts[0] + '.' + CommandParts[1] + '.' + CommandParts[2]))
			{
				ModIDEntry = CommandParts[0] + '.' + CommandParts[1] + '.' + CommandParts[2];
			}
			else
			{
				ConsoleSys.Error(CommandParts[2] + " is an invalid System ModIDEntry");
				ConsoleSys.Error(GetSystems(CommandParts[1], Systems));
				return;
			}

			Godot.Object TargetSystem = default;
			SysManager TargetManager;

			if (CommandParts[1] == "Shared")
			{
				TargetManager = Sys.Ref.Shared;
			}
			else if (CommandParts[1] == "Client")
			{
				TargetManager = Sys.Ref.Client;
			}
			else
			{
				TargetManager = Sys.Ref.Server;
			}

			Type = TargetManager.GetType(ModIDEntry);

			switch (Type)
			{
				case SysType.Object:
					TargetSystem = TargetManager.GetObject(ModIDEntry);
					break;
				case SysType.Node:
					TargetSystem = TargetManager.GetNode(ModIDEntry);
					break;
				case SysType.UI:
					TargetSystem = TargetManager.GetUI(ModIDEntry);
					break;
			}

			Type TargetType = TargetSystem.GetType();

			MethodInfo[] Methods = TargetType.GetMethods().Where(m => m.DeclaringType == TargetType).ToArray();

			MethodInfo TargetMethod = null;

			foreach (var Method in Methods)
			{
				if (Method.Name == CommandParts[3])
				{
					TargetMethod = Method;
					break;
				}
			}

			if (TargetMethod == null)
			{
				ConsoleSys.Error(CommandParts[3] + " is an invalid method");
				ConsoleSys.Error(GetMethods(Methods));
				return;
			}

			ParameterInfo[] ArgumentsInfo = TargetMethod.GetParameters();
			string ArgumentValues = '[' + Arguments + ']';
			JArray Array;

			try
			{
				Array = JArray.Parse(ArgumentValues);
			}
			catch (Exception)
			{
				ConsoleSys.Error(Arguments + " is an invalid set of arguments");
				ConsoleSys.Error(GetArguments(ArgumentsInfo));
				return;
			}

			if (Array.Count != ArgumentsInfo.Length)
			{
				ConsoleSys.Error("Invalid ammount of arguments");
				ConsoleSys.Error(GetArguments(ArgumentsInfo));
				return;
			}

			List<object> FinalArguments = new List<object>();

			for (int i = 0; i < Array.Count; ++i)
			{
				try
				{
					object Argument = JsonConvert.DeserializeObject(Array[i].ToString(Formatting.None), ArgumentsInfo[i].ParameterType);
					FinalArguments.Add(Argument);
				}
				catch (Exception)
				{
					ConsoleSys.Error(Array[i].ToString() + " is not matching an argument type of " + ArgumentsInfo[i].ParameterType);
					ConsoleSys.Error(GetArguments(ArgumentsInfo));
					return;
				}
			}

			object CallResult = TargetSystem.Call(CommandParts[3], FinalArguments.ToArray());

			if (CallResult != null)
			{
				ConsoleSys.Message(CallResult.ToString());
			}
		}

		public void ProcessScriptFile(string Path)
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
					ProcessScript(Line);
				}
			}
		}

		public List<Command> AutofillCommand(string Command)
		{
			List<Command> Result = new List<Command>();

			if(!string.IsNullOrEmpty(Command) || string.IsNullOrWhiteSpace(Command))
			{
				foreach(var ModID in RegisteredCommands)
				{
					foreach(var RegistredCommand in ModID.Value)
					{
						if(RegistredCommand.Alias.BeginsWith(Command) || RegistredCommand.Alias == Command)
						{
							if(Result.Count < AutofillLimit)
							{
								Result.Add(RegistredCommand);
							}
							else
							{
								return Result;
							}
						}
					}
				}
			}

			return Result;
		}

		public void ProcessFiles(string ModID, string Path)
		{
			if (FilesystemUtil.SystemFileExists(Path + CommandsPath))
			{
				ProcessCommandFile(Path + CommandsPath, ModID);
			}

			if (FilesystemUtil.SystemFileExists(Path + ScriptPath))
			{
				ProcessScriptFile(Path + ScriptPath);
			}
		}

		public override void InitCore(Mod Mod)
		{
			ProcessFiles(Mod.ModID, Mod.Path);
		}

		public override void InitMod(Mod Mod)
		{
			ProcessFiles(Mod.ModID, Mod.Path);
		}
	}
}
