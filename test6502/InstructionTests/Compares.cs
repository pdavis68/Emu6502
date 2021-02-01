using cpu6502;
using Xunit;

namespace test6502.InstructionTests
{
    public class Compares : CPUTest
    {
        [Theory]
        [InlineData(10, 0)]
        [InlineData(0x00, 00)]
        [InlineData(0x00, 10)]
        [InlineData(0x00, 0xFF)]
        [InlineData(0xFF, 0)]
        [InlineData(0xFF, 10)]
        [InlineData(0xFF, 0xFF)]
        public void OpCode_CMP(byte val, byte acc)
        {
            CPU cpu = new CPU();
            cpu.RAM = GetCleanRam();
            cpu.RAM[0x100] = val;
            cpu.Accumulator = acc;
            cpu.OpCode_CMP(0x100);

            ushort newVal = (ushort) (acc - val);
            Assert.Equal((newVal & 0x80) > 0, cpu.NegativeFlag);
            Assert.Equal(newVal == 0, cpu.ZeroFlag);
            Assert.Equal(newVal < 0x100, cpu.CarryFlag);
            Assert.Equal(acc, cpu.Accumulator);
            Assert.Equal(val, cpu.RAM[0x100]);
        }

        [Theory]
        [InlineData(10, 0)]
        [InlineData(0x00, 00)]
        [InlineData(0x00, 10)]
        [InlineData(0x00, 0xFF)]
        [InlineData(0xFF, 0)]
        [InlineData(0xFF, 10)]
        [InlineData(0xFF, 0xFF)]
        public void OpCode_CPX(byte val, byte xReg)
        {
            CPU cpu = new CPU();
            cpu.RAM = GetCleanRam();
            cpu.RAM[0x100] = val;
            cpu.XRegister = xReg;
            cpu.OpCode_CPX(0x100);

            ushort newVal = (ushort)(xReg - val);
            Assert.Equal((newVal & 0x80) > 0, cpu.NegativeFlag);
            Assert.Equal(newVal == 0, cpu.ZeroFlag);
            Assert.Equal(newVal < 0x100, cpu.CarryFlag);
            Assert.Equal(xReg, cpu.XRegister);
            Assert.Equal(val, cpu.RAM[0x100]);
        }

        [Theory]
        [InlineData(10, 0)]
        [InlineData(0x00, 00)]
        [InlineData(0x00, 10)]
        [InlineData(0x00, 0xFF)]
        [InlineData(0xFF, 0)]
        [InlineData(0xFF, 10)]
        [InlineData(0xFF, 0xFF)]
        public void OpCode_CPY(byte val, byte yReg)
        {
            CPU cpu = new CPU();
            cpu.RAM = GetCleanRam();
            cpu.RAM[0x100] = val;
            cpu.YRegister = yReg;
            cpu.OpCode_CPY(0x100);

            ushort newVal = (ushort)(yReg - val);
            Assert.Equal((newVal & 0x80) > 0, cpu.NegativeFlag);
            Assert.Equal(newVal == 0, cpu.ZeroFlag);
            Assert.Equal(newVal < 0x100, cpu.CarryFlag);
            Assert.Equal(yReg, cpu.YRegister);
            Assert.Equal(val, cpu.RAM[0x100]);
        }
    }
}