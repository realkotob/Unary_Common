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

using System;
using System.Collections.Generic;
using System.Linq;

using Godot;

using Newtonsoft.Json;

namespace Unary_Common.Utils
{
    public static class FilesystemUtil
    {

        // System filesystem methods

        public static bool SystemDirContainsFiles(string Path, params string[] Files)
        {
            if(!SystemDirExists(Path))
            {
                return false;
            }
            else
            {
                foreach(var Entry in Files)
                {
                    if(!SystemFileExists(Path + '/' + Entry))
                    {
                        return false;
                    }
                }

                return true;
            }
        }

        public static bool SystemDirContainsDir(string Path, string Dir)
        {
            if (!System.IO.Directory.Exists(Path))
            {
                return false;
            }
            else
            {
                if(System.IO.Directory.Exists(Path + '/' + Dir))
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        public static void SystemDirCreate(string Path)
        {
            try
            {
                System.IO.Directory.CreateDirectory(Path);
            }
            catch(Exception)
            {

            }
        }

        public static bool SystemDirExists(string Path)
        {
            return System.IO.Directory.Exists(Path);
        }

        public static void SystemDirDelete(string Path)
        {
            try
            {
                System.IO.Directory.Delete(Path, true);
            }
            catch (Exception)
            {

            }
        }

        public static List<string> SystemDirGetFiles(string Path)
        {
            List<string> Result = new List<string>();

            if (System.IO.Directory.Exists(Path))
            {
                string[] Files = System.IO.Directory.GetFiles(Path, "*.*", System.IO.SearchOption.AllDirectories);
                Result = Files.ToList();
            }

            return Result;
        }

        public static List<string> SystemDirGetDirs(string Path)
        {
            List<string> Result = new List<string>();

            if (System.IO.Directory.Exists(Path))
            {
                string[] Dirs = System.IO.Directory.GetDirectories(Path, "*.*", System.IO.SearchOption.AllDirectories);
                Result = Dirs.ToList();
            }

            return Result;
        }

        public static List<string> SystemDirGetDirsTop(string Path)
        {
            List<string> Result = new List<string>();

            if (System.IO.Directory.Exists(Path))
            {
                string[] Dirs = System.IO.Directory.GetDirectories(Path, "*.*", System.IO.SearchOption.TopDirectoryOnly);
                Result = Dirs.ToList();
            }

            return Result;
        }

        public static bool SystemFileExists(string Path)
        {
            return System.IO.File.Exists(Path);
        }

        public static string SystemFileRead(string Path)
        {
            if(SystemFileExists(Path))
            {
                try
                {
                    return System.IO.File.ReadAllText(Path);
                }
                catch(Exception)
                {
                    return null;
                }
            }
            else
            {
                return null;
            }
        }

        public static void SystemFileMove(string Path, string NewPath)
        {
            try
            {
                System.IO.File.Move(Path, NewPath);
            }
            catch(Exception)
            {
                return;
            }
        }

        public static void SystemFileDelete(string Path)
        {
            try
            {
                System.IO.File.Delete(Path);
            }
            catch(Exception)
            {
                return;
            }
        }

        public static bool SystemFileWrite(string Path, string Text)
        {
            try
            {
                System.IO.File.WriteAllText(Path, Text);
                return true;
            }
            catch(Exception)
            {
                return false;
            }
        }

        public static void SystemFileAppend(string Path, string Text)
        {
            try
            {
                System.IO.File.AppendAllText(Path, Text);
            }
            catch (Exception)
            {
                return;
            }
        }

        public static void SystemFileCreate(string Path)
        {
            try
            {
                System.IO.File.Create(Path).Dispose();
            }
            catch(Exception)
            {
                return;
            }
        }

        // Godot filesystem methods

        //Thanks, Godot!
        public static bool GodotDirExists(string Path)
        {
            Directory NewDir = new Directory();
            if (NewDir.Open(Path) != Error.Ok)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        //Thanks, Godot!
        public static bool GodotFileExists(string Path)
        {
            File NewFile = new File();

            if (NewFile.Open(Path, File.ModeFlags.Read) != Error.Ok)
            {
                NewFile.Close();
                return false;
            }
            else
            {
                NewFile.Close();
                return true;
            }
        }

        public static void GodotFileRemove(string Path)
        {
            if(!GodotFileExists(Path))
            {
                return;
            }

            using (Directory NewDir = new Directory())
            {
                NewDir.Remove(Path);
            }
        }

        public static List<string> GodotDirGetFiles(string Path)
        {
            List<string> Result = new List<string>();

            if (!GodotDirExists(Path))
            {
                return Result;
            }

            GodotGetFiles(Path, ref Result);

            return Result;
        }

        public static void GodotDirRemove(string Path)
        {
            if(!GodotDirExists(Path))
            {
                return;
            }

            {
                List<string> Files = new List<string>();

                GodotGetFiles(Path, ref Files);

                foreach (var File in Files)
                {
                    GodotFileRemove(File);
                }
            }

            {
                List<string> Dirs = new List<string>();

                GodotGetDirs(Path, ref Dirs);

                for(int i = Dirs.Count - 1; i >= 0; --i)
                {
                    GodotFileRemove(Dirs[i]);
                }
            }

            GodotFileRemove(Path);
        }

        public static bool GodotDirContainsDir(string Path, string Dir)
        {
            using (Directory NewDir = new Directory())
            {
                return NewDir.DirExists(Path + '/' + Dir);
            }
        }

        public static bool GodotDirContainsFiles(string Path, params string[] Files)
        {
            using (Directory NewDir = new Directory())
            {
                if (NewDir.Open(Path) != Error.Ok)
                {
                    return false;
                }

                foreach (var Entry in Files)
                {
                    if (!NewDir.FileExists(Entry))
                    {
                        return false;
                    }
                }

                return true;
            }
        }

        private static void GodotGetDirs(string DirName, ref List<string> Dirs)
        {
            using (Directory NewDirectory = new Directory())
            {
                if (NewDirectory.Open(DirName) == Error.Ok)
                {
                    NewDirectory.ListDirBegin(true);
                    string Path = NewDirectory.GetNext();
                    while (Path != "")
                    {
                        string Target = string.Empty;

                        if (DirName == "res://")
                        {
                            Target = "res://" + Path;
                        }
                        else
                        {
                            Target = DirName + '/' + Path;
                        }

                        if (NewDirectory.CurrentIsDir())
                        {
                            Dirs.Add(Target);
                            GodotGetDirs(Target, ref Dirs);
                        }

                        Path = NewDirectory.GetNext();
                    }
                    NewDirectory.ListDirEnd();
                }
                else
                {
                    return;
                }
            }
        }

        private static void GodotGetFiles(string DirName, ref List<string> Files)
        {
            using (Directory NewDirectory = new Directory())
            {
                if (NewDirectory.Open(DirName) == Error.Ok)
                {
                    NewDirectory.ListDirBegin(true);
                    string Path = NewDirectory.GetNext();
                    while (Path != "")
                    {
                        string Target = string.Empty;

                        if (DirName == "res://")
                        {
                            Target = "res://" + Path;
                        }
                        else
                        {
                            Target = DirName + '/' + Path;
                        }

                        if (NewDirectory.CurrentIsDir())
                        {
                            GodotGetFiles(Target, ref Files);
                        }
                        else
                        {
                            Files.Add(Target);
                        }

                        Path = NewDirectory.GetNext();
                    }
                    NewDirectory.ListDirEnd();
                }
                else
                {
                    return;
                }
            }
        }

        public static List<string> GodotGetAllFiles()
        {
            var Result = new List<string>();

            GodotGetFiles("res://", ref Result);

            return Result;
        }

        public static string GodotFileRead(string Path)
        {
            File NewFile = new File();
            string Result = null;

            if (NewFile.Open(Path, File.ModeFlags.Read) != Error.Ok)
            {
                return Result;
            }
            else
            {
                Result = NewFile.GetAsText();
                NewFile.Close();
                return Result;
            }
        }

        public static bool GodotPackPCK(string ModID, string TargetPath, List<string> TargetFolders)
        {
            TargetPath = TargetPath.Replace('\\', '/');

            TargetFolders.Add(ModID);

            List<string> FilesystemPaths = new List<string>();
            List<string> ImportPaths = new List<string>();

            if(SystemFileExists(TargetPath + '/' + ModID + ".pck"))
            {
                SystemFileDelete(TargetPath + '/' + ModID + ".pck");
            }

            //Adding actual ModID content
            {
                foreach(var Folder in TargetFolders)
                {
                    List<string> NewModFiles = SystemDirGetFiles(TargetPath + '/' + Folder);

                    if(ModID == "Common")
                    {
                        foreach (var File in NewModFiles)
                        {
                            FilesystemPaths.Add(File.Replace('\\', '/').Replace("./", ""));
                        }
                    }
                    else
                    {
                        foreach (var File in NewModFiles)
                        {
                            FilesystemPaths.Add(File.Replace('\\', '/'));
                        }
                    }
                }
            }

            //Adding .import content
            {
                List<string> NewImportFiles = SystemDirGetFiles(TargetPath + '/' + ".import");

                foreach (var File in NewImportFiles)
                {
                    if (ModID == "Common")
                    {
                        FilesystemPaths.Add(File.Replace('\\', '/').Replace("./", ""));
                    }
                    else
                    {
                        FilesystemPaths.Add(File.Replace('\\', '/'));
                    }

                    if (ModID == "Common")
                    {
                        ImportPaths.Add("res://" + File.Replace('\\', '/').Replace("./", ""));
                    }
                    else
                    {
                        ImportPaths.Add("res://" + File.Replace('\\', '/').Replace(TargetPath + '/', ""));
                    }
                }
            }

            //Remove originals if .import's are presented
            foreach (var File in FilesystemPaths.ToList())
            {
                if (System.IO.Path.GetExtension(File) == ".import")
                {
                    string RemovalPath = File.Replace(".import", "");

                    if (FilesystemPaths.Contains(RemovalPath))
                    {
                        FilesystemPaths.Remove(RemovalPath);
                    }
                }
            }

            string PackagePath = TargetPath + '/' + ModID + ".pck";
            string ManifestPath = TargetPath + '/' + ModID + ".json";

            //Writing PCK with all the content
            PCKPacker Packer = new PCKPacker();

            if (Packer.PckStart(PackagePath, 0) != Error.Ok)
            {
                return false;
            }

            foreach (var File in FilesystemPaths)
            {
                string InternalPath = "res://" + File.Replace(TargetPath + '/', "");

                if (System.IO.Path.GetExtension(File) == ".cs")
                {
                    string EmptyFile = "res://Common/Empty";

                    if (Packer.AddFile(InternalPath, EmptyFile) != Error.Ok)
                    {
                        return false;
                    }
                }
                else
                {
                    if (Packer.AddFile(InternalPath, File) != Error.Ok)
                    {
                        return false;
                    }
                }
            }

            if (Packer.Flush(false) != Error.Ok)
            {
                return false;
            }

            //Writing PCK manifest in order to remove stuff from .import on Clear
            if (System.IO.File.Exists(ManifestPath))
            {
                System.IO.File.Delete(ManifestPath);
            }

            try
            {
                System.IO.File.WriteAllText(ManifestPath, JsonConvert.SerializeObject(ImportPaths, Formatting.Indented));
            }
            catch(Exception)
            {
                return false;
            }

            return true;
        }
    }
}
