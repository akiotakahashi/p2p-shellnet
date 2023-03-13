#include "stdafx.h"
#include "ws2_32.h"
#include "manager.h"


static void s2a(unsigned char a __gc[], char* buf, int size) {
	if(a->Length>=size) return;
	unsigned char __pin* p = &a[0];
	memcpy(buf,p,a->Length);
	p[a->Length] = '\0';
}

char* s2a(String* s) {
	if(s==NULL) {return NULL;}
	unsigned char a __gc[] = System::Text::Encoding::Default->GetBytes(s);
	char* buf = new char[a->Length+1];
	s2a(a,buf,a->Length+1);
	return buf;
}

void s2a(String* s, char* buf, int size) {
	if(s==NULL) {buf[0]='\0'; return;}
	unsigned char a __gc[] = System::Text::Encoding::Default->GetBytes(s);
	s2a(a,buf,size);
}


void Manager::KeepaliveCallback(Object* sender, EventArgs* e) {
	Console::Error->WriteLine("begin keepalive infine loop");
	while(true) {
		Thread::Sleep(60*1000);
	}
}

void Manager::Register(IPeer* peer) {
	peerids->set_Item(__box(peer->PeerId),peer);
}

SOCKET Manager::newSocket(ISocket* socket) {
	setSocket(socket);
	return socket->SocketId;
}

ISocket* Manager::getSocket(SOCKET s) {
	Monitor::Enter(itos->SyncRoot);
	try {
		return static_cast<ISocket*>(itos->get_Item(__box(s)));
	} __finally {
		Monitor::Exit(itos->SyncRoot);
	}
}

void Manager::setSocket(ISocket* socket) {
	Monitor::Enter(itos->SyncRoot);
	try {
		itos->set_Item(__box(socket->SocketId),socket);
	} __finally {
		Monitor::Exit(itos->SyncRoot);
	}
}
