using cpu6502;
using Xunit;

namespace test6502.InstructionTests
{
    public class STX : CPUTest
    {
        [Theory]
        [InlineData(10, 100)]
        [InlineData(0x00, 100)]
        [InlineData(0xFF, 100)]
        [InlineData(0, 0x0000)]
        [InlineData(0, 0xFFFF)]
        [InlineData(0xFF, 0x0000)]
        [InlineData(0xFF, 0xFFFF)]
        public void OpCode_STX(byte val, ushort addr)
        {
            CPU cpu = new CPU();
            cpu.RAM = GetCleanRam();
            cpu.XRegister = val;

            cpu.OpCode_STX(addr);

            Assert.Equal(val, cpu.RAM[addr]);
        }
    }
}
