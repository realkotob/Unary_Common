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
			Sys.AddShared<AssemblySys>();
			Sys.AddShared<ConfigSys>();
			Sys.AddSharedNode<Shared.SteamSys>();
			Sys.AddSharedNode<DiscordSys>();
			Sys.AddShared<FilesystemSys>();
			Sys.AddShared<LocaleSys>();
			Sys.AddShared<EntriesSys>();
			Sys.AddSharedNode<InterpreterSys>();
			Sys.AddSharedNode<BindSys>();
			Sys.AddShared<HarmonySys>();
			Sys.AddSharedNode<OSSys>();
			Sys.AddSharedNode<EventSys>();
			Sys.AddSharedNode<RandomSys>();
			Sys.AddShared<TempSys>();
			//Sys.AddShared<DownloadsSys>();
			Sys.AddShared<SaveSys>();
			//Sys.(new ProfilerManager());

			Sys.AddSharedNode<SceneSys>();
		}

		public void AddClient()
		{
			// Adding client systems
			Sys.AddClient<Client.SteamSys>();
			Sys.AddClientNode<MusicSys>();
			Sys.AddClientNodeLoad<AlertSys>("Alert");
			Sys.AddClientNode<Client.NetworkSys>();
		}

		public void AddServer()
		{
			Sys.AddServer<Server.SteamSys>();
			Sys.AddServerNode<Server.NetworkSys>();
		}
	}
}
