using System;
using System.Collections.Generic;

using Godot;

namespace Unary.Common.Structs
{
    public struct Command
    {
        public string Target;
        public string Arguments;
        public string Alias;
        public string AliasColor;
        public string Description;
        public string DescriptionColor;
    }
}