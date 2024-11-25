namespace Grime.Core.Executor
{
    public class Page64
    {
        public byte[] Data { get; set; }
        public ulong VirtualAddress { get; set; }
        public ulong Size { get; set; }

        public ulong Alignment { get; set; }

        public PFlags Flags { get; set; }

        public Page64(byte[] data, ulong size, ulong virtualAddress, ulong alignment, PFlags flags)
        {
            Data = data;
            Size = size;
            VirtualAddress = virtualAddress;
            Alignment = alignment;
            Flags = flags;
        }

        public byte[] Read(ulong offset, uint size)
        {
            if ((Flags & PFlags.R) != PFlags.R)
            {
                throw new AccessViolationException($"Address {offset} is not in a page marked R");
            }

            var pageOffset = offset - VirtualAddress;
            return new ArraySegment<byte>(Data, (int)pageOffset, (int)size).ToArray(); // FIXME: do not copy array
        }

        public void Write(ulong offset, ulong size, byte[] data)
        {
            if ((Flags & PFlags.W) != PFlags.W)
            {
                throw new AccessViolationException($"Address {offset} is not in a page marked W");
            }

            Array.Copy(data, 0, Data, (int)(offset - VirtualAddress), (int)size);
        }

        public unsafe byte* PointerAt(ulong address)
        {
            if ((Flags & PFlags.R) != PFlags.R)
            {
                throw new AccessViolationException($"Address {address} is not in a page marked R");
            }

            var realAddress = address - VirtualAddress;
            //Console.Write($"ptr at {address:X}: {realAddress:X} ");
            fixed (byte* ptr = &Data[0])
            {
                //Console.WriteLine($"{*(ptr + realAddress)}");
                return (ptr + realAddress);
            }
        }
    }
}
