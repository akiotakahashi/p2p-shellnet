#include "stdafx.h"
#include "ssp.h"
#include "manager.h"
#define LINKAGE __declspec(dllexport)
#include "extensions.h"


/*
SNGetCurrentPeerId
SNCreateNewPeer
SNActivatePeer
SNShutdownPeer
*/

extern "C" __declspec(dllexport) BOOL SNPostMessage(void* fp, HWND hWnd, UINT Msg, WPARAM wParam, LPARAM lParam) {
	return ((BOOL(*)(HWND,UINT,WPARAM,LPARAM))fp)(hWnd,Msg,wParam,lParam);
}

extern "C" __declspec(dllexport) int SNGetCurrentPeerId() {
	Initialize();
	if(Manager::peer==NULL) return -1;
	return Manager::peer->PeerId;
}

extern "C" __declspec(dllexport) int SNCreatePeer() {
	Initialize();
	IPeer* peer = Manager::shellnet->Startup((int)GetCurrentProcessId(),Manager::KeepaliveMutexName);
	Manager::Register(peer);
	return peer->PeerId;
}

extern "C" __declspec(dllexport) int SNActivatePeer(int peerid) {
	Initialize();
	if(!Manager::peerids->ContainsKey(__box(peerid))) {
		return -1;
	} else {
		int oldid = -1;
		if(Manager::peer!=NULL) Manager::peer->PeerId;
		Manager::SetActivePeer(peerid);
		return oldid;
	}
}

extern "C" __declspec(dllexport) void SNShutdownPeer(int peerid) {
	Initialize();
	if(Manager::peerids->ContainsKey(__box(peerid)) && peerid!=Manager::peer->PeerId) {
		IPeer* peer = static_cast<IPeer*>(Manager::peerids->get_Item(__box(peerid)));
		peer->Cleanup();
		Manager::peerids->Remove(__box(peerid));
	}
}
