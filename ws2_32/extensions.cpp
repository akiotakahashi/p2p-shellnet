#include "stdafx.h"
#include "ws2_32.h"
#include "manager.h"
#define LINKAGE __declspec(dllexport)
#include "extensions.h"


/*
SNGetCurrentPeerId
SNCreateNewPeer
SNActivatePeer
SNShutdownPeer
*/

extern "C" __declspec(dllexport) int SNGetCurrentPeerId() {
	return Manager::peer->PeerId;
}

extern "C" __declspec(dllexport) int SNCreatePeer() {
	IPeer* peer = Manager::shellnet->Startup(Manager::KeepaliveMutexName);
	Manager::Register(peer);
	return peer->PeerId;
}

extern "C" __declspec(dllexport) int SNActivatePeer(int peerid) {
	if(!Manager::peerids->ContainsKey(__box(peerid))) {
		return -1;
	} else {
		IPeer* peer = static_cast<IPeer*>(Manager::peerids->get_Item(__box(peerid)));
		int oldid = Manager::peer->PeerId;
		Manager::peer = peer;
		Manager::host = peer->Host;
		return oldid;
	}
}

extern "C" __declspec(dllexport) void SNShutdownPeer(int peerid) {
	if(Manager::peerids->ContainsKey(__box(peerid)) && peerid!=Manager::peer->PeerId) {
		IPeer* peer = static_cast<IPeer*>(Manager::peerids->get_Item(__box(peerid)));
		peer->Cleanup();
		Manager::peerids->Remove(__box(peerid));
	}
}
