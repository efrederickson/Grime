# GRIME

Grime is a (massively incomplete) x86_64 emulator, purely for the purpose of teaching myself assembly and how
the x86 architecture works. Turns out Intel x86_64 ~~sucks~~ is necessarily very complicated.

## What Is Implemented

At least at a very basic level:

- ELF64 reader
- The necessary instructions (and/or hardcoded workarounds) for interpreting some basic asm progs
- Unit tests for very basic elf parsing

Just like every other project I start and abandon - no promises this will make it beyond <point in time>.
It's incredibly lazy in some spots (syscalls) - but that's not the point.
