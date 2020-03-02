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

using System;
using System.Collections.Generic;

using Godot;

namespace Unary_Common.Shared
{
    public class RandomSys : Node, IShared
    {
        public string IDCharacters { get; set; } = "0123456789ABCDEF";

        private uint Selector;
        private uint Range;

        private List<RareTickSubscriber> RareTickSubscribers;

        public override void _Process(float delta)
        {
            if(Chance(Selector, Range))
            {
                for(int i = RareTickSubscribers.Count - 1; i >= 0; --i)
                {
                    if(IsInstanceValid(RareTickSubscribers[i].Subscriber.Target))
                    {
                        if(RareTickSubscribers[i].Selector == RareTickSubscribers[i].Range)
                        {
                            RareTickSubscribers[i].Subscriber.Target.Call(RareTickSubscribers[i].Subscriber.MemberName);
                        }
                        else if(Chance(RareTickSubscribers[i].Selector, RareTickSubscribers[i].Range))
                        {
                            RareTickSubscribers[i].Subscriber.Target.Call(RareTickSubscribers[i].Subscriber.MemberName);
                        }
                    }
                    else
                    {
                        RareTickSubscribers.RemoveAt(i);
                    }
                }
            }
        }

        public void Init()
        {
            ConfigSys Config = Sys.Ref.GetShared<ConfigSys>();

            RareTickSubscribers = new List<RareTickSubscriber>();

            Selector = Config.GetShared<uint>("Unary_Common.Random.RareTickSelector");
            Range = Config.GetShared<uint>("Unary_Common.Random.RareTickRange");
        }

        public void Clear()
        {
            RareTickSubscribers.Clear();
        }

        public void ClearMod(Mod Mod)
        {

        }

        public void ClearedMods()
        {

        }

        public void InitCore(Mod Mod)
        {

        }

        public void InitMod(Mod Mod)
        {

        }

        public void SetSeed(long Seed)
        {
            GD.Seed((ulong)Seed);
        }

        public float RandFloat(float Min, float Max)
        {
            return (float)GD.RandRange(Min, Max);
        }

        public float RandFloat()
        {
            return GD.Randf();
        }

        public int RandInt()
        {
            return (int)GD.Randi();
        }

        public int RandInt(int Min, int Max)
        {
            return (int)GD.RandRange(Min, Max);
        }

        public uint RandUInt()
        {
            return GD.Randi();
        }

        public uint RandUInt(uint Min, uint Max)
        {
            return (uint)GD.RandRange(Min, Max);
        }

        public string RandID(int Pools = 4)
        {
            string Result = default;

            int IDCharsLength = IDCharacters.Length;

            for(int i = 0; i < Pools; ++i)
            {
                for(int c = 0; c < 4; ++c)
                {
                    Result += IDCharacters[RandInt(0, IDCharsLength)];
                }

                if((i + 1) < Pools)
                {
                    Result += '-';
                }
            }

            return Result;
        }

        public static long GetSeed(string Text)
        {
            int First = Text.Substring(0, Text.Length / 2).GetHashCode();
            int Second = Text.Substring(Text.Length / 2).GetHashCode();

            var Bytes = new byte[8];

            BitConverter.GetBytes(First).CopyTo(Bytes, 0);
            BitConverter.GetBytes(Second).CopyTo(Bytes, 4);

            return BitConverter.ToInt64(Bytes, 0);
        }

        public bool Chance(uint Selector = 1, uint Range = 20)
        {
            if((uint)GD.RandRange(0, Selector - 1) < Range - 1)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public void RareTickSubscribe(Godot.Object Target, string MemberName, uint Selector = 1, uint Range = 1)
        {
            RareTickSubscriber NewSubscriber = new RareTickSubscriber()
            {
                Subscriber = new Subscriber()
                {
                    Target = Target,
                    MemberName = MemberName,
                    Type = SubscriberType.Method
                },
                Selector = Selector,
                Range = Range
            };

            RareTickSubscribers.Add(NewSubscriber);
        }
    }
}
