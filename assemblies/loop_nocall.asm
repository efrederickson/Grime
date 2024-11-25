section .text
global _start

_start: ; entrypoint
    nop
    jmp _loop

_loop:
    ; write
    jmp _write

_inc:
    mov eax, [accum]
    add eax, 1 ; add one
    mov [accum], eax ; store it
    jmp _check

_check:
    ; check if done
    ; eax is already accum -> mov eax, accum
    mov eax, [accum]
    mov ebx, [count]
    cmp eax, ebx
    jz _exit
    jmp _loop

_write:
    mov edx, 1 ; len to write
    mov ecx, letter ; addr to write from
    mov ebx, 1 ; stdout
    mov eax, 4 ; syscall for write
    int 0x80 ; invoke syscall
    jmp _inc

_exit:
    mov eax, 1 ; syscall for exit
    int 0x80 ; syscall

section .data
    letter db 69 ; 'E'
    count dd 5 ; 5 repetitions
    accum dd 0 ; counter
