namespace Grime.Core.Executor
{
    /// <summary>
    /// Flags register values.
    /// https://en.wikipedia.org/wiki/FLAGS_register
    /// </summary>
    public enum RFlags : ulong
    {
        CARRY_FLAG_ = 0b0000000000000001,
        RES________ = 0b0000000000000010,
        PARITY_FLAG = 0b0000000000000100,
        RES2_______ = 0b0000000000001000,
        AUX_CARRY_F = 0b0000000000010000,
        RES3_______ = 0b0000000000100000,
        ZERO_FLAG__ = 0b0000000001000000,
        SIGN_FLAG__ = 0b0000000010000000,
        TRAP_FLAG__ = 0b0000000100000000,
        INT_ENABLE_ = 0b0000001000000000,
        DIRECTION_F = 0b0000010000000000,
        OVERFLOW_F_ = 0b0000100000000000,
        IO_PRIV____ = 0b0001000000000000,
        NT_FLAG____ = 0b0010000000000000,
        MD_FLAG____ = 0b0100000000000000,
    }
}
