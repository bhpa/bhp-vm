﻿using System.Collections.Generic;

namespace Bhp.VM
{
    public class Debugger
    {
        private readonly ExecutionEngine engine;
        private readonly Dictionary<byte[], HashSet<uint>> break_points = new Dictionary<byte[], HashSet<uint>>(new HashComparer());

        public Debugger(ExecutionEngine engine)
        {
            this.engine = engine;
        }

        public void AddBreakPoint(byte[] script_hash, uint position)
        {
            if (!break_points.TryGetValue(script_hash, out HashSet<uint> hashset))
            {
                hashset = new HashSet<uint>();
                break_points.Add(script_hash, hashset);
            }
            hashset.Add(position);
        }

        public VMState Execute()
        {
            if (engine.State == VMState.BREAK)
                engine.State = VMState.NONE;
            while (engine.State == VMState.NONE)
                ExecuteAndCheckBreakPoints();
            return engine.State;
        }

        private void ExecuteAndCheckBreakPoints()
        {
            engine.ExecuteNext();
            if (engine.State == VMState.NONE && engine.InvocationStack.Count > 0 && break_points.Count > 0)
            {
                if (break_points.TryGetValue(engine.CurrentContext.ScriptHash, out HashSet<uint> hashset) && hashset.Contains((uint)engine.CurrentContext.InstructionPointer))
                    engine.State = VMState.BREAK;
            }
        }

        public bool RemoveBreakPoint(byte[] script_hash, uint position)
        {
            if (!break_points.TryGetValue(script_hash, out HashSet<uint> hashset)) return false;
            if (!hashset.Remove(position)) return false;
            if (hashset.Count == 0) break_points.Remove(script_hash);
            return true;
        }

        public VMState StepInto()
        {
            if (engine.State == VMState.HALT || engine.State == VMState.FAULT)
                return engine.State;
            engine.ExecuteNext();
            if (engine.State == VMState.NONE)
                engine.State = VMState.BREAK;
            return engine.State;
        }

        public VMState StepOut()
        {
            if (engine.State == VMState.BREAK)
                engine.State = VMState.NONE;
            int c = engine.InvocationStack.Count;
            while (engine.State == VMState.NONE && engine.InvocationStack.Count >= c)
                ExecuteAndCheckBreakPoints();
            if (engine.State == VMState.NONE)
                engine.State = VMState.BREAK;
            return engine.State;
        }

        public VMState StepOver()
        {
            if (engine.State == VMState.HALT || engine.State == VMState.FAULT)
                return engine.State;
            engine.State = VMState.NONE;
            int c = engine.InvocationStack.Count;
            do
            {
                ExecuteAndCheckBreakPoints();
            }
            while (engine.State == VMState.NONE && engine.InvocationStack.Count > c);
            if (engine.State == VMState.NONE)
                engine.State = VMState.BREAK;
            return engine.State;
        }
    }
}
