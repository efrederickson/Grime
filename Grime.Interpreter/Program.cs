﻿using Grime.Core;
using Grime.Core.Executor;

namespace Grime.Interpreter
{
    internal class Program
    {
        static void Main(string[] args)
        {
            foreach (var file in Directory.GetFiles("fixtures"))
            {
                Console.WriteLine($"Processing {file}");
                var s = new FileStream(file, FileMode.Open);
                analyze(s);
                s.Close();
            }
        }

        static void analyze(Stream s)
        {
            Console.WriteLine("\nStart analyzing\n");
            var elf = ELFReader.Load(s);

            var offset = elf.Header.e_shoff + (ulong)(elf.Header.e_shstrndx * elf.Header.e_shentsize);
            s.Seek((long)offset, SeekOrigin.Begin);
            var shstrtab = ELFReader.ReadSectionHeader64(s);

            // Section names from .shstrtab
            for (int i = 0; i < elf.SectionHeaders.Length; i++)
            {
                var section = elf.SectionHeaders[i];
                var stroffset = shstrtab.offset + section.name;
                s.Seek((long)stroffset, SeekOrigin.Begin);
                Console.WriteLine($"Section {i} {section.name}: {ELFReader.ReadString(s)}");
            }

            // .symtab and .strtab
            ELFReader.ReadSymTable(elf, s);

            // Read segments
            var pages = ELFReader.ReadAndAlignSegments(elf, s);
            // Append a page at (what can only be assumed to be the end) to be used for the stack - otherwise i overwrote the stack in fixtures/loop
            pages = [.. pages, new Page64(new byte[4096], 4096, 0xFFFFFFFF, 0, PFlags.R | PFlags.W)];

            // This should go into reader?
            var vmem = new VirtualMemory64(pages);

            // Create cpu
            var cpu = new CPU(vmem, elf.Header.e_entry);

            // Execute until completion or exception
            while (cpu.Cycle())
            {

            }
            Console.WriteLine("\nDone analyzing\n");
        }
    }
}