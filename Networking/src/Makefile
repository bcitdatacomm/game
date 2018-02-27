CC= g++

NAME=Library
FLAGS= -std=c++11 -Wall -ggdb -pedantic -c -fPIC
LINK= -shared 


server.o:
	$(CC) $(FLAGS) server.cpp 
library.o:
	$(CC) $(FLAGS) library.cpp

make library: server.o library.o
	$(CC) -shared library.o server.o -o libNetwork.so

