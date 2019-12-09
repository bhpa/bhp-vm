﻿using System.Diagnostics;

namespace Bhp.VM.Types
{
    [DebuggerDisplay("Type={GetType().Name}, Position={Position}")]
    public class Pointer : StackItem
    {
        public int Position { get; }

        public Pointer(int position)
        {
            this.Position = position;
        }

        public override bool Equals(StackItem other)
        {
            if (other == this) return true;
            if (other is Pointer p) return Position == p.Position;
            return false;
        }

        public override int GetHashCode()
        {
            return Position.GetHashCode();
        }

        public override bool GetBoolean()
        {
            return true;
        }

        public override byte[] GetByteArray()
        {
            throw new System.NotImplementedException();
        }
    }
}