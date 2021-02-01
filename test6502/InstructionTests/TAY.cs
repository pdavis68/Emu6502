using cpu6502;
using Xunit;

namespace test6502.InstructionTests
{
    public class TAY : CPUTest
    {
        [Theory]
        [InlineData(0)]
        [InlineData(10)]
        [InlineData(0x7F)]
        [InlineData(0xFF)]
        public void OpCode_TAY(byte val)
        {
            CPU cpu = new CPU();
            cpu.RAM = GetCleanRam();
            cpu.Accumulator = val;
            cpu.OpCode_TAY(0);

            Assert.Equal((val & 0x80) > 0, cpu.NegativeFlag);
            Assert.Equal(val == 0, cpu.ZeroFlag);
            Assert.Equal(val, cpu.Accumulator);
            Assert.Equal(val, cpu.YRegister);
        }
    }
}
