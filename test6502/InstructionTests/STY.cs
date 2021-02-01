using cpu6502;
using Xunit;

namespace test6502.InstructionTests
{
    public class STY : CPUTest
    {
        [Theory]
        [InlineData(10, 100)]
        [InlineData(0x00, 100)]
        [InlineData(0xFF, 100)]
        [InlineData(0, 0x0000)]
        [InlineData(0, 0xFFFF)]
        [InlineData(0xFF, 0x0000)]
        [InlineData(0xFF, 0xFFFF)]
        public void OpCode_STY(byte val, ushort addr)
        {
            CPU cpu = new CPU();
            cpu.RAM = GetCleanRam();
            cpu.YRegister = val;

            cpu.OpCode_STY(addr);

            Assert.Equal(val, cpu.RAM[addr]);
        }
    }
}
