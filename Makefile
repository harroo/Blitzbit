
examples:
	mcs BlitClient/*.cs Example/ClientExample.cs -out:ClientExample.out
	mcs BlitServer/*.cs Example/ServerExample.cs -out:ServerExample.out
	mcs BlitHybrid/*.cs Example/HybridExample.cs -out:HybridExample.out
	mcs BlitStream/*.cs Example/StreamExample.cs -out:StreamExample.out

dll:
	mcs -t:library Blit*/*.cs -out:Blitzbit_vX.dll
