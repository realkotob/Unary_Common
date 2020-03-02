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

namespace Unary_Common.Utils
{
    public static class NodeUtil
    {
        public static Node NewNode(string ModID, string Path)
        {
            string TargetPath = "res://" + ModID + "/Nodes/" + Path + ".tscn";

            if (!ResourceLoader.Exists(TargetPath))
            {
                return default;
            }

            PackedScene NewScene = GD.Load<PackedScene>(TargetPath);
            Node NewNode = NewScene.Instance();

            return NewNode;
        }

        public static T NewResource<T>(string Path) where T : Godot.Object
        {
            try
            {
                if (!ResourceLoader.Exists(Path))
                {
                    return default;
                }

                T NewResource = GD.Load<T>(Path);

                return NewResource;
            }
            catch (Exception)
            {
                return default;
            }
        }

        public static T NewResource<T>(string ModID, string ResourceType, string Path) where T : Godot.Object
        {
            return NewResource<T>("res://" + ModID + '/' + ResourceType + '/' + Path);
        }

        public static T NewNode<T>(string ModID, string Path) where T : Node
        {
            Type Type = typeof(T);

            string TargetPath = "res://" + ModID + "/Nodes/" + Path + ".tscn";

            try
            {
                if (!ResourceLoader.Exists(TargetPath))
                {
                    return default;
                }

                PackedScene NewScene = GD.Load<PackedScene>(TargetPath);
                Node NewNode = NewScene.Instance();

                T ActualNode = (T)Activator.CreateInstance(Type);

                foreach (Node Child in NewNode.GetChildren())
                {
                    NewNode.RemoveChild(Child);
                    ActualNode.AddChild(Child);
                }

                NewNode.Free();

                return ActualNode;
            }
            catch (Exception)
            {
                return default;
            }
        }

        public static Node ReloadNode(string Path, Node Object)
        {
            try
            {
                if (!ResourceLoader.Exists(Path))
                {
                    return default;
                }

                PackedScene NewScene = GD.Load<PackedScene>(Path);
                Node NewNode = NewScene.Instance();

                foreach (Node Child in Object.GetChildren())
                {
                    Child.QueueFree();
                }

                foreach (Node Child in NewNode.GetChildren())
                {
                    Object.AddChild(Child);
                }

                NewNode.Free();

                return Object;
            }
            catch (Exception)
            {
                return default;
            }
        }
    }
}
