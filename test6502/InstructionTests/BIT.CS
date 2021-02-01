using cpu6502;
using Xunit;

namespace test6502.InstructionTests
{
    public class BIT : CPUTest
    {
        [Theory]
        [InlineData(0x00, 100)]
        [InlineData(0xFF, 100)]
        [InlineData(0x00, 0x00)]
        [InlineData(0xFF, 0x00)]
        [InlineData(0x00, 0xFF)]
        [InlineData(0xFF, 0xFF)]
        [InlineData(0x88, 0x00)]
        [InlineData(0x88, 0xFF)]
        public void OpCode_BIT(byte val, byte accumulator)
        {
            CPU cpu = new CPU();
            cpu.RAM = GetCleanRam();
            cpu.RAM[0x100] = val;
            cpu.Accumulator = accumulator;
            cpu.OpCode_BIT(0x100);

            byte res = (byte) (val & accumulator);
            Assert.Equal(cpu.StatusRegister & 0xC0, cpu.RAM[0x100] & 0xC0);
            Assert.Equal(res == 0, cpu.ZeroFlag);
            Assert.Equal(accumulator, cpu.Accumulator);
        }
    }
}