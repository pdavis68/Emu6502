using cpu6502;
using Xunit;

namespace test6502.InstructionTests
{
    public class STA : CPUTest
    {
        [Theory]
        [InlineData(10, 100)]
        [InlineData(0x00, 100)]
        [InlineData(0xFF, 100)]
        [InlineData(0, 0x0000)]
        [InlineData(0, 0xFFFF)]
        [InlineData(0xFF, 0x0000)]
        [InlineData(0xFF, 0xFFFF)]
        public void OpCode_STA(byte val, ushort addr)
        {
            CPU cpu = new CPU();
            cpu.RAM = GetCleanRam();
            cpu.Accumulator = val;

            cpu.OpCode_STA(addr);

            Assert.Equal(val, cpu.RAM[addr]);
        }
    }
}
