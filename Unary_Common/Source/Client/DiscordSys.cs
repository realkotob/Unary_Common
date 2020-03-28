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
using Unary_Common.Utils;
using Unary_Common.Structs;
using Unary_Common.Abstract;

using Godot;

using System;
using System.Collections.Generic;

using Discord;

namespace Unary_Common.Client
{
    class DiscordSys : SysNode
    {
        private IConsoleSys ConsoleSys;

        public Discord.Discord Client { get; private set; }

        public override void Init()
        {
            ConsoleSys = Shared.Sys.Ref.ConsoleSys;

            try
            {
                Client = new Discord.Discord(670224014903345182, (ulong)CreateFlags.NoRequireDiscord);

                var activity = new Activity
                {
                    State = "Playing",
                    Details = "Playing the game",
                    Timestamps = { Start = 1 },
                    Assets = { LargeImage = "logo" },
                    Instance = true,
                };

                Client.GetActivityManager().UpdateActivity(activity, (result) =>
                {
                    if (result == Result.Ok)
                    {
                        ConsoleSys.Message("Updated Discord presence.");
                    }
                    else
                    {
                        ConsoleSys.Message("Could not update Discord presence.");
                    }
                });
            }
            catch (Exception Exception)
            {
                ConsoleSys.Error("Failed to init Discord");
                ConsoleSys.Error(Exception.Message);
            }
        }

        public override void Clear()
        {
            if (Client != null)
            {
                Client.Dispose();
            }
        }

        public override void _Process(float delta)
        {
            if(Client != null)
            {
                Client.RunCallbacks();
            }
        }
    }
}
