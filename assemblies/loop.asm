section .text
global _start

_start: ; entrypoint
    nop ; this is just to check i implemented the nop instr
    call _loop ; start loop

_loop:
    call _write ; write letter
    call _inc ; inc accum
    call _check ; check accum to count
    call _loop ; restart

_inc:
    mov eax, [accum]
    add eax, 1 ; add one
    mov [accum], eax ; store it
    ret

_check:
    ; check if done
    mov eax, [accum]
    mov ebx, [count]
    cmp eax, ebx
    jz _exit
    ret

_write:
    mov edx, 1 ; len to write
    mov ecx, letter ; addr to write from
    mov ebx, 1 ; stdout
    mov eax, 4 ; syscall for write
    int 0x80 ; invoke syscall
    ret

_exit:
    mov eax, 1 ; syscall for exit
    int 0x80 ; syscall

section .data
    letter db 69 ; 'E'
    count dd 5 ; 5 repetitions
    accum dd 0 ; counter
