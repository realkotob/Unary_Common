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

using Unary_Common.Abstract;
using Unary_Common.Utils;
using Unary_Common.Interfaces;
using Unary_Common.Structs;

namespace Unary_Common.Shared
{
	public class ConsoleSys : SysUI
	{
		private enum ConsoleColors
		{
			Message,
			Warning,
			Error
		};

		public int ConsoleMaxLines { get; set; } = 36;

		private PackedScene ConsoleEntry;
		private PackedScene AutofillEntry;

		private WindowDialog Window;
		private LineEdit ConsoleLine;
		
		private VBoxContainer ConsoleHistory;
		private VScrollBar ScrollBar;

		private VBoxContainer Autofill;
		
		private bool ConsoleVisible = false;

		public override void Init()
		{
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

		public override void ClearedMods()
		{
			Message("Cleared mods!");
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
					Shared.Sys.Ref.Shared.GetNode<Shared.InterpreterSys>().ProcessScript(ConsoleLine.Text);
					ConsoleLine.Clear();
				}
			}
		}

		public void OnTextChanged(string NewCommand)
		{
			if(string.IsNullOrEmpty(NewCommand) || string.IsNullOrWhiteSpace(NewCommand))
			{
				return;
			}

			int Count = Autofill.GetChildCount();

			for (int i = 0; i < Count; ++i)
			{
				Autofill.GetChild(i).QueueFree();
			}

			List<Command> Commands = Sys.Ref.Shared.GetNode<InterpreterSys>().AutofillCommand(NewCommand);

			foreach(var Command in Commands)
			{
				Node NewEntry = AutofillEntry.Instance();

				Label Alias = NewEntry.GetNode<Label>("Alias");

				System.Drawing.Color AliasColor = System.Drawing.Color.FromName(Command.AliasColor);

				Alias.AddColorOverride("font_color", new Color()
				{
					r8 = AliasColor.R,
					g8 = AliasColor.G,
					b8 = AliasColor.B,
					a8 = AliasColor.A
				});

				Alias.Text = Command.Alias;

				if(Command.Arguments == null)
				{
					Alias.Text += "()";
				}
				else
				{
					Alias.Text += '(' + Command.Arguments + ')';
				}

				Label Description = NewEntry.GetNode<Label>("Description");

				System.Drawing.Color DescriptionColor = System.Drawing.Color.FromName(Command.DescriptionColor);

				Description.AddColorOverride("font_color", new Color()
				{
					r8 = DescriptionColor.R,
					g8 = DescriptionColor.G,
					b8 = DescriptionColor.B,
					a8 = DescriptionColor.A
				});

				Description.Text = Command.Description;

				Autofill.AddChild(NewEntry);
			}
		}

		private void AddEntry(string Text, ConsoleColors TextColor)
		{
			if (ConsoleHistory.GetChildCount() >= ConsoleMaxLines)
			{
				ConsoleHistory.GetChild(0).QueueFree();
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

			CallDeferred(nameof(SetScrollbar));
		}

		public void SetScrollbar()
		{
			ScrollBar.Value = ScrollBar.MaxValue;
		}

		private void WriteToLog(string Type, string Text)
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
			Sys.Ref.Quit();
		}
		
		public override void _Ready()
		{
			ConsoleEntry = GD.Load<PackedScene>("Unary_Common/Nodes/ConsoleEntry.tscn");
			AutofillEntry = GD.Load<PackedScene>("Unary_Common/Nodes/AutofillEntry.tscn");

			Window = GetNode<WindowDialog>("Window");

			ConsoleLine = GetNode<LineEdit>("Window/Container/ConsoleLine");
			ConsoleLine.Connect("text_changed", this, nameof(OnTextChanged));

			ConsoleHistory = GetNode<VBoxContainer>("Window/Container/ScrollContainer/ConsoleHistory");

			ScrollBar = GetNode<ScrollContainer>("Window/Container/ScrollContainer").GetVScrollbar();

			Autofill = GetNode<VBoxContainer>("Window/Container/Autofill");

			Message("Reinitialized console!");
		}
	}

}
