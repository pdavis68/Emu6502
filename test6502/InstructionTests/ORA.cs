using cpu6502;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace test6502.InstructionTests
{
    public class ORA : CPUTest
    {
        [Theory]
        [InlineData(10, 100, 0x10)]
        [InlineData(0x00, 100, 0x20)]
        [InlineData(0xFF, 100, 0x20)]
        [InlineData(0x20, 100, 0xFF)]
        [InlineData(0x20, 100, 0x00)]
        [InlineData(0, 0x0000, 0x00)]
        [InlineData(0, 0xFFFF, 0x00)]
        [InlineData(0, 0x0000, 0xFF)]
        [InlineData(0, 0xFFFF, 0xFF)]
        [InlineData(0xFF, 0x0000, 0x00)]
        [InlineData(0xFF, 0xFFFF, 0x00)]
        [InlineData(0xFF, 0x0000, 0xFF)]
        [InlineData(0xFF, 0xFFFF, 0xFF)]
        public void OpCode_ORA(byte val, ushort addr, byte accumulator)
        {
            CPU cpu = new CPU();
            cpu.RAM = GetCleanRam();
            cpu.RAM[addr] = val;
            cpu.Accumulator = accumulator;
            cpu.OpCode_ORA(addr);
            byte newVal = (byte) (val | accumulator);
            Assert.Equal(newVal, cpu.Accumulator);
            Assert.Equal((newVal & 0x80) > 0, cpu.NegativeFlag);
            Assert.Equal(newVal == 0, cpu.ZeroFlag);

        }

    }
}
