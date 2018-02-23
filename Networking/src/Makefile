CC= g++
NAME=Server
FLAGS= -std=c++11 -Wall -ggdb -pedantic

Connection.o:
	$(CC) $(FLAGS) Connection.cpp 


Server.o:
	$(CC) $(FLAGS) server.cpp

library:
	$(CC) -o Library.so server.cpp 
