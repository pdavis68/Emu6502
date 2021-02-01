using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cpu6502
{
    public class Instruction
    {
        private Func<ushort> _addressMode;
        private Action<ushort> _opCode;
        private int _cycles;
        private byte _instruction;

        public Instruction(byte instruction, Func<ushort> addressMode, Action<ushort> opCode, int cycles)
        {
            _instruction = instruction;
            _addressMode = addressMode;
            _opCode = opCode;
            _cycles = cycles;
        }

        public Func<ushort> AddressMode => _addressMode;
        public Action<ushort> OpCode => _opCode;
        public byte Instr => _instruction;
        public int Cycles => _cycles;

        public void Execute()
        {
            OpCode(AddressMode());
        }
    }
}
