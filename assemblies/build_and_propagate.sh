#!/bin/bash

# Build 
for prog in hello loop_nocall loop; do
  echo Building $prog
  ./build.sh $prog
  echo Copying $prog to Interpreter
  cp $prog ../Grime.Interpreter/fixtures/
done

