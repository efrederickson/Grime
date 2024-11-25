namespace Grime.Core.Executor
{
    /// <summary>
    /// Grime Math (GMath) to avoid conflicts with builtin math lib
    /// </summary>
    public class GMath
    {
        /// <summary>
        /// Convert unsigned byte to signed byte via Two's Complement
        /// </summary>
        public static sbyte UnsignedToSigned(byte value)
        {
            if ((value & 0b10000000) == 0)
            {
                return (sbyte)value; // Positive number
            }

            return (sbyte)((value & 0b01111111) - (value & 0b10000000));
        }

        /// <summary>
        /// Convert signed byte to unsigned byte via two's complement. This doesn't work, but also isn't used
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static byte SignedToUnsigned(sbyte value)
        {
            unchecked
            {
                if (value >= 0)
                {
                    return (byte)value;
                }

                return (byte)((byte)(~value) + 1);
            }
        }

        /// <summary>
        /// Convert unsigned byte to signed byte via Two's Complement
        /// </summary>
        public static int UnsignedToSigned(uint value)
        {
            unchecked
            {
                if ((value & 0x8000) == 0)
                {
                    return (int)value; // Positive number
                }

                var r = (int)((value & 0x7FFF) - (value & 0x8000));
                return r;
            }
        }
    }
}
