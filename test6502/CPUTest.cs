using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace test6502
{
    public class CPUTest
    {

        protected static byte[] GetCleanRam()
        {
            int size = 64 * 1024;
            byte[] ram = new byte[size];
            for (int index = 0; index < size; index++)
            {
                ram[index] = 0;
            }
            return ram;
        }

        public static byte[] GetProgramData(string hex)
        {
            hex = hex.Replace(" ", "");
            return Enumerable.Range(0, hex.Length)
                             .Where(x => x % 2 == 0)
                             .Select(x => Convert.ToByte(hex.Substring(x, 2), 16))
                             .ToArray();
        }
    }
}
