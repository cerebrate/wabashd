.PHONY: wabashd.cs

all: wabashd

wabashd: wabashd.cs
	mcs -out:../pkg-debian/usr/bin/wabashd.exe -r:System.ServiceProcess.dll -r:Mono.Posix.dll *.cs



