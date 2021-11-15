
examples:
	mcs BlitClient/*.cs Examples/ClientExample.cs -out:ClientExample.out
	mcs BlitServer/*.cs Examples/ServerExample.cs -out:ServerExample.out
	mcs BlitHybrid/*.cs Examples/HybridExample.cs -out:HybridExample.out
	mcs BlitClient/*.cs Examples/SecondClientExample.cs -out:SecondClientExample.out
	mcs BlitStream/*.cs Examples/StreamExample.cs -out:StreamExample.out

dll:
	mcs -t:library Blit*/*.cs -out:Blitzbit_vX.dll
