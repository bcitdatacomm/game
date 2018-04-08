#!/bin/bash

echo "Switching folder..."
cd Networking/src

echo "Running make..."
make clean
make library

echo "Returning.."
cd ../..
