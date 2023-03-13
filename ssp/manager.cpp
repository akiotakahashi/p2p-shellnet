#include "stdafx.h"
#include "ssp.h"
#include "manager.h"
#include "datatransport.h"
#include "nameresolution.h"


extern SOCKET NewSocket(DWORD categoryEntryId, const WSPUPCALLTABLE& UpcallTable, AddressFamily af, SocketType st, ProtocolType pt, ISocket* socket, int* lpErrno) {
	SOCKETDESC* sd = new SOCKETDESC();
	sd->categoryEntryId = categoryEntryId;
	sd->peerId = Manager::peer->PeerId;
	sd->socketId = socket->SocketId;
	sd->addressFamily = af;
	sd->protocolType = pt;
	SOCKET s = WPUCreateSocketHandle(categoryEntryId,reinterpret_cast<DWORD>(sd),lpErrno);
	if(s==INVALID_SOCKET) {
		delete sd;
		Console::Error->WriteLine(S"WPUCreateSocketHandle returns the invaild pointer.");
		*lpErrno = WSAEFAULT;
		return INVALID_SOCKET;
	} else {
		Manager::sockets->Add(__box(sd->socketId), socket);
		sd->s = s;
		Manager::peer->Log->WriteLine(S"ssp generates socket#{0}",__box((unsigned int)s));
		return s;
	}
}

extern SOCKET NewSocket(DWORD categoryEntryId, const WSPUPCALLTABLE& UpcallTable, AddressFamily af, SocketType st, ProtocolType pt, int* lpErrno) {
	ISocket* socket = Manager::peer->CreateSocket(af2(af),st2(st),pt2(pt));
	if(socket==NULL) {
		Console::Error->WriteLine(S"Peer::CreateSocket returns NULL");
		*lpErrno = WSAENETDOWN;
		return INVALID_SOCKET;
	} else {
		return NewSocket(categoryEntryId,UpcallTable,af,st,pt,socket,lpErrno);
	}
}

extern const SOCKETDESC* GetSocketDesc(const WSPUPCALLTABLE& UpcallTable, SOCKET s) {
	int errnum;
	SOCKETDESC* sd = NULL;
	WPUQuerySocketHandleContext(s,(PDWORD_PTR)&sd,&errnum);
	return sd;
}

extern ISocket* GetSocket(int peerId, int socketId) {
	Hashtable* sockets = static_cast<Hashtable*>(Manager::socketlists->get_Item(__box(peerId)));
	return static_cast<ISocket*>(sockets->get_Item(__box(socketId)));
}


void Manager::KeepaliveCallback(Object* sender, EventArgs* e) {
	Console::Error->WriteLine("begin keepalive infine loop");
	while(true) {
		Thread::Sleep(60*1000);
	}
}

void Manager::Register(IPeer* peer) {
	peerids->set_Item(__box(peer->PeerId),peer);
	socketlists->set_Item(__box(peer->PeerId), new Hashtable());
}

void Manager::SetActivePeer(int peerid) {
	peer = static_cast<IPeer*>(peerids->get_Item(__box(peerid)));
	host = peer->Host;
	sockets = static_cast<Hashtable*>(socketlists->get_Item(__box(peerid)));
}
