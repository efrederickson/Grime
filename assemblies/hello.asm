section .text
global _start
_start:
    mov edx, len ; len to write
    mov ecx, msg ; data to write

    mov ebx, 1 ; stdout

    mov eax, 4 ; syscall for write
    int 0x80 ; syscall

    mov eax, 1 ; syscall for exit
    int 0x80 ; syscall

section .data
    msg db  "Hello world!", 10 ; hello world + newline
    len equ $ -msg ; length of msg
