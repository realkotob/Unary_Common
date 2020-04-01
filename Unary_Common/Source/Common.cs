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

using Godot;
using Unary_Common.UI;

using Unary_Common.Shared;
using Unary_Common.Client;

namespace Unary_Common
{
	public class Common : IBoot
	{
		private Sys Sys;
		
		public void AddShared()
		{
			Sys = Sys.Ref;

			// Adding shared systems
			Sys.Shared.Add(new Shared.RegistrySys());
			Sys.Shared.Add(new EventSys());
			Sys.Shared.Add(new AssemblySys());
			Sys.Shared.Add(new ConfigSys());
			Sys.Shared.Add(new Shared.SteamSys());
			Sys.Shared.Add(new FilesystemSys());
			Sys.Shared.Add(new EntriesSys());
			Sys.Shared.Add(new InterpreterSys());
			Sys.Shared.Add(new HarmonySys());
			Sys.Shared.Add(new OSSys());
			Sys.Shared.Add(new RandomSys());
			Sys.Shared.Add(new DataSys());
			Sys.Shared.Add(new SaveSys());
			//Sys.Shared.Add(new ProfilerManager());
		}

		public void AddClient()
		{
			// Adding client systems

			Sys.Client.Add(new Client.DiscordSys());
			Sys.Client.Add(new Client.LocaleSys());
			Sys.Client.Add(new Client.BindSys());
			Sys.Client.Add(new Client.SteamSys());
			Sys.Client.Add(new MusicSys());
			Sys.Client.Add(new AlertSys(), "Alert");

			Sys.Client.Add(new Client.NetworkSys());
			Sys.Client.Add(new Client.RegistrySys());
			
			Sys.Client.Add(new SceneSys());
		}

		public void AddServer()
		{

			Sys.Server.Add(new Server.SteamSys());
			Sys.Server.Add(new Server.NetworkSys());
			Sys.Server.Add(new Server.RegistrySys());

			Sys.Server.Add(new SceneSys());
		}
	}
}
