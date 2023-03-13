#pragma once


inline Array* array(Object* arr __gc[]) {
	Array* wrap = arr;
	return wrap;
}

extern char* s2a(String* s);
extern wchar_t* s2w(String* s);
extern void s2a(String* s, char* buf, int size);
extern void s2w(String* s, wchar_t* buf, int size);


__gc class Manager {
public:
	static String* KeepaliveMutexName;
	static Mutex* KeepaliveMutex;
	static IShellnet* shellnet;
	static IHost* host;
	static IPeer* peer;
	static Hashtable* itos;
	static Hashtable* peerids;
	static Manager() {
		KeepaliveMutexName = Guid::NewGuid().ToString();
		KeepaliveMutex = new System::Threading::Mutex(true,KeepaliveMutexName);
		itos = new Hashtable();
		peerids = new Hashtable();
	}
	static void KeepaliveCallback(Object* sender, EventArgs* e);
	static void Register(IPeer* peer);
	static SOCKET newSocket(ISocket* socket);
	static ISocket* getSocket(SOCKET s);
	static void setSocket(ISocket* socket);
};
