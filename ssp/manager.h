#pragma once


__gc class Manager {
public:
	static String* KeepaliveMutexName;
	static System::Threading::Mutex* KeepaliveMutex;
	static IShellnet* shellnet;
	static IHost* host;
	static IPeer* peer;
	static Hashtable* itos;
	static Hashtable* peerids;
	static Manager() {
		KeepaliveMutexName = Guid::NewGuid().ToString();
		KeepaliveMutex = new System::Threading::Mutex(true,KeepaliveMutexName);
		peerids = Hashtable::Synchronized(new Hashtable());
		sockets = Hashtable::Synchronized(new Hashtable());
		socketlists = Hashtable::Synchronized(new Hashtable());
	}
	static void KeepaliveCallback(Object* sender, EventArgs* e);
	static void Register(IPeer* peer);
	static void SetActivePeer(int peerid);
	static Hashtable* socketlists;
	static Hashtable* sockets;
};

struct SOCKETDESC {
	DWORD categoryEntryId;
	SOCKET s;
	int peerId;
	int socketId;
	int addressFamily;
	int socketType;
	int protocolType;
	mutable HWND hAsyncSelectWnd;
	mutable WORD wAsyncSelectMsg;
	mutable int fdAsyncSelectFilter;
	SOCKETDESC() {
		ZeroMemory(this,sizeof(*this));
	}
	void PostAsyncMessage(const WSPUPCALLTABLE& upcalls, int lEvent) const {
		if(hAsyncSelectWnd==NULL) return;
		if(0==(fdAsyncSelectFilter&lEvent)) return;
        upcalls.lpWPUPostMessage(hAsyncSelectWnd,wAsyncSelectMsg,(WPARAM)s,(LPARAM)(lEvent));
	}
};

extern SOCKET NewSocket(DWORD categoryEntryId, const WSPUPCALLTABLE& UpcallTable, AddressFamily af, SocketType st, ProtocolType pt, ISocket* socket, int* lpErrno);
extern SOCKET NewSocket(DWORD categoryEntryId, const WSPUPCALLTABLE& UpcallTable, AddressFamily af, SocketType st, ProtocolType pt, int* lpErrno);
extern const SOCKETDESC* GetSocketDesc(const WSPUPCALLTABLE& UpcallTable, SOCKET s);
extern ISocket* GetSocket(int peerId, int socketId);
