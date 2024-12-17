namespace Grime.Core.Tests
{
    [TestClass]
    public sealed class TestELFReader
    {
        [TestMethod]
        public void TestLoader()
        {
            var s = new FileStream("fixtures/hello", FileMode.Open);
            var elf = ELFReader.Load(s);

            // E_IDENT
            Assert.IsTrue(elf.Header.e_ident.EI_MAG0 == 0x7F, $"Expected 0x7F, got {elf.Header.e_ident.EI_MAG0}");
            Assert.IsTrue(elf.Header.e_ident.EI_MAG1 == 'E', $"Expected 'E', got {elf.Header.e_ident.EI_MAG1}");
            Assert.IsTrue(elf.Header.e_ident.EI_MAG2 == 'L', $"Expected 'L', got {elf.Header.e_ident.EI_MAG2}");
            Assert.IsTrue(elf.Header.e_ident.EI_MAG3 == 'F', $"Expected 'F', got {elf.Header.e_ident.EI_MAG3}");
            Assert.IsTrue(elf.Header.e_ident.EI_CLASS == EIClass.ELFCLASS64, $"Expected EI_CLASS to be {EIClass.ELFCLASS64}, got {elf.Header.e_ident.EI_CLASS}");
            Assert.IsTrue(elf.Header.e_ident.EI_DATA == EIData.LSB, $"Expected EI_DATA to be {EIData.LSB}, got {elf.Header.e_ident.EI_DATA}");
            Assert.IsTrue(elf.Header.e_ident.EI_VERSION == 1, $"Expected EI_VERSION to be 1, got {elf.Header.e_ident.EI_VERSION}");
            Assert.IsTrue(elf.Header.e_ident.EI_OSABI == EIOSABI.NONE, $"Expected EI_OSABI to be {EIOSABI.NONE}, got {elf.Header.e_ident.EI_OSABI}");
            Assert.IsTrue(elf.Header.e_ident.EI_ABIVERSION == 0, $"Expected EI_ABIVERSION to be 0, got {elf.Header.e_ident.EI_ABIVERSION}");
            Assert.IsTrue(elf.Header.e_ident.EI_PAD.SequenceEqual(new byte[7] { 0, 0, 0, 0, 0, 0, 0 }), $"Expected padding to be all zeroes, got {elf.Header.e_ident.EI_PAD}");

            // Other header fields
            Assert.IsTrue(elf.Header.e_type == EType.ET_EXEC, $"Expected E_TYPE to be {EType.ET_EXEC}, got {elf.Header.e_type}");
            Assert.IsTrue(elf.Header.e_machine == EMachine.EM_X86_64, $"Expected E_MACHINE to be {EMachine.EM_X86_64}, got {elf.Header.e_machine}");
            Assert.IsTrue(elf.Header.e_version == EVersion.EV_CURRENT, $"Expected E_VERSION to be {EVersion.EV_CURRENT}, got {elf.Header.e_version}");

            Assert.IsTrue(elf.Header.e_phentsize == 0x38, "Expected e_phentsize to be 0x38 (56)");
            Assert.IsTrue(elf.Header.e_phnum == 3, "Expected 3 program headers");
            Assert.IsTrue(elf.ProgramHeaders.Length == 3, "Expected 3 program headers to be read");

            Assert.IsTrue(elf.Header.e_shentsize == 0x40, "Expected e_shentsize to be 0x40 (64)");
            Assert.IsTrue(elf.Header.e_shnum == 6, "Expected 6 section headers");
            Assert.IsTrue(elf.SectionHeaders.Length == 6, "Expected 6 section headers to be read");

            // Program Headers
            Assert.IsTrue(elf.ProgramHeaders[0].type == PType.PT_LOAD, $"Expected pheader 0 type to be load, got {elf.ProgramHeaders[0].type}");
            Assert.IsTrue(elf.ProgramHeaders[0].flags == PFlags.R, $"Expected pheader 0 flags to be {PFlags.R}, got {elf.ProgramHeaders[0].flags}");

            Assert.IsTrue(elf.ProgramHeaders[1].type == PType.PT_LOAD, $"Expected pheader 1 type to be load, got {elf.ProgramHeaders[1].type}");
            Assert.IsTrue(elf.ProgramHeaders[1].flags == (PFlags.R | PFlags.X), $"Expected pheader 1 flags to be RX, got {elf.ProgramHeaders[1].flags}");

            Assert.IsTrue(elf.ProgramHeaders[2].type == PType.PT_LOAD, $"Expected pheader 2 type to be load, got {elf.ProgramHeaders[2].type}");
            Assert.IsTrue(elf.ProgramHeaders[2].flags == (PFlags.R | PFlags.W), $"Expected pheader 2 flags to be RW, got {elf.ProgramHeaders[2].flags}");

            // Section headers
            Assert.IsTrue(elf.SectionHeaders[0].type == SHType.SHT_NULL);
            Assert.IsTrue(elf.SectionHeaders[1].type == SHType.SHT_PROGBITS);
            Assert.IsTrue(elf.SectionHeaders[2].type == SHType.SHT_PROGBITS);
            Assert.IsTrue(elf.SectionHeaders[3].type == SHType.SHT_SYMTAB);
            Assert.IsTrue(elf.SectionHeaders[4].type == SHType.SHT_STRTAB);
            Assert.IsTrue(elf.SectionHeaders[5].type == SHType.SHT_STRTAB);
        }
    }
}
