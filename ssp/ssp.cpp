#include "stdafx.h"
#include "ssp.h"
#include "manager.h"


extern void s2w(String* s, wchar_t* buf, int len) {
	if(s==NULL || len<s->Length+1) {buf[0]='\0'; return;}
	for(int i=0; i<len; ++i) {
		buf[i] = s->Chars[i];
	}
}

static void s2a(unsigned char a __gc[], char* buf, int len) {
	if(a->Length>=len) return;
	unsigned char __pin* p = &a[0];
	memcpy(buf,p,a->Length);
	p[a->Length] = '\0';
}

extern char* s2a(String* s) {
	if(s==NULL) {return NULL;}
	unsigned char a __gc[] = System::Text::Encoding::Default->GetBytes(s);
	char* buf = new char[a->Length+1];
	s2a(a,buf,a->Length+1);
	return buf;
}

extern void s2a(String* s, char* buf, int size) {
	if(s==NULL) {buf[0]='\0'; return;}
	unsigned char a __gc[] = System::Text::Encoding::Default->GetBytes(s);
	s2a(a,buf,size);
}


extern IPEndPoint* getEndPoint(const sockaddr* addr, int addrlen) {
	unsigned char buf __gc[];
	unsigned char __pin* p;
	switch(addr->sa_family) {
	case AF_UNSPEC:
		return NULL;
	case AF_INET:
		if(addrlen>0 && addrlen<sizeof(sockaddr_in)) return NULL;
		sockaddr_in* sa4;
		sa4 = (sockaddr_in*)addr;
		buf = new unsigned char __gc[4];
		p = &buf[0];
		memcpy(p,&sa4->sin_addr,buf->Length);
		return new IPEndPoint(Manager::host->GetIPAddress(*(unsigned long*)p),ntohs(sa4->sin_port));
	case AF_INET6:
		if(addrlen>0 && addrlen<sizeof(sockaddr_in6)) return NULL;
		sockaddr_in6* sa6;
		sa6 = (sockaddr_in6*)addr;
		buf = new unsigned char __gc[16];
		p = &buf[0];
		memcpy(p,&sa6->sin6_addr,buf->Length);
		return new IPEndPoint(Manager::host->GetIPAddress(buf,sa6->sin6_scope_id),ntohs(sa6->sin6_port));
	default:
		throw new System::Net::Sockets::SocketException(WSAEAFNOSUPPORT);
	}
}

int getSockAddr(IPEndPoint* ep, sockaddr* sa, int* salen) {
	unsigned char __pin* p;
	switch(ep->AddressFamily) {
	case AddressFamily::InterNetwork:
		if(*salen<sizeof(sockaddr_in)) return SOCKET_ERROR;
		*salen = sizeof(sockaddr_in);
		sockaddr_in* sa4;
		sa4 = (sockaddr_in*)sa;
		sa4->sin_family = AF_INET;
		p = &ep->Address->GetAddressBytes()[0];
		memcpy(&sa4->sin_addr,p,4);
		sa4->sin_port = htons(ep->Port);
		return 0;
	case AddressFamily::InterNetworkV6:
		if(*salen<sizeof(sockaddr_in6)) return SOCKET_ERROR;
		*salen = sizeof(sockaddr_in6);
		sockaddr_in6* sa6;
		sa6 = (sockaddr_in6*)sa;
		sa6->sin6_family = AF_INET6;
		p = &ep->Address->GetAddressBytes()[0];
		memcpy(&sa6->sin6_addr,p,16);
		sa6->sin6_port = htons(ep->Port);
		sa6->sin6_flowinfo = 0;
		sa6->sin6_scope_id = htonl((u_long)ep->Address->ScopeId);
		return 0;
	default:
		return SOCKET_ERROR;
	}
}

BOOL APIENTRY DllMain(HANDLE hModule, DWORD  ul_reason_for_call, LPVOID lpReserved) {
	switch (ul_reason_for_call) {
	case DLL_PROCESS_ATTACH:
		Console::WriteLine("shellnet service provider is attached to process");
		{
		DWORD iosize;
		wchar_t szExecutablePath[MAX_PATH];
		GetModuleFileName(GetModuleHandle(NULL),szExecutablePath,sizeof(szExecutablePath));
		std::wstring szLogFilePath = L"C:\\ssp.log";
		HANDLE hFile = CreateFile(szLogFilePath.c_str(),GENERIC_READ|GENERIC_WRITE,FILE_SHARE_READ,NULL,OPEN_ALWAYS,FILE_ATTRIBUTE_NORMAL,NULL);
		SetFilePointer(hFile,0,NULL,FILE_END);
		WriteFile(hFile,szLogFilePath.c_str(),(int)wcslen(szExecutablePath)*sizeof(wchar_t),&iosize,NULL);
		WriteFile(hFile,L"\r\n",4,&iosize,NULL);
		CloseHandle(hFile);
		/*
		wchar_t szModulePath[MAX_PATH];
		GetModuleFileName((HMODULE)hModule,szModulePath,sizeof(szModulePath));
		String* privatepath = System::IO::Path::GetDirectoryName(new String(szModulePath));
		Console::WriteLine(String::Concat(S"ssp registers \"",privatepath,"\" as a private path"));
		System::AppDomain::CurrentDomain->AppendPrivatePath(privatepath);
		*/
		}
		return TRUE;
	case DLL_THREAD_ATTACH:
		return TRUE;
	case DLL_THREAD_DETACH:
		return TRUE;
	case DLL_PROCESS_DETACH:
		Console::WriteLine("shellnet service provider is detached from process");
		return TRUE;
	default:
		return FALSE;
	}
}


#undef GetObject

extern int Initialize() {

	static bool initialized = false;
	if(initialized) return 0;

	initialized = true;

	try {

		Console::Write(S"registering pipe channel...");
		IChannel* chnl = __gc new Pipe::PipeChannel();
		ChannelServices::RegisterChannel(chnl);
		Console::WriteLine(S"ok");
	
		Console::Write(S"activating the shellnet...");
		Manager::shellnet = static_cast<IShellnet*>(Activator::GetObject(
			__typeof(IShellnet),"pipe://shellnet/shellnet"));

		if(Manager::shellnet==NULL) {
			Console::WriteLine(S"failed");
			Console::WriteLine(S"the shellnet instance is not running or published.");
			return -1;
		} else {
			Console::WriteLine(S"ok");
		}

		Console::Write(S"initializing...");
		//Manager::peer = Manager::shellnet->Startup(Manager::KeepaliveMutexName);
		IPeer* peer = static_cast<IPeer*>(Manager::shellnet->Startup(
			(int)GetCurrentProcessId(), Manager::KeepaliveMutexName));
		Console::Write(peer->GetType()->ToString());
		Console::WriteLine(S"...ok");
	
		Console::Write(S"setting up...");
		Manager::Register(peer);
		Manager::SetActivePeer(peer->PeerId);
		Console::WriteLine(S"ok");

		Console::WriteLine(S"peer#{0} started",__box(Manager::peer->PeerId));

		return 0;
	
	} catch(Exception* ex) {
		Console::WriteLine("failed");
		Console::Error->WriteLine(ex);
		return -1;
	} catch(...) {
		return -1;
	}

}
