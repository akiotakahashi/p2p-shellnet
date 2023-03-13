#include "stdafx.h"
#include "ws2_32.h"
#include "manager.h"


static int lasterror = 0;


#define WIDEN2(x) L ## x
#define WIDEN(x) WIDEN2(x)
#define seterr(ec) _seterr(ec, WIDEN(__FILE__), __LINE__, WIDEN(__FUNCTION__));
#define seterrif(cond,ec) _seterrif(cond,ec, WIDEN(__FILE__), __LINE__, WIDEN(__FUNCTION__));

inline static int _seterr(int errcode, const wchar_t* file, int line, const wchar_t* function) {
	WSASetLastError(errcode);
	return SOCKET_ERROR;
}

inline static int _seterrif(int cond, int errcode, const wchar_t* file, int line, const wchar_t* function) {
	if(cond) WSASetLastError(errcode);
	return cond;
}

static IPAddress* getIPAddress(const struct sockaddr* sa, int salen = -1) {
	unsigned char buf __gc[];
	unsigned char __pin* p;
	switch(sa->sa_family) {
	case PF_INET:
		if(salen>=0 && salen<sizeof(sockaddr_in)) return NULL;
		sockaddr_in* sa4;
		sa4 = (sockaddr_in*)sa;
		buf = new unsigned char __gc[4];
		p = &buf[0];
		memcpy(p,&sa4->sin_addr,4);
		return new IPAddress(buf);
	case PF_INET6:
		if(salen>=0 && salen<sizeof(sockaddr_in6)) return NULL;
		sockaddr_in6* sa6;
		sa6 = (sockaddr_in6*)sa;
		buf = new unsigned char __gc[16];
		p = &buf[0];
		memcpy(p,&sa6->sin6_addr,16);
		return new IPAddress(buf);
	default:
		return NULL;
	}
}

static IPEndPoint* getEndPoint(const sockaddr* addr, int addrlen) {
	unsigned char buf __gc[];
	unsigned char __pin* p;
	switch(addr->sa_family) {
	case AF_INET:
		sockaddr_in* sa4;
		sa4 = (sockaddr_in*)addr;
		buf = new unsigned char __gc[4];
		p = &buf[0];
		memcpy(p,&sa4->sin_addr,buf->Length);
		return new IPEndPoint(new IPAddress(buf),ntohs(sa4->sin_port));
	case AF_INET6:
		sockaddr_in6* sa6;
		sa6 = (sockaddr_in6*)addr;
		buf = new unsigned char __gc[16];
		p = &buf[0];
		memcpy(p,&sa6->sin6_addr,buf->Length);
		IPAddress* tmp;
		tmp = new IPAddress(buf);
		tmp->ScopeId = sa6->sin6_scope_id;
		return new IPEndPoint(tmp,ntohs(sa6->sin6_port));
	default:
		return NULL;
	}
}

static int getSockAddr(IPEndPoint* ep, sockaddr* sa, int* salen) {
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
		sa4->sin_port = ep->Port;
		return 0;
	case AddressFamily::InterNetworkV6:
		if(*salen<sizeof(sockaddr_in6)) return SOCKET_ERROR;
		*salen = sizeof(sockaddr_in6);
		sockaddr_in6* sa6;
		sa6 = (sockaddr_in6*)sa;
		sa6->sin6_family = AF_INET6;
		p = &ep->Address->GetAddressBytes()[0];
		memcpy(&sa6->sin6_addr,p,16);
		sa6->sin6_port = ep->Port;
		sa6->sin6_flowinfo = 0;
		sa6->sin6_scope_id = (u_long)ep->Address->ScopeId;
		return 0;
	default:
		return SOCKET_ERROR;
	}
}

/*
accept
bind
closesocket
connect
freeaddrinfo
getaddrinfo
gethostbyaddr
gethostbyname
gethostname
getnameinfo
getpeername
getprotobyname
getprotobynumber
getservbyname
getservbyport
getsockname
getsockopt
htonl
htons
inet_addr
inet_ntoa
ioctlsocket
listen
ntohl
ntohs
recv
recvfrom
select
send
sendto
setsockopt
shutdown
socket
*/

extern "C" int __stdcall __WSAFDIsSet(SOCKET fd, fd_set* set) {
	return 0;
}

extern "C" SOCKET __stdcall accept(SOCKET s, sockaddr* addr, int* addrlen) {
	ISocket* socket = Manager::getSocket(s);
	if(socket==NULL) return WSAENOTSOCK;
	ISocket* newsock = socket->Accept();
	if(newsock!=NULL && addr!=NULL) {
		IPEndPoint* ep = newsock->get_RemoteEndPoint();
		unsigned char __pin* buf = &ep->Address->GetAddressBytes()[0];
		switch(ep->AddressFamily) {
		case AddressFamily::InterNetwork:
			*addrlen = sizeof(sockaddr_in);
			sockaddr_in* sa4;
			sa4 = (sockaddr_in*)addr;
			sa4->sin_family = AF_INET;
			sa4->sin_port = htons(ep->Port);
			memcpy(&sa4->sin_addr,buf,4);
			break;
		case AddressFamily::InterNetworkV6:
			*addrlen = sizeof(sockaddr_in6);
			sockaddr_in6* sa6;
			sa6 = (sockaddr_in6*)addr;
			sa6->sin6_family = AF_INET6;
			sa6->sin6_port = htons(ep->Port);
			sa6->sin6_flowinfo = 0;
			sa6->sin6_scope_id = 0;
			memcpy(&sa6->sin6_addr,buf,16);
			break;
		default:
			return NULL;
		}
	}
	return Manager::newSocket(newsock);
}

extern "C" int __stdcall bind(SOCKET s, const sockaddr* name, int namelen) {
	ISocket* socket = Manager::getSocket(s);
	if(socket==NULL) return WSAENOTSOCK;
	socket->Bind(getEndPoint(name,namelen));
	return 0;
}

extern "C" int __stdcall closesocket(SOCKET s) {
	ISocket* socket = Manager::getSocket(s);
	if(socket==NULL) return WSAENOTSOCK;
	socket->Close();
	return 0;
}

extern "C" int __stdcall connect(SOCKET s, const sockaddr* name, int namelen) {
	ISocket* socket = Manager::getSocket(s);
	if(socket==NULL) return WSAENOTSOCK;
	IPEndPoint* ep = getEndPoint(name,namelen);
	socket->Connect(ep);
	return 0;
}

extern "C" void __stdcall freeaddrinfo(struct addrinfo* ai) {
	while(ai!=NULL) {
		delete ai->ai_addr;
		delete [] ai->ai_canonname;
		ai = ai->ai_next;
	}
}

/*
extern "C" void __stdcall FreeAddrInfoW(struct addrinfoW* ai) {
}
*/

extern "C" int __stdcall getaddrinfo(const char* nodename, const char* servname, const addrinfo* hints, struct addrinfo** res) {
	if(nodename==NULL && servname==NULL) return WSAEINVAL;

	bool ipv4 = Socket::SupportsIPv4 && (hints==NULL || hints->ai_protocol==PF_INET || hints->ai_protocol==PF_UNSPEC);
	bool ipv6 = Socket::SupportsIPv6 && (hints==NULL || hints->ai_protocol==PF_INET6 || hints->ai_protocol==PF_UNSPEC);
	if(!ipv4 && !ipv6) {
		return WSAEPFNOSUPPORT;
	}

	SocketType st = SocketType::Unknown;
	ProtocolType pt = ProtocolType::Unspecified;
	if(hints!=NULL) {
		if(hints->ai_addrlen!=0 || hints->ai_canonname!=NULL || hints->ai_addr!=NULL || hints->ai_next!=NULL) {
			return WSAEINVAL;
		}
		switch(hints->ai_socktype) {
		case 0:
			break;
		case SOCK_STREAM:
			st = SocketType::Stream;
			break;
		case SOCK_DGRAM:
			st = SocketType::Dgram;
			break;
		default:
			return WSAESOCKTNOSUPPORT;
		}
		switch(hints->ai_protocol) {
		case 0:
			break;
		case IPPROTO_TCP:
			pt = ProtocolType::Tcp;
			if(st==SocketType::Dgram) return WSANO_DATA;
			break;
		case IPPROTO_UDP:
			pt = ProtocolType::Udp;
			if(st==SocketType::Stream) return WSANO_DATA;
			break;
		default:
			return WSAEPROTONOSUPPORT;
		}
	}

	ArrayList* addresses = new ArrayList();
	String* canonname = NULL;
	if(nodename==NULL) {
		bool passive = hints!=NULL && (hints->ai_flags&AI_PASSIVE)==AI_PASSIVE;
		if(!passive) canonname = S"localhost";
		addresses->Add(passive ?IPAddress::Any :IPAddress::Loopback);
		addresses->Add(passive ?IPAddress::IPv6Any :IPAddress::IPv6Loopback);
	} else {
		String* node = new String(nodename);
		IPHostEntry* e;
		try {
			IPAddress* addr = IPAddress::Parse(node);
			e = Manager::host->GetHostByAddress(addr);
		} catch(...) {
			e = Manager::host->GetHostByName(node);
		}
		addresses->AddRange(array(e->AddressList));
		canonname = e->HostName;
	}

	if(addresses->Count==0) {
		return WSANO_DATA;
	} else {
		addrinfo* first = NULL;
		addrinfo* last = NULL;
		addrinfo ai;
		for(int i=0; i<addresses->Count; ++i) {
			IPAddress* ipaddr = static_cast<IPAddress*>(addresses->Item[i]);
			ai.ai_flags = hints->ai_flags;
			ai.ai_canonname = s2a(canonname);
			unsigned char __pin* buf = &ipaddr->GetAddressBytes()[0];
			switch(ipaddr->GetAddressBytes()->Length) {
			case 4:
				if(!ipv4) break;
				ai.ai_family = PF_INET;
				sockaddr_in* sa4;
				sa4 = new sockaddr_in();
				sa4->sin_family = AF_INET;
				memcpy(&sa4->sin_addr,buf,4);
				sa4->sin_port = 0;
				ai.ai_addr = (sockaddr*)sa4;
				ai.ai_addrlen = sizeof(SOCKADDR_IN);
				break;
			case 16:
				if(!ipv6) break;
				ai.ai_family = PF_INET6;
				SOCKADDR_IN6* sa6;
				sa6 = new SOCKADDR_IN6();
				sa6->sin6_family = AF_INET6;
				memcpy(&sa6->sin6_addr,buf,16);
				sa6->sin6_port = 0;
				sa6->sin6_flowinfo = 0;
				ai.ai_addr = (sockaddr*)sa6;
				ai.ai_addrlen = sizeof(SOCKADDR_IN6);
				break;
			default:
				return WSAEFAULT;
			}
			if(st!=SocketType::Dgram && pt!=ProtocolType::Udp) {
				addrinfo* tmp = new addrinfo(ai);
				tmp->ai_socktype = SOCK_STREAM;
				tmp->ai_protocol = IPPROTO_TCP;
				tmp->ai_next = NULL;
				if(first==NULL) {
					first = last = tmp;
				} else {
					last->ai_next = tmp;
					last = tmp;
				}
			}
			if(st!=SocketType::Stream && pt!=ProtocolType::Tcp) {
				addrinfo* tmp = new addrinfo(ai);
				tmp->ai_socktype = SOCK_DGRAM;
				tmp->ai_protocol = IPPROTO_UDP;
				tmp->ai_next = NULL;
				if(first==NULL) {
					first = last = tmp;
				} else {
					last->ai_next = tmp;
					last = tmp;
				}
			}

		}
		*res = first;
		return 0;
	}
}

/*
extern "C" int __stdcall GetAddrInfoW(const wchar_t* nodename, const wchar_t* servname, const addrinfoW* hints, struct addrinfoW** res) {
	return 0;
}
*/

extern "C" HOSTENT* __stdcall gethostbyaddr(const char* _addr, int len, int type) {
	return NULL;
	/*
	static hostent entry;
	static char buf[256];
	IPAddress* addr = IPAddress::Parse(_addr);
	IPHostEntry* ipentry = Manager::host->GetHostByAddress(addr);
	switch(addr->AddressFamily) {
	case AddressFamily::InterNetwork:
        entry.h_addrtype = ADDR_INET;
		break;
	case AddressFamily::InterNetworkV6:
		entry.h_addrtype = ADDR_INET6;
		break;
	default:
		return NULL;
	}
	s2a(ipentry->HostName,buf,sizeof(buf));
	entry.h_name = buf;
	entry.h_length 
	*/
}

extern "C" hostent* __stdcall gethostbyname(const char* name) {
	return NULL;
	/*
	static hostent entry;
	IPHostEntry* ipentry = Manager::host->GetHostByName(new String(name));
	entry.h_addrtype = ADDR_INET;
	entry.h_name 
	*/
}

extern "C" int __stdcall gethostname(char* name, int namelen) {
	unsigned char buf __gc[] = System::Text::Encoding::Default->GetBytes(Manager::host->HostName);
	if(buf->Length>=namelen) {
		return SOCKET_ERROR;
	} else {
		unsigned char __pin* p = &buf[0];
		memcpy(name,p,buf->Length);
		name[buf->Length] = '\0';
		return 0;
	}
}

extern "C" int __stdcall getnameinfo(const struct sockaddr* sa, socklen_t salen, char* host, DWORD hostlen, char* serv, DWORD servlen, int flags) {
	IPEndPoint* ep = getEndPoint(sa,salen);
	if(ep==NULL) return seterr(WSAEFAULT);
	if((flags&NI_NUMERICHOST)==NI_NUMERICHOST) {
		s2a(ep->Address->ToString(),host,hostlen);
	} else {
		IPHostEntry* e = Manager::host->GetHostByAddress(ep->Address);
		if(e!=NULL) {
			String* hostName = e->HostName;
			if((flags&NI_NOFQDN)==NI_NOFQDN) {
				int i = hostName->IndexOf('.');
				if(i>=0) hostName = hostName->Substring(0,i);
			}
			s2a(hostName,host,hostlen);
		} else {
			s2a(ep->Address->ToString(),host,hostlen);
		}
	}
	//TODO: service port resolution
	return 0;
}

extern "C" int __stdcall GetNameInfoW(const struct sockaddr* sa, socklen_t salen, wchar_t* host, DWORD hostlen, wchar_t* serv, DWORD servlen, int flags) {
	return WSAEOPNOTSUPP;
}

extern "C" int __stdcall getpeername(SOCKET s, struct sockaddr* name, int* namelen) {
	ISocket* socket = Manager::getSocket(s);
	if(socket==NULL) return WSAENOTSOCK;
	return seterrif(getSockAddr(socket->RemoteEndPoint,name,namelen),WSAEFAULT);
}

extern "C" PROTOENT* __stdcall getprotobyname(const char* name) {
	//TODO: unsupported
	return NULL;
}

extern "C" PROTOENT* __stdcall getprotobynumber(int number) {
	//TODO: unsupported
	return NULL;
}

extern "C" servent* __stdcall getservbyname(const char* name, const char* proto) {
	//TODO: unsupported
	return NULL;
}

extern "C" servent* __stdcall getservbyport(int port, const char* proto) {
	//TODO: unsupported
	return NULL;
}

extern "C" int __stdcall getsockname(SOCKET s, struct sockaddr* name, int* namelen) {
	ISocket* socket = Manager::getSocket(s);
	if(socket==NULL) return WSAENOTSOCK;
	return seterrif(getSockAddr(socket->LocalEndPoint,name,namelen),WSAEFAULT);
}

/*
extern "C" int __stdcall getsockopt(SOCKET s, int level, int optname, char* optval, int* optlen) {
	return 0;
}
*/

extern "C" u_long __stdcall htonl(u_long hostlong) {
	return (u_long)Manager::host->hton((__int32)hostlong);
}

extern "C" u_short __stdcall htons(u_short hostshort) {
	return (u_short)Manager::host->hton((__int16)hostshort);
}

extern "C" unsigned long __stdcall inet_addr(const char* cp) {
	return (u_long)Manager::shellnet->inet_addr(new String(cp));
}

extern "C" char* __stdcall inet_ntoa(struct in_addr in) {
	static char buf[16];
	sprintf(buf,"%d.%d.%d.%d"
		,in.S_un.S_un_b.s_b1
		,in.S_un.S_un_b.s_b2
		,in.S_un.S_un_b.s_b3
		,in.S_un.S_un_b.s_b4);
	return buf;
}

/*
extern "C" int __stdcall ioctlsocket(SOCKET s, long cmd, u_long* argp) {
	return 0;
}
*/

extern "C" int __stdcall listen(SOCKET s, int backlog) {
	ISocket* socket = Manager::getSocket(s);
	if(socket==NULL) return WSAENOTSOCK;
	try {
		socket->Listen(backlog);
		return 0;
	} catch(...) {
		return SOCKET_ERROR;
	}
}

extern "C" u_long __stdcall ntohl(u_long netlong) {
	return (u_long)Manager::host->ntoh((__int32)netlong);
}

extern "C" u_short __stdcall ntohs(u_short netshort) {
	return (u_short)Manager::host->ntoh((__int16)netshort);
}

extern "C" int __stdcall recv(SOCKET s, char* buf, int len, int flags) {
	ISocket* socket = Manager::getSocket(s);
	if(socket==NULL) return WSAENOTSOCK;
	unsigned char _buf __gc[] = new unsigned char __gc[len];
	int _len = socket->Receive(&_buf);
	unsigned char __pin* p = &_buf[0];
	memcpy(buf,p,_len);
	return _len;
}

extern "C" int __stdcall recvfrom(SOCKET s, char* buf, int len, int flags, sockaddr* from, int* fromlen) {
	WSABUF wsabuf;
	wsabuf.buf = buf;
	wsabuf.len = len;
	DWORD received = -1;
	DWORD _flags = flags;
	return WSARecvFrom(s,&wsabuf,1,&received,&_flags,from,fromlen,NULL,NULL);
}

extern "C" int __stdcall select(int nfds, fd_set* readfds, fd_set* writefds, fd_set* exceptfds, const struct timeval* timeout) {
	ISocket* r __gc[] = new ISocket* __gc[readfds->fd_count];
	ISocket* w __gc[] = new ISocket* __gc[writefds->fd_count];
	ISocket* e __gc[] = new ISocket* __gc[exceptfds->fd_count];
	for(int i=0; i<r->Length; ++i) {
		r[i] = Manager::getSocket(readfds->fd_array[i]);
	}
	for(int i=0; i<w->Length; ++i) {
		w[i] = Manager::getSocket(writefds->fd_array[i]);
	}
	for(int i=0; i<e->Length; ++i) {
		e[i] = Manager::getSocket(exceptfds->fd_array[i]);
	}
	int ret = Manager::peer->Select(&r,&w,&e,timeout->tv_sec*1000+timeout->tv_usec);
	FD_ZERO(readfds);
	FD_ZERO(writefds);
	FD_ZERO(exceptfds);
	for(int i=0; i<r->Length; ++i) {
		FD_SET(r[i]->SocketId,readfds);
	}
	for(int i=0; i<w->Length; ++i) {
		FD_SET(w[i]->SocketId,writefds);
	}
	for(int i=0; i<e->Length; ++i) {
		FD_SET(e[i]->SocketId,exceptfds);
	}
	return ret;
}

extern "C" int __stdcall send(SOCKET s, const char* buf, int len, int flags) {
	ISocket* socket = Manager::getSocket(s);
	if(socket==NULL) return WSAENOTSOCK;
	unsigned char _buf __gc[] = new unsigned char __gc[len];
	unsigned char __pin* p = &_buf[0];
	memcpy(p,buf,len);
	p = NULL;
	return socket->Send(_buf);
}

/*
extern "C" int __stdcall sendto(SOCKET s, const char* buf, int len, int flags, const sockaddr* to, int tolen) {
	return 0;
}

extern "C" int __stdcall setsockopt(SOCKET s, int level, int optname, const char* optval, int optlen) {
	return 0;
}
*/

extern "C" int __stdcall shutdown(SOCKET s, int how) {
	ISocket* socket = Manager::getSocket(s);
	if(socket==NULL) return WSAENOTSOCK;
	switch(how) {
	case SD_RECEIVE:
		socket->Shutdown(SocketShutdown::Receive);
		break;
	case SD_SEND:
		socket->Shutdown(SocketShutdown::Send);
		break;
	case SD_BOTH:
		socket->Shutdown(SocketShutdown::Both);
		break;
	}
	return 0;
}

extern "C" SOCKET __stdcall socket(int af, int type, int protocol) {
	return WSASocket(af,type,protocol,NULL,0,0);
}

static bool initialized = false;

extern "C" int __stdcall WSAStartup(IN WORD wVersionRequested, OUT LPWSADATA lpWSAData) {
	
	if(initialized) return 0;

	initialized = true;
	__crt_dll_initialize();

	try {

		Console::Write("Registering pipe channel...");
		//RemotingConfiguration::Configure("ws2_32.config");
		IChannel* chnl = __gc new Pipe::PipeChannel();
		ChannelServices::RegisterChannel(chnl);
		Console::WriteLine("ok");
	
		Console::Write("Activating the shellnet...");
		Manager::shellnet = static_cast<IShellnet*>(Activator::GetObject(
			__typeof(IShellnet),"pipe://shellnet/shellnet"));
		/*
		Object* args __gc[];
		args = new Object* __gc[1];
		args[0] = new UrlAttribute("pipe://shellnet/Bootstrap");
		Manager::shellnet = static_cast<Bootstrap*>(Activator::CreateInstance(__typeof(Bootstrap),NULL,args))->Shellnet;
		*/

		if(Manager::shellnet==NULL) {
			Console::WriteLine("failed");
			Console::WriteLine("the shellnet instance is not running or published.");
			__crt_dll_terminate();
			initialized = false;
			return -1;
		} else {
			Console::WriteLine("ok");
		}

		Console::Write("initializing...");
		Manager::peer = Manager::shellnet->Startup(Manager::KeepaliveMutexName);
		Manager::host = Manager::peer->Host;
		Manager::Register(Manager::peer);
		Console::WriteLine("ok");

		IPAddress* addr = new IPAddress(0x100007F);
	
		lpWSAData->wVersion = wVersionRequested;
		lpWSAData->wHighVersion = wVersionRequested;
		strcpy(lpWSAData->szDescription,"shellnet socket redirector");
		strcpy(lpWSAData->szSystemStatus,"");
		lpWSAData->iMaxSockets = 30000;
		lpWSAData->iMaxUdpDg = 0;
		lpWSAData->lpVendorInfo = NULL;

		return 0;
	
	} catch(Exception* ex) {
		Console::WriteLine("failed");
		Console::Error->WriteLine(ex->ToString());
		return -1;
	} catch(...) {
		return -1;
	}

}

extern "C" int __stdcall WSACleanup() {
	Console::WriteLine("Disconnected from Shellnet Socket Redirector");
	__crt_dll_terminate();
	return Manager::peer->Cleanup();
}

extern "C" int __stdcall WSAEnumProtocolsA(LPINT lpiProtocols, LPWSAPROTOCOL_INFOA lpProtocolBuffer, LPDWORD lpdwBufferLength) {
	unsigned int protocols __gc[];
	if(lpiProtocols==NULL) {
		protocols = NULL;
	} else {
		int i=0;
		while(lpiProtocols[i]!=0) {
			++i;
		}
		++i;
		protocols = new unsigned int __gc[i];
		unsigned int __pin* p = &protocols[0];
		memcpy(p,lpiProtocols,i*sizeof(int));
	}
	unsigned char buf __gc[] = new unsigned char __gc[*lpdwBufferLength];
	unsigned int dwBufferLength = *lpdwBufferLength;
	try {
		return Manager::shellnet->_EnumProtocolsA(protocols, buf, &dwBufferLength);
	} __finally {
		*lpdwBufferLength = dwBufferLength;
		unsigned char __pin* p = &buf[0];
		memcpy(lpProtocolBuffer,p,*lpdwBufferLength);
	}
}

extern "C" int __stdcall WSAEnumProtocolsW(LPINT lpiProtocols, LPWSAPROTOCOL_INFOW lpProtocolBuffer, LPDWORD lpdwBufferLength) {
	unsigned int protocols __gc[];
	if(lpiProtocols==NULL) {
		protocols = NULL;
	} else {
		int i=0;
		while(lpiProtocols[i]!=0) {
			++i;
		}
		++i;
		protocols = new unsigned int __gc[i];
		unsigned int __pin* p = &protocols[0];
		memcpy(p,lpiProtocols,i*sizeof(int));
	}
	unsigned char buf __gc[] = new unsigned char __gc[*lpdwBufferLength];
	unsigned int dwBufferLength = *lpdwBufferLength;
	try {
		return Manager::shellnet->_EnumProtocolsW(protocols, buf, &dwBufferLength);
	} __finally {
		*lpdwBufferLength = dwBufferLength;
		unsigned char __pin* p = &buf[0];
		memcpy(lpProtocolBuffer,p,*lpdwBufferLength);
	}
}

extern "C" int __stdcall WSAGetLastError() {
	return lasterror;
}

extern "C" int __stdcall WSARecv(
	SOCKET s,
	LPWSABUF lpBuffers,
	DWORD dwBufferCount,
	LPDWORD lpNumberOfBytesRecvd,
	LPDWORD lpFlags,
	LPWSAOVERLAPPED lpOverlapped,
	LPWSAOVERLAPPED_COMPLETION_ROUTINE lpCompletionRoutine)
{
	return WSARecvFrom(s,lpBuffers,dwBufferCount,lpNumberOfBytesRecvd,lpFlags,NULL,NULL,lpOverlapped,lpCompletionRoutine);
}

extern "C" int __stdcall WSARecvFrom(
	SOCKET s,
	LPWSABUF lpBuffers,
	DWORD dwBufferCount,
	LPDWORD lpNumberOfBytesRecvd,
	LPDWORD lpFlags,
	struct sockaddr* lpFrom,
	LPINT lpFromlen,
	LPWSAOVERLAPPED lpOverlapped,
	LPWSAOVERLAPPED_COMPLETION_ROUTINE lpCompletionRoutine)
{
	ISocket* socket = Manager::getSocket(s);
	if(socket==NULL) return WSAENOTSOCK;
	if(lpOverlapped==NULL && lpCompletionRoutine==NULL) {
		// Non-overlapped I/O
		*lpNumberOfBytesRecvd = 0;
		for(int i=0; i<(int)dwBufferCount; ++i) {
			unsigned char _buf __gc[] = new unsigned char __gc[lpBuffers[i].len];
			int _len = socket->Receive(&_buf);
			unsigned char __pin* p = &_buf[0];
			memcpy(lpBuffers[i].buf,p,_len);
			*lpNumberOfBytesRecvd += _len;
		}
		return 0;
	} else {
		return -1;
	}
}

extern "C" void __stdcall WSASetLastError(int iError) {
	lasterror = iError;;
}

extern "C" SOCKET __stdcall WSASocketA(int af, int type, int protocol, LPWSAPROTOCOL_INFOA lpProtocolInfo, GROUP g, DWORD dwFlags) {
	if(lpProtocolInfo!=NULL) {
		Console::Error->WriteLine("Specified protocol information is ignored.");
	}
	return WSASocketW(af,type,protocol,NULL,g,dwFlags);
}

extern "C" SOCKET __stdcall WSASocketW(int af, int type, int protocol, LPWSAPROTOCOL_INFOW lpProtocolInfo, GROUP g, DWORD dwFlags) {
	AddressFamily _af;
	SocketType _st;
	ProtocolType _pt;
	switch(af) {
	case AF_INET:
		_af = AddressFamily::InterNetwork;
		break;
	case AF_INET6:
		_af = AddressFamily::InterNetworkV6;
		break;
	default:
		_af = AddressFamily::Unknown;
		break;
	}
	switch(type) {
	case SOCK_STREAM:
		_st = SocketType::Stream;
		break;
	case SOCK_DGRAM:
		_st = SocketType::Dgram;
		break;
	default:
		_st = SocketType::Unknown;
		break;
	}
	if(protocol==0) {
		_pt = ProtocolType::Unspecified;
	} else {
		Console::Error->WriteLine("Specified protocol type is ignored and use default protocol selected by winsock.");
	}
	if(lpProtocolInfo!=NULL) {
		Console::Error->WriteLine("Specified protocol information is ignored.");
	}
	if(g!=0) Console::Error->WriteLine("Specified group information is ignored.");
	if(dwFlags!=0) Console::Error->WriteLine("Specified flags are all ignored.");
	return Manager::newSocket(Manager::peer->CreateSocket(_af,_st,_pt));
}
