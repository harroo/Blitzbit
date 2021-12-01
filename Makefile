
examples:
	mcs BlitClient/*.cs Examples/ClientExample.cs -out:ClientExample.out
	mcs BlitClient/*.cs Examples/ClientExampleII.cs -out:ClientExampleII.out
	mcs BlitServer/*.cs Examples/ServerExample.cs -out:ServerExample.out
	mcs BlitServer/*.cs Examples/ServerExampleII.cs -out:ServerExampleII.out
	mcs BlitHybrid/*.cs Examples/HybridExample.cs -out:HybridExample.out
	mcs BlitStream/*.cs Examples/StreamExample.cs -out:StreamExample.out
	mcs BlitPacket/*.cs Examples/PacketExample.cs -out:PacketExample.out
	mcs UBlitClient/*.cs Examples/UBlitClientExample.cs -out:UBlitClientExample.out
	mcs UBlitServer/*.cs Examples/UBlitServerExample.cs -out:UBlitServerExample.out
	mcs UBlitHybrid/*.cs Examples/UBlitHybridExample.cs -out:UBlitHybridExample.out

dll:
	mcs -t:library Blit*/*.cs UBlit*/*.cs -out:Blitzbit_vX.dll
