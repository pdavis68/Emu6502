using cpu6502;
using Xunit;

namespace test6502.InstructionTests
{
    public class EOR : CPUTest
    {
        [Theory]
        [InlineData(10, 10)]
        [InlineData(0x00, 0x00)]
        [InlineData(0x00, 0xFF)]
        [InlineData(0xFF, 0X00)]
        [InlineData(0xFF, 0xFF)]
        [InlineData(0xFF, 0x10)]
        [InlineData(0xFF, 0x20)]
        [InlineData(0x00, 0x10)]
        [InlineData(0x00, 0x20)]
        [InlineData(0x10, 0x10)]
        [InlineData(0x20, 0x20)]
        public void OpCode_EOR(byte val, byte accumulator)
        {
            CPU cpu = new CPU();
            cpu.RAM = GetCleanRam();
            cpu.RAM[0x100] = val;
            cpu.Accumulator = accumulator;
            cpu.OpCode_EOR(0x100);

            byte newVal = (byte)(val ^ accumulator);

            Assert.Equal((newVal & 0x80) > 0, cpu.NegativeFlag);
            Assert.Equal(newVal == 0, cpu.ZeroFlag);
            Assert.Equal(newVal, cpu.Accumulator);
        }
    }
}