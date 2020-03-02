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
using Unary_Common.UI;
using Unary_Common.Utils;
using Unary_Common.Structs;

using System;
using System.Collections.Generic;

using Godot;

namespace Unary_Common.Shared
{
    public class Sys : Node
    {
        public enum SysType : byte
        {
            Interface,
            Node
        }

        public static Sys Ref { get; private set; }

        private ConsoleSys ConsoleSys;

        private Dictionary<string, Godot.Object> SharedSys;
        private Dictionary<string, NodeID> SharedNodeID;
        private List<string> SharedOrder;

        public Dictionary<string, SysType> GetAllShared()
        {
            Dictionary<string, SysType> Result = new Dictionary<string, SysType>();

            foreach (var ModIDEntry in SharedOrder)
            {
                if (SharedSys.ContainsKey(ModIDEntry))
                {
                    Result[ModIDEntry] = SysType.Interface;
                }
                else if (SharedNodeID.ContainsKey(ModIDEntry))
                {
                    Result[ModIDEntry] = SysType.Node;
                }
            }

            return Result;
        }

        public Dictionary<string, SysType> GetAllShared(string ModID)
        {
            Dictionary<string, SysType> Result = new Dictionary<string, SysType>();

            foreach(var ModIDEntry in SharedOrder)
            {
                if(ModIDEntry.StartsWith(ModID + '.'))
                {
                    if (SharedSys.ContainsKey(ModIDEntry))
                    {
                        Result[ModIDEntry] = SysType.Interface;
                    }
                    else if (SharedNodeID.ContainsKey(ModIDEntry))
                    {
                        Result[ModIDEntry] = SysType.Node;
                    }
                }
            }

            return Result;
        }

        private Dictionary<string, Godot.Object> ClientSys;
        private Dictionary<string, NodeID> ClientNodeID;
        private List<string> ClientOrder;

        public Dictionary<string, SysType> GetAllClient()
        {
            Dictionary<string, SysType> Result = new Dictionary<string, SysType>();

            foreach (var ModIDEntry in ClientOrder)
            {
                if (ClientSys.ContainsKey(ModIDEntry))
                {
                    Result[ModIDEntry] = SysType.Interface;
                }
                else if (ClientNodeID.ContainsKey(ModIDEntry))
                {
                    Result[ModIDEntry] = SysType.Node;
                }
            }

            return Result;
        }

        public Dictionary<string, SysType> GetAllClient(string ModID)
        {
            Dictionary<string, SysType> Result = new Dictionary<string, SysType>();

            foreach (var ModIDEntry in ClientOrder)
            {
                if (ModIDEntry.StartsWith(ModID + '.'))
                {
                    if (ClientSys.ContainsKey(ModIDEntry))
                    {
                        Result[ModIDEntry] = SysType.Interface;
                    }
                    else if (ClientNodeID.ContainsKey(ModIDEntry))
                    {
                        Result[ModIDEntry] = SysType.Node;
                    }
                }
            }

            return Result;
        }

        private Dictionary<string, Godot.Object> ServerSys;
        private Dictionary<string, NodeID> ServerNodeID;
        private List<string> ServerOrder;

        public Dictionary<string, SysType> GetAllServer()
        {
            Dictionary<string, SysType> Result = new Dictionary<string, SysType>();

            foreach (var ModIDEntry in ServerOrder)
            {
                if (ServerSys.ContainsKey(ModIDEntry))
                {
                    Result[ModIDEntry] = SysType.Interface;
                }
                else if (ServerNodeID.ContainsKey(ModIDEntry))
                {
                    Result[ModIDEntry] = SysType.Node;
                }
            }

            return Result;
        }

        public Dictionary<string, SysType> GetAllServer(string ModID)
        {
            Dictionary<string, SysType> Result = new Dictionary<string, SysType>();

            foreach (var ModIDEntry in ServerOrder)
            {
                if (ModIDEntry.StartsWith(ModID + '.'))
                {
                    if (ServerSys.ContainsKey(ModIDEntry))
                    {
                        Result[ModIDEntry] = SysType.Interface;
                    }
                    else if (ServerNodeID.ContainsKey(ModIDEntry))
                    {
                        Result[ModIDEntry] = SysType.Node;
                    }
                }
            }

            return Result;
        }

        public void Init()
        {
            Ref = this;

            SharedSys = new Dictionary<string, Godot.Object>();
            SharedNodeID = new Dictionary<string, NodeID>();
            SharedOrder = new List<string>();

            ClientSys = new Dictionary<string, Godot.Object>();
            ClientNodeID = new Dictionary<string, NodeID>();
            ClientOrder = new List<string>();

            ServerSys = new Dictionary<string, Godot.Object>();
            ServerNodeID = new Dictionary<string, NodeID>();
            ServerOrder = new List<string>();

            AddSharedNodeLoad<ConsoleSys>("Console");

            ConsoleSys = GetSharedNode<ConsoleSys>();

            Common NewCommon = new Common();
            NewCommon.AddShared();

            AddShared<BootSys>();
            GetShared<BootSys>().Add("Unary_Common", NewCommon);

            if(GetShared<ConfigSys>().GetShared<bool>("Common.Headless"))
            {
                GetShared<BootSys>().AddServer("Unary_Common");
            }
            else
            {
                GetShared<BootSys>().AddClient("Unary_Common");
            }

            AddShared<ModSys>();
            InitCore(GetShared<ModSys>().Core.ModID, GetShared<ModSys>().Core.Path);
        }

        public void ClearMods()
        {


            ClearAllServerMod();
            ClearAllClientMod();
            ClearAllSharedMod();
        }

        public void Clear()
        {
            ClearMods();

            ClearAllServer();
            ClearAllClient();
            ClearAllShared();
        }

        // Shared systems

        public void AddShared<T>() where T : Godot.Object, IShared
        {
            Type Type = typeof(T);
            string ModIDEntry = ModIDUtil.FromType(Type);

            if(!ModIDUtil.Validate(ModIDEntry))
            {
                ConsoleSys.Error("Failed to validate " + ModIDEntry);
                return;
            }

            if (SharedSys.ContainsKey(ModIDEntry) || SharedNodeID.ContainsKey(ModIDEntry))
            {
                ConsoleSys.Error(ModIDEntry + " is already registered as Shared.");
                return;
            }

            try
            {
                T NewSys = (T)Activator.CreateInstance(Type);
                NewSys.Init();
                SharedSys[ModIDEntry] = NewSys;
                SharedOrder.Add(ModIDEntry);
            }
            catch (Exception Exception)
            {
                ConsoleSys.Error("Failed to register " + ModIDEntry + " as Shared.");
                ConsoleSys.Error(Exception.Message);
            }
        }

        public void AddSharedNodeLoad<T>(string Path) where T : Node, IShared
        {
            Type Type = typeof(T);
            string ModIDEntry = ModIDUtil.FromType(Type);

            if (!ModIDUtil.Validate(ModIDEntry))
            {
                ConsoleSys.Error("Failed to validate " + ModIDEntry);
                return;
            }

            if (SharedSys.ContainsKey(ModIDEntry) || SharedNodeID.ContainsKey(ModIDEntry))
            {
                ConsoleSys.Error(ModIDEntry + " is already registered as Shared.");
                return;
            }

            try
            {
                T ActualNode = NodeUtil.NewNode<T>(ModIDUtil.ModID(ModIDEntry), Path);
                ActualNode.Init();
                ActualNode.Name = ModIDEntry;
                AddChild(ActualNode, true);

                NodeID NewNodeSys = new NodeID
                {
                    Path = Path,
                    ID = GetChildCount() - 1
                };

                SharedNodeID[ModIDEntry] = NewNodeSys;
                SharedOrder.Add(ModIDEntry);
            }
            catch (Exception Exception)
            {
                ConsoleSys.Error("Failed to register " + ModIDEntry + " as Shared.");
                ConsoleSys.Error(Exception.Message);
            }
        }

        public void AddSharedNode<T>() where T : Node, IShared
        {
            Type Type = typeof(T);
            string ModIDEntry = ModIDUtil.FromType(Type);

            if (!ModIDUtil.Validate(ModIDEntry))
            {
                ConsoleSys.Error("Failed to validate " + ModIDEntry);
                return;
            }

            if (SharedSys.ContainsKey(ModIDEntry) || SharedNodeID.ContainsKey(ModIDEntry))
            {
                ConsoleSys.Error(ModIDEntry + " is already registered as Shared.");
                return;
            }

            try
            {
                T NewSys = (T)Activator.CreateInstance(Type);
                NewSys.Init();
                NewSys.Name = ModIDEntry;
                AddChild(NewSys, true);

                NodeID NewNodeSys = new NodeID
                {
                    ID = GetChildCount() - 1
                };

                SharedNodeID[ModIDEntry] = NewNodeSys;
                SharedOrder.Add(ModIDEntry);
            }
            catch (Exception Exception)
            {
                ConsoleSys.Error("Failed to register " + ModIDEntry + " as Shared.");
                ConsoleSys.Error(Exception.Message);
            }
        }

        public T GetShared<T>() where T : Godot.Object, IShared
        {
            string ModIDEntry = ModIDUtil.FromType(typeof(T));

            if (!ModIDUtil.Validate(ModIDEntry))
            {
                ConsoleSys.Error("Failed to verify " + ModIDEntry);
                return default;
            }

            if (SharedSys.ContainsKey(ModIDEntry))
            {
                return (T)SharedSys[ModIDEntry];
            }
            else
            {
                ConsoleSys.Error("Failed to get find Shared sys " + ModIDEntry);
                return default;
            }
        }

        public Godot.Object GetShared(string ModIDEntry)
        {
            if (!ModIDUtil.Validate(ModIDEntry))
            {
                ConsoleSys.Error("Failed to verify " + ModIDEntry);
                return default;
            }

            if (SharedSys.ContainsKey(ModIDEntry))
            {
                return SharedSys[ModIDEntry];
            }
            else
            {
                ConsoleSys.Error("Failed to get find Shared sys " + ModIDEntry);
                return default;
            }
        }

        public T GetSharedNode<T>() where T : Node, IShared
        {
            string ModIDEntry = ModIDUtil.FromType(typeof(T));

            if (!ModIDUtil.Validate(ModIDEntry))
            {
                ConsoleSys.Error("Failed to verify " + ModIDEntry);
                return default;
            }

            if (SharedNodeID.ContainsKey(ModIDEntry))
            {
                return (T)GetChild(SharedNodeID[ModIDEntry].ID);
            }
            else
            {
                ConsoleSys.Error("Failed to get find Shared sys " + ModIDEntry);
                return default;
            }
        }

        public Godot.Object GetSharedNode(string ModIDEntry)
        {
            if (!ModIDUtil.Validate(ModIDEntry))
            {
                ConsoleSys.Error("Failed to verify " + ModIDEntry);
                return default;
            }

            if (SharedNodeID.ContainsKey(ModIDEntry))
            {
                return GetChild(SharedNodeID[ModIDEntry].ID);
            }
            else
            {
                ConsoleSys.Error("Failed to get find Shared sys " + ModIDEntry);
                return default;
            }
        }

        public void InitSharedCore(string ModID, string Path)
        {
            foreach (var OrderModID in SharedOrder)
            {
                if (SharedSys.ContainsKey(OrderModID))
                {
                    IShared Shared = (IShared)SharedSys[OrderModID];
                    Shared.InitCore(ModID, Path);
                }
                else if (SharedNodeID.ContainsKey(OrderModID))
                {
                    GetChild<IShared>(SharedNodeID[OrderModID].ID).InitCore(ModID, Path);
                }
                else
                {
                    ConsoleSys.Error("Could not init Shared Core " + OrderModID);
                }
            }
        }

        public void InitSharedMod(string ModID, string Path)
        {
            foreach (var OrderModID in SharedOrder)
            {
                if (SharedSys.ContainsKey(OrderModID))
                {
                    IShared Shared = (IShared)SharedSys[OrderModID];
                    Shared.InitCore(ModID, Path);
                }
                else if (SharedNodeID.ContainsKey(OrderModID))
                {
                    GetChild<IShared>(SharedNodeID[OrderModID].ID).InitMod(ModID, Path);
                }
                else
                {
                    ConsoleSys.Error("Could not init Shared Mod " + OrderModID);
                }
            }
        }

        public void ReloadSharedNodes()
        {
            foreach (var ModID in SharedOrder)
            {
                if (SharedNodeID.ContainsKey(ModID))
                {
                    if(SharedNodeID[ModID].Path != null)
                    {
                        //Replacing original node with a node that got new children attached to it
                        GetChild(SharedNodeID[ModID].ID).ReplaceBy(NodeUtil.ReloadNode(SharedNodeID[ModID].Path, GetChild(SharedNodeID[ModID].ID)), true);                        ;
                    }

                    //Rehooking all of the nodes with it
                    GetChild(SharedNodeID[ModID].ID)._Ready();
                }
            }
        }

        public void ClearShared(string ModID)
        {
            if (SharedSys.ContainsKey(ModID))
            {
                IShared Shared = (IShared)SharedSys[ModID];
                Shared.Clear();
                SharedSys[ModID].Free();
                SharedSys.Remove(ModID);
            }
            else if (SharedNodeID.ContainsKey(ModID))
            {
                GetChild<IShared>(SharedNodeID[ModID].ID).Clear();
                GetChild(SharedNodeID[ModID].ID).QueueFree();
                SharedNodeID.Remove(ModID);
            }
            else
            {
                ConsoleSys.Error("Could not clear Shared " + ModID);
            }
        }

        public void ClearAllShared()
        {
            for (int i = SharedOrder.Count - 1; i >= 0; --i)
            {
                string ModID = SharedOrder[i];
                ClearShared(ModID);
                SharedOrder.RemoveAt(i);
            }
        }

        // Client systems

        public void AddClient<T>() where T : Godot.Object, IClient
        {
            Type Type = typeof(T);
            string ModIDEntry = ModIDUtil.FromType(Type);

            if (!ModIDUtil.Validate(ModIDEntry))
            {
                ConsoleSys.Error("Failed to validate " + ModIDEntry);
                return;
            }

            if (ClientSys.ContainsKey(ModIDEntry) || ClientNodeID.ContainsKey(ModIDEntry))
            {
                ConsoleSys.Error(ModIDEntry + " is already registered as Client.");
                return;
            }

            try
            {
                T NewSys = (T)Activator.CreateInstance(Type);
                NewSys.Init();
                ClientSys[ModIDEntry] = NewSys;
                ClientOrder.Add(ModIDEntry);
            }
            catch (Exception Exception)
            {
                ConsoleSys.Error("Failed to register " + ModIDEntry + " as Client.");
                ConsoleSys.Error(Exception.Message);
            }
        }

        public void AddClientNodeLoad<T>(string Path) where T : Node, IClient
        {
            Type Type = typeof(T);
            string ModIDEntry = ModIDUtil.FromType(Type);

            if (!ModIDUtil.Validate(ModIDEntry))
            {
                ConsoleSys.Error("Failed to validate " + ModIDEntry);
                return;
            }

            if (ClientSys.ContainsKey(ModIDEntry) || ClientNodeID.ContainsKey(ModIDEntry))
            {
                ConsoleSys.Error(ModIDEntry + " is already registered as Client.");
                return;
            }

            try
            {
                T ActualNode = NodeUtil.NewNode<T>(ModIDUtil.ModID(ModIDEntry), Path);
                ActualNode.Init();
                ActualNode.Name = ModIDEntry;
                AddChild(ActualNode, true);

                NodeID NewNodeSys = new NodeID
                {
                    Path = Path,
                    ID = GetChildCount() - 1
                };

                ClientNodeID[ModIDEntry] = NewNodeSys;
                ClientOrder.Add(ModIDEntry);
            }
            catch (Exception Exception)
            {
                ConsoleSys.Error("Failed to register " + ModIDEntry + " as Client.");
                ConsoleSys.Error(Exception.Message);
            }
        }

        public void AddClientNode<T>() where T : Node, IClient
        {
            Type Type = typeof(T);
            string ModIDEntry = ModIDUtil.FromType(Type);

            if (!ModIDUtil.Validate(ModIDEntry))
            {
                ConsoleSys.Error("Failed to validate " + ModIDEntry);
                return;
            }

            if (ClientSys.ContainsKey(ModIDEntry) || ClientNodeID.ContainsKey(ModIDEntry))
            {
                ConsoleSys.Error(ModIDEntry + " is already registered as Client.");
                return;
            }

            try
            {
                T NewSys = (T)Activator.CreateInstance(Type);
                NewSys.Init();
                NewSys.Name = ModIDEntry;
                AddChild(NewSys, true);

                NodeID NewNodeSys = new NodeID
                {
                    ID = GetChildCount() - 1
                };

                ClientNodeID[ModIDEntry] = NewNodeSys;
                ClientOrder.Add(ModIDEntry);
            }
            catch (Exception Exception)
            {
                ConsoleSys.Error("Failed to register " + ModIDEntry + " as Client.");
                ConsoleSys.Error(Exception.Message);
            }
        }

        public T GetClient<T>() where T : Godot.Object, IClient
        {
            string ModIDEntry = ModIDUtil.FromType(typeof(T));

            if (!ModIDUtil.Validate(ModIDEntry))
            {
                ConsoleSys.Error("Failed to verify " + ModIDEntry);
                return default;
            }

            if (ClientSys.ContainsKey(ModIDEntry))
            {
                return (T)ClientSys[ModIDEntry];
            }
            else
            {
                ConsoleSys.Error("Failed to get find Client sys " + ModIDEntry);
                return default;
            }
        }

        public Godot.Object GetClient(string ModIDEntry)
        {
            if (!ModIDUtil.Validate(ModIDEntry))
            {
                ConsoleSys.Error("Failed to verify " + ModIDEntry);
                return default;
            }

            if (ClientSys.ContainsKey(ModIDEntry))
            {
                return ClientSys[ModIDEntry];
            }
            else
            {
                ConsoleSys.Error("Failed to get find Client sys " + ModIDEntry);
                return default;
            }
        }

        public T GetClientNode<T>() where T : Node, IClient
        {
            string ModIDEntry = ModIDUtil.FromType(typeof(T));

            if (!ModIDUtil.Validate(ModIDEntry))
            {
                ConsoleSys.Error("Failed to verify " + ModIDEntry);
                return default;
            }

            if (ClientNodeID.ContainsKey(ModIDEntry))
            {
                return (T)GetChild(ClientNodeID[ModIDEntry].ID);
            }
            else
            {
                ConsoleSys.Error("Failed to get find Client sys " + ModIDEntry);
                return default;
            }
        }

        public Godot.Object GetClientNode(string ModIDEntry)
        {
            if (!ModIDUtil.Validate(ModIDEntry))
            {
                ConsoleSys.Error("Failed to verify " + ModIDEntry);
                return default;
            }

            if (ClientNodeID.ContainsKey(ModIDEntry))
            {
                return GetChild(ClientNodeID[ModIDEntry].ID);
            }
            else
            {
                ConsoleSys.Error("Failed to get find Client sys " + ModIDEntry);
                return default;
            }
        }

        public void ReloadClientNodes()
        {
            foreach (var ModID in ClientOrder)
            {
                if (ClientNodeID.ContainsKey(ModID))
                {
                    if (ClientNodeID[ModID].Path != null)
                    {
                        //Replacing original node with a node that got new children attached to it
                        GetChild(ClientNodeID[ModID].ID).ReplaceBy(NodeUtil.ReloadNode(ClientNodeID[ModID].Path, GetChild(ClientNodeID[ModID].ID)), true); ;
                    }

                    //Rehooking all of the nodes with it
                    GetChild(ClientNodeID[ModID].ID)._Ready();
                }
            }
        }

        public void ClearClient(string ModID)
        {
            if (ClientSys.ContainsKey(ModID))
            {
                IClient Client = (IClient)ClientSys[ModID];
                Client.Clear();
                ClientSys[ModID].Free();
                ClientSys.Remove(ModID);
            }
            else if (ClientNodeID.ContainsKey(ModID))
            {
                GetChild<IClient>(ClientNodeID[ModID].ID).Clear();
                GetChild(ClientNodeID[ModID].ID).QueueFree();
                ClientNodeID.Remove(ModID);
            }
            else
            {
                ConsoleSys.Error("Could not clear Client " + ModID);
            }
        }

        public void ClearAllClient()
        {
            for (int i = ClientOrder.Count - 1; i >= 0; --i)
            {
                string ModID = ClientOrder[i];
                ClearClient(ModID);
                ClientOrder.RemoveAt(i);
            }
        }

        // Server systems

        public void AddServer<T>() where T : Godot.Object, IServer
        {
            Type Type = typeof(T);
            string ModIDEntry = ModIDUtil.FromType(Type);

            if (!ModIDUtil.Validate(ModIDEntry))
            {
                ConsoleSys.Error("Failed to validate " + ModIDEntry);
                return;
            }

            if (ServerSys.ContainsKey(ModIDEntry) || ServerNodeID.ContainsKey(ModIDEntry))
            {
                ConsoleSys.Error(ModIDEntry + " is already registered as Server.");
                return;
            }

            try
            {
                T NewSys = (T)Activator.CreateInstance(Type);
                NewSys.Init();
                ServerSys[ModIDEntry] = NewSys;
                ServerOrder.Add(ModIDEntry);
            }
            catch (Exception Exception)
            {
                ConsoleSys.Error("Failed to register " + ModIDEntry + " as Server.");
                ConsoleSys.Error(Exception.Message);
            }
        }

        public void AddServerNodeLoad<T>(string Path) where T : Node, IServer
        {
            Type Type = typeof(T);
            string ModIDEntry = ModIDUtil.FromType(Type);

            if (!ModIDUtil.Validate(ModIDEntry))
            {
                ConsoleSys.Error("Failed to validate " + ModIDEntry);
                return;
            }

            if (ServerSys.ContainsKey(ModIDEntry) || ServerNodeID.ContainsKey(ModIDEntry))
            {
                ConsoleSys.Error(ModIDEntry + " is already registered as Server.");
                return;
            }

            try
            {
                T ActualNode = NodeUtil.NewNode<T>(ModIDUtil.ModID(ModIDEntry), Path);
                ActualNode.Init();
                ActualNode.Name = ModIDEntry;
                AddChild(ActualNode, true);

                NodeID NewNodeSys = new NodeID
                {
                    Path = Path,
                    ID = GetChildCount() - 1
                };

                ServerNodeID[ModIDEntry] = NewNodeSys;
                ServerOrder.Add(ModIDEntry);
            }
            catch (Exception Exception)
            {
                ConsoleSys.Error("Failed to register " + ModIDEntry + " as Server.");
                ConsoleSys.Error(Exception.Message);
            }
        }

        public void AddServerNode<T>() where T : Node, IServer
        {
            Type Type = typeof(T);
            string ModIDEntry = ModIDUtil.FromType(Type);

            if (!ModIDUtil.Validate(ModIDEntry))
            {
                ConsoleSys.Error("Failed to validate " + ModIDEntry);
                return;
            }

            if (ServerSys.ContainsKey(ModIDEntry) || ServerNodeID.ContainsKey(ModIDEntry))
            {
                ConsoleSys.Error(ModIDEntry + " is already registered as Server.");
                return;
            }

            try
            {
                T NewSys = (T)Activator.CreateInstance(Type);
                NewSys.Init();
                NewSys.Name = ModIDEntry;
                AddChild(NewSys, true);

                NodeID NewNodeSys = new NodeID
                {
                    ID = GetChildCount() - 1
                };

                ServerNodeID[ModIDEntry] = NewNodeSys;
                ServerOrder.Add(ModIDEntry);
            }
            catch (Exception Exception)
            {
                ConsoleSys.Error("Failed to register " + ModIDEntry + " as Server.");
                ConsoleSys.Error(Exception.Message);
            }
        }

        public T GetServer<T>() where T : Godot.Object, IServer
        {
            string ModIDEntry = ModIDUtil.FromType(typeof(T));

            if (!ModIDUtil.Validate(ModIDEntry))
            {
                ConsoleSys.Error("Failed to verify " + ModIDEntry);
                return default;
            }

            if (ServerSys.ContainsKey(ModIDEntry))
            {
                return (T)ServerSys[ModIDEntry];
            }
            else
            {
                ConsoleSys.Error("Failed to get find Server sys " + ModIDEntry);
                return default;
            }
        }

        public Godot.Object GetServer(string ModIDEntry)
        {
            if (!ModIDUtil.Validate(ModIDEntry))
            {
                ConsoleSys.Error("Failed to verify " + ModIDEntry);
                return default;
            }

            if (ServerSys.ContainsKey(ModIDEntry))
            {
                return ServerSys[ModIDEntry];
            }
            else
            {
                ConsoleSys.Error("Failed to get find Server sys " + ModIDEntry);
                return default;
            }
        }

        public T GetServerNode<T>() where T : Node, IServer
        {
            string ModIDEntry = ModIDUtil.FromType(typeof(T));

            if (!ModIDUtil.Validate(ModIDEntry))
            {
                ConsoleSys.Error("Failed to verify " + ModIDEntry);
                return default;
            }

            if (ServerNodeID.ContainsKey(ModIDEntry))
            {
                return (T)GetChild(ServerNodeID[ModIDEntry].ID);
            }
            else
            {
                ConsoleSys.Error("Failed to get find Server sys " + ModIDEntry);
                return default;
            }
        }

        public Godot.Object GetServerNode(string ModIDEntry)
        {
            if (!ModIDUtil.Validate(ModIDEntry))
            {
                ConsoleSys.Error("Failed to verify " + ModIDEntry);
                return default;
            }

            if (ServerNodeID.ContainsKey(ModIDEntry))
            {
                return GetChild(ServerNodeID[ModIDEntry].ID);
            }
            else
            {
                ConsoleSys.Error("Failed to get find Server sys " + ModIDEntry);
                return default;
            }
        }

        public void ReloadServerNodes()
        {
            foreach (var ModID in ServerOrder)
            {
                if (ServerNodeID.ContainsKey(ModID))
                {
                    if (ServerNodeID[ModID].Path != null)
                    {
                        //Replacing original node with a node that got new children attached to it
                        GetChild(ServerNodeID[ModID].ID).ReplaceBy(NodeUtil.ReloadNode(ServerNodeID[ModID].Path, GetChild(ServerNodeID[ModID].ID)), true); ;
                    }

                    //Rehooking all of the nodes with it
                    GetChild(ServerNodeID[ModID].ID)._Ready();
                }
            }
        }

        public void ClearServer(string ModID)
        {
            if (ServerSys.ContainsKey(ModID))
            {
                IServer Server = (IServer)ServerSys[ModID];
                Server.Clear();
                ServerSys[ModID].Free();
                ServerSys.Remove(ModID);
            }
            else if (ServerNodeID.ContainsKey(ModID))
            {
                GetChild<IServer>(ServerNodeID[ModID].ID).Clear();
                GetChild(ServerNodeID[ModID].ID).QueueFree();
                ServerNodeID.Remove(ModID);
            }
            else
            {
                ConsoleSys.Error("Could not clear Server " + ModID);
            }
        }

        public void ClearAllServer()
        {
            for (int i = ServerOrder.Count - 1; i >= 0; --i)
            {
                string ModID = ServerOrder[i];
                ClearServer(ModID);
                ServerOrder.RemoveAt(i);
            }
        }

        // Util functions

        public void InitCore(string ModID, string Path)
        {
            InitSharedCore(ModID, Path);
        }

        public void InitMod(string ModID, string Path)
        {
            InitSharedMod(ModID, Path);
        }

        public void ClearMods(List<string> ExcludedModIDs)
        {
            for (int i = ServerOrder.Count - 1; i >= 0; --i)
            {
                string ModID = ServerOrder[i];

                if (!ExcludedModIDs.Contains(ModID))
                {
                    ClearServer(ModID);
                    ServerOrder.RemoveAt(i);
                }
            }

            for (int i = ClientOrder.Count - 1; i >= 0; --i)
            {
                string ModID = ClientOrder[i];

                if (!ExcludedModIDs.Contains(ModID))
                {
                    ClearClient(ModID);
                    ClientOrder.RemoveAt(i);
                }
            }

            for (int i = SharedOrder.Count - 1; i >= 0; --i)
            {
                string ModID = SharedOrder[i];

                if (!ExcludedModIDs.Contains(ModID))
                {
                    ClearShared(ModID);
                    SharedOrder.RemoveAt(i);
                }
            }
        }
    }
}
