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

using Godot;

using System;
using System.IO;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;

using Unary_Common.Utils;
using Unary_Common.Interfaces;

namespace Unary_Common.Shared
{
	public class ConsoleSys : CanvasLayer, IShared
	{
		private enum ConsoleColors
		{
			Message,
			Warning,
			Error
		};

		public int ConsoleMaxLines { get; set; } = 36;

		private WindowDialog Window;
		private LineEdit ConsoleLine;
		private PackedScene ConsoleEntry;
		private VBoxContainer ConsoleHistory;
		private VBoxContainer AutoContainer;
		private VScrollBar ScrollBar;

		private Queue<Label> ConsoleHistoryContainer;

		private bool ConsoleVisible = false;

		public void Init()
		{
			ConsoleHistoryContainer = new Queue<Label>();

			if (FilesystemUtil.SystemFileExists("log.old"))
			{
				FilesystemUtil.SystemFileDelete("log.old");
			}

			if (FilesystemUtil.SystemFileExists("log.txt"))
			{
				FilesystemUtil.SystemFileMove("log.txt", "log.old");
			}

			if (!FilesystemUtil.SystemFileExists("log.txt"))
			{
				FilesystemUtil.SystemFileCreate("log.txt");
			}
		}

		public void Clear()
		{
			int Count = ConsoleHistoryContainer.Count;

			for (int i = 0; i < Count; ++i)
			{
				ConsoleHistoryContainer.Dequeue().QueueFree();
			}
		}

		public void ClearMod(string ModID)
		{
			
		}

		public void ClearedMods()
		{
			Clear();
		}

		public override void _Input(InputEvent @event)
		{
			if (@event.IsActionReleased("ui_console"))
			{
				if (ConsoleVisible)
				{
					Window.Hide();
					ConsoleVisible = false;
				}
				else
				{
					Window.Popup_();
					ConsoleLine.CallDeferred("grab_focus");
					ConsoleVisible = true;
				}
			}

			if (@event.IsActionPressed("ui_accept"))
			{
				if (ConsoleVisible)
				{
					Sys.Ref.GetSharedNode<InterpreterSys>().ProcessCommand(ConsoleLine.Text);
					ConsoleLine.Clear();
				}
			}
		}

		private void AddEntry(string Text, ConsoleColors TextColor)
		{
			if (ConsoleHistoryContainer.Count >= ConsoleMaxLines)
			{
				ConsoleHistoryContainer.Dequeue().QueueFree();
			}

			Node ConsoleEntryNode = ConsoleEntry.Instance();
			Label ConsoleEntryLabel = (Label)ConsoleEntryNode;

			switch (TextColor)
			{
				default:
				case ConsoleColors.Message:
					ConsoleEntryLabel.AddColorOverride("font_color", new Color(1.0f, 1.0f, 1.0f));
					break;
				case ConsoleColors.Warning:
					ConsoleEntryLabel.AddColorOverride("font_color", new Color(0.988f, 0.91f, 0.012f));
					break;
				case ConsoleColors.Error:
					ConsoleEntryLabel.AddColorOverride("font_color", new Color(0.988f, 0.012f, 0.012f));
					break;
			}

			ConsoleEntryLabel.Text = Text;

			ConsoleHistory.AddChild(ConsoleEntryLabel);
			ConsoleHistoryContainer.Enqueue(ConsoleEntryLabel);

			//TODO: Fix this scrolling, seems to be broken from Godot side
			ScrollBar.Value = ScrollBar.MaxValue;
		}

		public void WriteToLog(string Type, string Text)
		{
			var Time = OS.GetTime();
			string Result = "[" + Time["hour"] + ':' + Time["minute"] + ':' + Time["second"] + "][" + Type + "]: " + Text + System.Environment.NewLine;
			FilesystemUtil.SystemFileAppend("log.txt", Result);
		}

		public void Message(string Text)
		{
			AddEntry(Text, ConsoleColors.Message);
			WriteToLog("Message", Text);
			GD.Print(Text);
		}

		public void Warning(string Text)
		{
			AddEntry(Text, ConsoleColors.Warning);
			WriteToLog("Warning", Text);
			GD.PushWarning(Text);
		}

		public void Error(string Text)
		{
			AddEntry(Text, ConsoleColors.Error);
			WriteToLog("Error", Text);
			GD.PushError(Text);
		}

		public void Panic(string Text)
		{
			AddEntry(Text, ConsoleColors.Error);
			WriteToLog("Panic", Text);
			GD.PushError(Text);
			Main.Ref.Quit();
		}
		
		public void Files()
		{
			var Files = FilesystemUtil.GodotGetAllFiles();

			foreach (var File in Files)
			{
				Message("File: " + File);
			}
		}

		public void InitCore(string ModID, string Path)
		{
			
		}

		public void InitMod(string ModID, string Path)
		{
			
		}

		public override void _Ready()
		{
			ConsoleEntry = GD.Load<PackedScene>("Common/Nodes/ConsoleEntry.tscn");

			Window = GetNode<WindowDialog>("Console");

			ConsoleLine = GetNode<LineEdit>("Console/Container/ConsoleLine");

			ConsoleHistory = GetNode<VBoxContainer>("Console/Container/ScrollContainer/ConsoleHistory");
			AutoContainer = GetNode<VBoxContainer>("Console/AutoContainer");

			ScrollBar = GetNode<ScrollContainer>("Console/Container/ScrollContainer").GetVScrollbar();
			Message("Reinitialized console!");
		}

		public void ClearMods(List<string> ExcludedModIDs)
		{
			
		}
	}

}
