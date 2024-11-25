PROG=$1

if [[ "$PROG" == "" ]]; then
  echo "Program without ext required as argument 1";
  exit 1;
fi

nasm -f elf64 -o $PROG.o $PROG.asm
ld -o $PROG $PROG.o
