namespace Grime.Core.Executor
{
    public readonly record struct PageKey(ulong VirtualAddress, ulong Size)
    {
        public static implicit operator (ulong VirtualAddress, ulong Size)(PageKey value) => (value.VirtualAddress, value.Size);
        public static implicit operator PageKey((ulong VirtualAddress, ulong Size) value) => new(value.VirtualAddress, value.Size);
    }

    /// <summary>
    /// Handle the mapping into and out of a set of virtual addresses 
    /// </summary>
    public class VirtualMemory64
    {
        // Tuple<vaddr, memsz>
        public Dictionary<PageKey, Page64> Pages { get; private set; } = [];

        public VirtualMemory64(Page64[] pages) => Map(pages);

        /// <summary>
        /// Replace the existing pages with the new ones
        /// </summary>
        /// <param name="pages"></param>
        public void Map(Page64[] pages)
        {
            Pages.Clear();
            foreach (var page in pages)
            {
                Pages[(page.VirtualAddress, page.Size)] = page;
            }
        }

        /// <summary>
        /// Determine the page that contains `address`
        /// </summary>
        /// <param name="address">The address to lookup the page for</param>
        /// <returns></returns>
        public PageKey LookupPage(ulong address)
        {
            foreach ((var vaddr, var memsz) in Pages.Keys)
            {
                // vaddr <= address <= vaddr+memsz
                // TODO: does it fall within the page if it's less than vaddr+alignment?
                if (vaddr <= address && address <= vaddr + memsz)
                {
                    return (vaddr, memsz);
                }
            }
            throw new AddressOutOfBoundsException($"Address {address:X} does not exist in this virtual memory");
        }

        public Page64 GetPage(ulong address) => Pages[LookupPage(address)];
        public byte[] Read(ulong offset, uint size) => GetPage(offset).Read(offset, size);
        public void Write(ulong address, byte[] data, ulong size) => GetPage(address).Write(address, size, data);
    }
}
