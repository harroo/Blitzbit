
examples:
	mcs BlitClient/*.cs Example/ClientExample.cs -out:ClientExample.out
	mcs BlitServer/*.cs Example/ServerExample.cs -out:ServerExample.out

dll:
	mcs -t:library Blit*/*.cs -out:Blitzbit_vX.dll
