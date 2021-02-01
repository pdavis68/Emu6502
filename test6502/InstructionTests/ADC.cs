using cpu6502;
using Xunit;

namespace test6502.InstructionTests
{
    public class ADC : CPUTest
    {
        [Theory]
        [InlineData(10, 10, true, true)]
        [InlineData(0x00, 0x00, true, true)]
        [InlineData(0x00, 0xFF, true, true)]
        [InlineData(0xFF, 0X00, true, true)]
        [InlineData(0xFF, 0xFF, true, true)]
        [InlineData(10, 10, false, true)]
        [InlineData(0x00, 0x00, false, true)]
        [InlineData(0x00, 0xFF, false, true)]
        [InlineData(0xFF, 0X00, false, true)]
        [InlineData(0xFF, 0xFF, false, true)]
        [InlineData(10, 10, true, false)]
        [InlineData(0x00, 0x00, true, false)]
        [InlineData(0x00, 0xFF, true, false)]
        [InlineData(0xFF, 0X00, true, false)]
        [InlineData(0xFF, 0xFF, true, false)]
        [InlineData(10, 10, false, false)]
        [InlineData(0x00, 0x00, false, false)]
        [InlineData(0x00, 0xFF, false, false)]
        [InlineData(0xFF, 0X00, false, false)]
        [InlineData(0xFF, 0xFF, false, false)]
        public void OpCode_ADC(byte val, byte accumulator, bool carry_flag, bool decimal_flag)
        {
            CPU cpu = new CPU();
            cpu.RAM = GetCleanRam();
            cpu.RAM[0x0100] = val;
            cpu.Accumulator = accumulator;
            cpu.SetDecimal(decimal_flag);
            cpu.SetCarry(carry_flag);
            cpu.OpCode_ADC(0x0100);

            byte newVal = PerformADC(val, accumulator, carry_flag, decimal_flag);

            Assert.Equal(GetADCNegative(val, accumulator, carry_flag, decimal_flag), cpu.NegativeFlag);
            Assert.Equal(GetADCZero(val, accumulator, carry_flag, decimal_flag), cpu.ZeroFlag);
            Assert.Equal(newVal, cpu.Accumulator);
        }

        private byte PerformADC(byte val, byte accumulator, bool carry_flag, bool decimal_flag)
        {
            if (decimal_flag)
            {
                return PerformADCDecimal(val, accumulator, carry_flag);
            }
            else
            {
                return PerformADCBinary(val, accumulator, carry_flag);
            }
        }

        private byte PerformADCDecimal(byte val, byte accumulator, bool carry_flag)
        {
            byte carry = (carry_flag ? 1 : 0);
            ushort tmp = (ushort) (val + accumulator + carry);
            if (((val & 0xF) + (accumulator & 0xF) + carry) > 9)
            {
                tmp += 6;
            }
            if (tmp > 0x99)
            {
                tmp += 96;
            }
            return (byte) (tmp & 0xFF);
        }

        private byte PerformADCBinary(byte val, byte accumulator, bool carry_flag)
        {
            return (byte) ((val + accumulator + (carry_flag ? 1 : 0)) & 0xFF);
        }

        private bool GetADCNegative(byte val, byte accumulator, bool carry, bool decimal_flag)
        {
            if (decimal_flag)
            {
                return GetADCNegativeDecimal(val, accumulator, carry);
            }
            else
            {
                return GetADCNegativeBinary(val, accumulator, carry);
            }
        }
        private bool GetADCNegativeDecimal(byte val, byte accumulator, bool carry_flag)
        {
            byte carry = (carry_flag ? 1 : 0);
            ushort tmp = (ushort)(val + accumulator + carry);
            if (((val & 0xF) + (accumulator & 0xF) + carry) > 9)
            {
                tmp += 6;
            }
            return (tmp & 0x80) > 0;
        }
        private bool GetADCNegativeBinary(byte val, byte accumulator, bool carry_flag)
        {
            return (PerformADCBinary(val, accumulator, carry_flag) & 0x80) > 0;
        }


        private bool GetADCZero(byte val, byte accumulator, bool carry_flag, bool decimal_flag)
        {
            return PerformADCBinary(val, accumulator, carry_flag) == 0;
        }
    }
}