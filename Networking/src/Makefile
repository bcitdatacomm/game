CC= g++

NAME=Library
FLAGS= -std=c++11 -Wall -ggdb -pedantic -c -fPIC
LINK= -shared 


client.o:
	$(CC) $(FLAGS) client.cpp

server.o:
	$(CC) $(FLAGS) server.cpp 
library.o:
	$(CC) $(FLAGS) library.cpp

make library: server.o library.o client.o
	$(CC) -shared library.o server.o client.o -o libNetwork.so

