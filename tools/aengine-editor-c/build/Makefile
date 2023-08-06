.PHONY: all windows mac linux

all: windows mac linux

windows:
	g++ ../src/main.cpp -o main.exe -O2 -Wall -Wno-missing-braces -Wno-enum-compare -I ../include/ -L ../lib/libW/ -lraylib -lopengl32 -lgdi32 -lwinmm

mac:
	clang++ -framework CoreVideo -framework IOKit -framework Cocoa -framework GLUT -framework OpenGL ../lib/libM/libraylib.a ../src/main.cpp -o main -I ../include/ -L ../lib/libM -lraylib

linux:
	g++ ../src/main.cpp -o main -O2 -Wall -Wno-missing-braces -Wno-enum-compare -I ../include/ -L ../lib/libL -lraylib -lm -lpthread -ldl -lrt -lX11