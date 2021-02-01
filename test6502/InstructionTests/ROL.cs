using cpu6502;
using Xunit;

namespace test6502.InstructionTests
{
    public class ROL : CPUTest
    {
        [Theory]
        [InlineData(10, 100)]
        [InlineData(0x00, 100)]
        [InlineData(0xFF, 100)]
        [InlineData(0, 0x0000)]
        [InlineData(0, 0xFFFF)]
        [InlineData(0xFF, 0x0000)]
        [InlineData(0xFF, 0xFFFF)]
        public void OpCode_ROL(byte val, ushort addr)
        {
            CPU cpu = new CPU();
            cpu.RAM = GetCleanRam();
            cpu.RAM[addr] = val;
            cpu.OpCode_ROL(addr);

            ushort longVal = (ushort)(val << 1);
            byte newVal = (byte)(val << 1);
            Assert.Equal(newVal, cpu.RAM[addr]);
            Assert.Equal(newVal == 0, cpu.ZeroFlag);
            Assert.Equal(longVal > 0xFF, cpu.CarryFlag);
            Assert.Equal((newVal & 0x80) > 0, cpu.NegativeFlag);
        }

        [Theory]
        [InlineData(10)]
        [InlineData(0x00)]
        [InlineData(0xFF)]
        public void OpCode_ROL_ACC(byte val)
        {
            CPU cpu = new CPU();
            cpu.RAM = GetCleanRam();
            cpu.Accumulator = val;
            cpu.OpCode_ROL_ACC(0x100);

            ushort longVal = (ushort)(val << 1);
            byte newVal = (byte)(val << 1);
            Assert.Equal(newVal, cpu.Accumulator);
            Assert.Equal(newVal == 0, cpu.ZeroFlag);
            Assert.Equal(longVal > 0xFF, cpu.CarryFlag);
            Assert.Equal((newVal & 0x80) > 0, cpu.NegativeFlag);
        }
    }
}
