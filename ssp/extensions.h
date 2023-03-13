#pragma once


#if !defined(LINKAGE)
#define LINKAGE
#endif

extern "C" LINKAGE int SNGetCurrentPeerId();
extern "C" LINKAGE int SNCreatePeer();
extern "C" LINKAGE int SNActivatePeer(int peerid);
extern "C" LINKAGE void SNShutdownPeer(int peerid);
