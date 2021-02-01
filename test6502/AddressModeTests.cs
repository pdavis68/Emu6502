using cpu6502;
using Xunit;

namespace test6502
{
    public class ADdressModeTests : CPUTest
    {
        [Fact]
        public void AddressMode_ACC()
        {
            CPU cpu = new CPU();
            cpu.RAM = GetCleanRam();

            Assert.Equal(0, cpu.AddressMode_ACC());
        }

        [Fact]
        public void AddressMode_IMM()
        {
            CPU cpu = new CPU();
            cpu.RAM = GetCleanRam();
            ushort ip = cpu.InstructionPointer;

            Assert.Equal(ip, cpu.AddressMode_IMM());
            Assert.Equal(ip + 1, cpu.InstructionPointer);
        }

        [Theory]
        [InlineData(0x400, 0x400)]
        [InlineData(0x1400, 0xD400)]
        [InlineData(0x0000, 0xFFFE)]
        [InlineData(0xFFFE, 0x0000 )]
        public void AddressMode_ABS(ushort ipAddr, ushort refAddr)
        {
            CPU cpu = new CPU();
            cpu.RAM = GetCleanRam();
            cpu.InstructionPointer = ipAddr;
            cpu.RAM[ipAddr] = (byte) (refAddr % 256);
            cpu.RAM[ipAddr + 1] = (byte) (refAddr / 256);

            var result = cpu.AddressMode_ABS();
            Assert.Equal(refAddr, result);
            Assert.Equal((ushort)(ipAddr + 2), cpu.InstructionPointer);
        }


        /// <summary>
        ///  Don't set ipAddr to page 0.
        /// </summary>
        /// <param name="val"></param>
        /// <param name="ipAddr"></param>
        [Theory]
        [InlineData(0x00, 0x4212)]
        [InlineData(0xFF, 0xFFFE)]
        [InlineData(0x80, 0x0300)]
        [InlineData(0xCC, 0x2220)]
        public void AddressMode_ZER(byte val, ushort ipAddr)
        {
            CPU cpu = new CPU();
            cpu.RAM = GetCleanRam();
            cpu.InstructionPointer = ipAddr;
            cpu.RAM[ipAddr] = val;
            for(int index = 0; index < 256; index++)
            {
                cpu.RAM[index] = (byte) index;
            }

            var result = cpu.AddressMode_ZER();
            Assert.Equal(val, result);
            Assert.Equal(ipAddr + 1, cpu.InstructionPointer);
        }

        [Fact]
        public void AddressMode_IMP()
        {
            CPU cpu = new CPU();
            cpu.RAM = GetCleanRam();

            Assert.Equal(0, cpu.AddressMode_IMP());
        }

        /// <summary>
        ///  Don't set ipAddr to page 0.
        /// </summary>
        /// <param name="val"></param>
        /// <param name="ipAddr"></param>
        [Theory]
        [InlineData(0x0000, 0x4212)]
        [InlineData(0x2FFE, 0xFFFE)]
        [InlineData(0x8043, 0x0300)]
        [InlineData(0xCCFF, 0x2220)]
        public void AddressMode_ABI(ushort refAddr, ushort ipAddr)
        {
            CPU cpu = new CPU();
            cpu.RAM = GetCleanRam();
            cpu.InstructionPointer = ipAddr;
            cpu.RAM[ipAddr] = (byte)(refAddr % 256);
            cpu.RAM[ipAddr + 1] = (byte)(refAddr / 256);
            cpu.RAM[refAddr] = (byte)0xF0;
            cpu.RAM[refAddr + 1] = (byte)0x0F;

            var result = cpu.AddressMode_ABI();
            Assert.Equal(0x0FF0, result);
            Assert.Equal((ushort) (ipAddr + 2), cpu.InstructionPointer);
        }
    }
}