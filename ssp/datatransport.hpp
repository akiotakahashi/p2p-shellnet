#include "stdafx.h"
#include "ssp.h"
#include "manager.h"


namespace PROTOCOL {

	static WSAPROTOCOL_INFO ProtocolInfo;
	static WSPUPCALLTABLE UpcallTable;

	static SOCKET WSPAPI WSPAccept(
		SOCKET s,
		struct sockaddr	FAR	* addr,
		LPINT addrlen,
		LPCONDITIONPROC	lpfnCondition,
		DWORD_PTR dwCallbackData,
		LPINT lpErrno)
	{
		PREPARE_SOCKET(s,sd,socket,INVALID_SOCKET,lpErrno);
		ISocket* newsock = socket->Accept();
		if(newsock==NULL) {
			return seterr(INVALID_SOCKET,WSAEINVAL,lpErrno);
		} else {
			getSockAddr(newsock->RemoteEndPoint,addr,addrlen);
			return NewSocket(sd->categoryEntryId,UpcallTable,af2(sd->addressFamily),st2(sd->socketType),pt2(sd->protocolType),newsock,lpErrno);
		}
	}

	static INT WSPAPI WSPAddressToString(
		LPSOCKADDR lpsaAddress,
		DWORD dwAddressLength,
		LPWSAPROTOCOL_INFOW	lpProtocolInfo,
		LPWSTR lpszAddressString,
		LPDWORD	lpdwAddressStringLength,
		LPINT lpErrno)
	{
		switch(dwAddressLength) {
		case sizeof(sockaddr_in):
			if(lpszAddressString==NULL) {
				*lpdwAddressStringLength = 16;
				return 0;
			} else {
				sockaddr_in* sa4 = (sockaddr_in*)lpsaAddress;
				wsprintf(lpszAddressString,L"%d.%d.%d.%d"
					,sa4->sin_addr.S_un.S_un_b.s_b1
					,sa4->sin_addr.S_un.S_un_b.s_b2
					,sa4->sin_addr.S_un.S_un_b.s_b3
					,sa4->sin_addr.S_un.S_un_b.s_b4);
				*lpdwAddressStringLength = (DWORD)wcslen(lpszAddressString)+1;
				return 0;
			}
		case sizeof(sockaddr_in6):
			if(lpszAddressString==NULL) {
				*lpdwAddressStringLength = 40;
				return 0;
			} else {
				sockaddr_in6* sa6 = (sockaddr_in6*)lpsaAddress;
				wsprintf(lpszAddressString,L"%x:%x:%x:%x:%x:%x:%x:%x"
					,sa6->sin6_addr.u.Word[0]
					,sa6->sin6_addr.u.Word[1]
					,sa6->sin6_addr.u.Word[2]
					,sa6->sin6_addr.u.Word[3]
					,sa6->sin6_addr.u.Word[4]
					,sa6->sin6_addr.u.Word[5]
					,sa6->sin6_addr.u.Word[6]
					,sa6->sin6_addr.u.Word[7]);
				*lpdwAddressStringLength = (DWORD)wcslen(lpszAddressString)+1;
				return 0;
			}
		}
		IPEndPoint* ep = getEndPoint(lpsaAddress,dwAddressLength);
		String* str = ep->Address->ToString();
		int bufsize = *lpdwAddressStringLength;
		*lpdwAddressStringLength = str->Length+1;
		if(str->Length > bufsize-1) {
			*lpErrno = WSAEFAULT;
			return SOCKET_ERROR;
		} else {
			s2w(str,lpszAddressString,*lpdwAddressStringLength);
			return 0;
		}
	}

	inline int wrap(bool ret) {
		return ret ? 0 : SOCKET_ERROR;
	}

	static int WSPAPI WSPAsyncSelect(
		SOCKET s,
		HWND hWnd,
		unsigned int wMsg,
		long lEvent,
		LPINT lpErrno)
	{
		PREPARE_SOCKET(s,sd,socket,SOCKET_ERROR,lpErrno);
		sd->hAsyncSelectWnd = hWnd;
		sd->wAsyncSelectMsg = wMsg;
		sd->fdAsyncSelectFilter = lEvent;
		return wrap(socket->AsyncSelect(hWnd,wMsg,lEvent,UpcallTable.lpWPUPostMessage,(void*)s,lpErrno));
	}

	static int WSPAPI WSPBind(SOCKET s, const struct sockaddr FAR * name, int namelen, LPINT lpErrno) {
		PREPARE_SOCKET(s,sd,socket,SOCKET_ERROR,lpErrno);
		try {
			return socket->Bind(getEndPoint(name,namelen),lpErrno) ? 0 : SOCKET_ERROR;
		} catch(Exception* ex) {
			Console::Error->WriteLine(ex);
			return seterr(SOCKET_ERROR,WSAEFAULT,lpErrno);
		}
	}

	static int WSPAPI WSPCancelBlockingCall(LPINT lpErrno)
	{
		//TODO: not implemented
		return SOCKET_ERROR;
	}

	static int WSPAPI WSPCloseSocket(SOCKET s, LPINT lpErrno) {
		PREPARE_SOCKET(s,sd,socket,INVALID_SOCKET,lpErrno);
		socket->Close();
		return 0;
	}

	static int WSPAPI WSPConnect(
		SOCKET s,
		const struct sockaddr FAR *	name,
		int	namelen,
		LPWSABUF lpCallerData,
		LPWSABUF lpCalleeData,
		LPQOS lpSQOS,
		LPQOS lpGQOS,
		LPINT lpErrno)
	{
		PREPARE_SOCKET(s,sd,socket,SOCKET_ERROR,lpErrno);
		IPEndPoint* rep = getEndPoint(name,namelen);
		if(rep==NULL) return seterr(SOCKET_ERROR,WSAEFAULT,lpErrno);
		try {
			return socket->Connect(rep,lpErrno) ? 0 : SOCKET_ERROR;
		} catch(Exception* ex) {
			Console::Error->WriteLine(ex);
			return seterr(SOCKET_ERROR,WSAECONNREFUSED,lpErrno);
		}
	}

	static int WSPAPI WSPDuplicateSocket(
		SOCKET s,
		DWORD dwProcessId,
		LPWSAPROTOCOL_INFO lpProtocolInfo,
		LPINT lpErrno)
	{
		PREPARE_SOCKET(s,sd,socket,SOCKET_ERROR,lpErrno);
		return SOCKET_ERROR;
	}

	static int WSPAPI WSPEnumNetworkEvents(
		SOCKET s,
		WSAEVENT hEventObject,
		LPWSANETWORKEVENTS lpNetworkEvents,
		LPINT lpErrno)
	{
		//TODO: not implemented
		PREPARE_SOCKET(s,sd,socket,SOCKET_ERROR,lpErrno);
		return SOCKET_ERROR;
	}

	static int WSPAPI WSPEventSelect(
		SOCKET s,
		WSAEVENT hEventObject,
		long lNetworkEvents,
		LPINT lpErrno)
	{
		//TODO: not implemented
		PREPARE_SOCKET(s,sd,socket,SOCKET_ERROR,lpErrno);
		return SOCKET_ERROR;
	}

	static BOOL WSPAPI WSPGetOverlappedResult(
		SOCKET s,
		LPWSAOVERLAPPED lpOverlapped,
		LPDWORD lpcbTransfer,
		BOOL fWait,
		LPDWORD lpdwFlags,
		LPINT lpErrno)
	{
		//TODO: not implemented
		PREPARE_SOCKET(s,sd,socket,SOCKET_ERROR,lpErrno);
		return seterr(SOCKET_ERROR,WSAEFAULT,lpErrno);
	}

	static int WSPAPI WSPGetPeerName(
		SOCKET s,
		struct sockaddr	FAR	* name,
		LPINT namelen,
		LPINT lpErrno)
	{
		PREPARE_SOCKET(s,sd,socket,SOCKET_ERROR,lpErrno);
		return seterrif(getSockAddr(socket->RemoteEndPoint,name,namelen),SOCKET_ERROR,WSAEFAULT,lpErrno);
	}

	static int WSPAPI WSPGetSockName(
		SOCKET s,
		struct sockaddr	FAR	* name,
		LPINT namelen,
		LPINT lpErrno)
	{
		PREPARE_SOCKET(s,sd,socket,SOCKET_ERROR,lpErrno);
		return seterrif(getSockAddr(socket->LocalEndPoint,name,namelen),SOCKET_ERROR,WSAEFAULT,lpErrno);
	}

	static int WSPAPI WSPGetSockOpt(
		SOCKET s,
		int level,
		int optname,
		char* optval,
		LPINT optlen,
		LPINT lpErrno)
	{
		PREPARE_SOCKET(s,sd,socket,SOCKET_ERROR,lpErrno);
		int bufsize = *optlen;
		switch(level) {
		default:
			*lpErrno = WSAEINVAL;
			return SOCKET_ERROR;
		case SOL_SOCKET:
			switch(optname) {
			default:
				*lpErrno = WSAEFAULT;
				return SOCKET_ERROR;
			case SO_PROTOCOL_INFO:
				*optlen = sizeof(WSAPROTOCOL_INFO);
				if(bufsize<sizeof(WSAPROTOCOL_INFO)) {
					return SOCKET_ERROR;
				} else {
					*((LPWSAPROTOCOL_INFO)optval) = ProtocolInfo;
					return 0;
				}
			}
		}
	}

	static BOOL WSPAPI WSPGetQOSByName(
		SOCKET s,
		LPWSABUF lpQOSName,
		LPQOS lpQOS,
		LPINT lpErrno)
	{
		PREPARE_SOCKET(s,sd,socket,SOCKET_ERROR,lpErrno);
		return FALSE;
	}

	static int WSPAPI WSPIoctl(
		SOCKET s,
		DWORD dwIoControlCode,
		LPVOID lpvInBuffer,
		DWORD cbInBuffer,
		LPVOID lpvOutBuffer,
		DWORD cbOutBuffer,
		LPDWORD lpcbBytesReturned,
		LPWSAOVERLAPPED lpOverlapped,
		LPWSAOVERLAPPED_COMPLETION_ROUTINE lpCompletionRoutine,
		LPWSATHREADID lpThreadId,
		LPINT lpErrno)
	{
		//TODO: not implemented
		PREPARE_SOCKET(s,sd,socket,SOCKET_ERROR,lpErrno);
		return seterr(SOCKET_ERROR,WSAEFAULT,lpErrno);
	}

	static SOCKET WSPAPI WSPJoinLeaf(
		SOCKET s,
		const struct sockaddr* name,
		int namelen,
		LPWSABUF lpCallerData,
		LPWSABUF lpCalleeData,
		LPQOS lpSQOS,
		LPQOS lpGQOS,
		DWORD dwFlags,
		LPINT lpErrno)
	{
		//TODO: not implemented
		PREPARE_SOCKET(s,sd,socket,SOCKET_ERROR,lpErrno);
		return seterr(SOCKET_ERROR,WSAEFAULT,lpErrno);
	}

	static int WSPAPI WSPListen(SOCKET s, int backlog, LPINT lpErrno) {
		PREPARE_SOCKET(s,sd,socket,SOCKET_ERROR,lpErrno);
		try {
			return socket->Listen(backlog,lpErrno) ? 0 : SOCKET_ERROR;
		} catch(Exception* ex) {
			Console::Error->WriteLine(ex);
			return seterr(SOCKET_ERROR,WSAEOPNOTSUPP,lpErrno);
		}
	}

	static int WSPAPI WSPRecv(
		SOCKET s,
		LPWSABUF lpBuffers,
		DWORD dwBufferCount,
		LPDWORD	lpNumberOfBytesRecvd,
		LPDWORD	lpFlags,
		LPWSAOVERLAPPED	lpOverlapped,
		LPWSAOVERLAPPED_COMPLETION_ROUTINE lpCompletionRoutine,
		LPWSATHREADID lpThreadId,
		LPINT lpErrno)
	{
		if(lpOverlapped!=NULL || lpCompletionRoutine!=NULL) return seterr(SOCKET_ERROR,WSA_OPERATION_ABORTED,lpErrno);
		_PREPARE_SOCKET(s,sd,socket,SOCKET_ERROR,lpErrno);
		*lpNumberOfBytesRecvd = 0;
		for(int i=0; i<(int)dwBufferCount; ++i) {
			if(lpBuffers[i].len<=0) continue;
			unsigned char _buf __gc[] = socket->Receive(lpBuffers[i].len);
			if(_buf==NULL || _buf->Length==0) break;
			unsigned char __pin* p = &_buf[0];
			memcpy(lpBuffers[i].buf,p,_buf->Length);
			*lpNumberOfBytesRecvd += _buf->Length;
		}
		//Manager::peer->Log->WriteLine(S"WSPRecv {0} bytes",__box(*lpNumberOfBytesRecvd));
		if(*lpNumberOfBytesRecvd>0) {
			int avail = socket->Available;
			if(avail>0) {
				sd->PostAsyncMessage(UpcallTable,FD_READ);
			}
			return 0;
		} else {
			*lpErrno = WSAECONNRESET;
			return SOCKET_ERROR;
		}
	}

    static int WSPAPI WSPRecvDisconnect(
		SOCKET s,
		LPWSABUF lpInboundDisconnectData,
		LPINT lpErrno)
	{
		//TODO: not implemented
		PREPARE_SOCKET(s,sd,socket,SOCKET_ERROR,lpErrno);
		*lpErrno = WSAEFAULT;
		return SOCKET_ERROR;
	}

	static int WSPAPI WSPRecvFrom(
		SOCKET s,
		LPWSABUF lpBuffers,
		DWORD dwBufferCount,
		LPDWORD lpNumberOfBytesRecvd,
		LPDWORD lpFlags,
		struct sockaddr* lpFrom,
		LPINT lpFromlen,
		LPWSAOVERLAPPED lpOverlapped,
		LPWSAOVERLAPPED_COMPLETION_ROUTINE lpCompletionRoutine,
		LPWSATHREADID lpThreadId,
		LPINT lpErrno)
	{
		//TODO: not implemented
		PREPARE_SOCKET(s,sd,socket,SOCKET_ERROR,lpErrno);
		*lpErrno = WSAEFAULT;
		return SOCKET_ERROR;
	}

	static int WSPAPI WSPSelect(
		int nfds,
		fd_set* readfds,
		fd_set* writefds,
		fd_set* exceptfds,
		const struct timeval* timeout,
		LPINT lpErrno)
	{
		std::cerr << __FUNCTION__;
		if(timeout!=NULL) {
			std::cerr << " timeout " << std::dec << (timeout->tv_sec*1000+timeout->tv_usec/1000) << "ms";
		}
		std::cerr << std::endl;
		ISocket* rs __gc[] = NULL;
		ISocket* ws __gc[] = NULL;
		ISocket* es __gc[] = NULL;
		if(readfds!=NULL) {
			rs = new ISocket* __gc[readfds->fd_count];
			for(unsigned int i=0; i<readfds->fd_count; ++i) {
				const SOCKETDESC* sd = GetSocketDesc(UpcallTable,readfds->fd_array[i]);
				if(sd!=NULL) {
					rs[i] = GetSocket(sd->peerId,sd->socketId);
					std::wcerr << "select reading socket #" << std::dec << (UINT)readfds->fd_array[i] << std::endl;
				} else {
					//if((sd)==NULL) return seterr(SOCKET_ERROR,WSAENOTSOCK,lpErrno);
					std::wcerr << L"WSPSelect ignores invalid socket#" << std::dec << (UINT)readfds->fd_array[i] << std::endl;
				}
			}
		}
		if(writefds!=NULL) {
			ws = new ISocket* __gc[writefds->fd_count];
			for(unsigned int i=0; i<writefds->fd_count; ++i) {
				const SOCKETDESC* sd = GetSocketDesc(UpcallTable,readfds->fd_array[i]);
				if(sd!=NULL) {
					ws[i] = GetSocket(sd->peerId,sd->socketId);
					std::wcerr << "select writing socket #" << std::dec << (UINT)writefds->fd_array[i] << std::endl;
				} else {
					//if((sd)==NULL) return seterr(SOCKET_ERROR,WSAENOTSOCK,lpErrno);
					std::wcerr << L"WSPSelect ignores invalid socket#" << std::dec << (UINT)writefds->fd_array[i] << std::endl;
				}
			}
		}
		if(exceptfds!=NULL) {
			es = new ISocket* __gc[exceptfds->fd_count];
			for(unsigned int i=0; i<readfds->fd_count; ++i) {
				const SOCKETDESC* sd = GetSocketDesc(UpcallTable,readfds->fd_array[i]);
				if(sd!=NULL) {
					es[i] = GetSocket(sd->peerId,sd->socketId);
					std::wcerr << "select except socket #" << std::dec << (UINT)exceptfds->fd_array[i] << std::endl;
				} else {
					//if((sd)==NULL) return seterr(SOCKET_ERROR,WSAENOTSOCK,lpErrno);
					std::wcerr << L"WSPSelect ignores invalid socket#" << std::dec << (UINT)exceptfds->fd_array[i] << std::endl;
				}
			}
		}
		Manager::peer->Select(&rs,&ws,&es,timeout->tv_sec*1000*1000+timeout->tv_usec);
		int readiness = 0;
		if(readfds!=NULL) {
			fd_set rfds;
			FD_ZERO(&rfds);
			for(int i=0; i<rs->Length; ++i) {
				if(rs[i]!=NULL) {
					std::wcerr << "readable socket #" << std::dec << (UINT)readfds->fd_array[i] << std::endl;
					FD_SET(readfds->fd_array[i],&rfds);
					++readiness;
				}
			}
			*readfds = rfds;
		}
		if(writefds!=NULL) {
			fd_set wfds;
			FD_ZERO(&wfds);
			for(int i=0; i<ws->Length; ++i) {
				if(rs[i]!=NULL) {
					std::wcerr << "writeable socket #" << std::dec << (UINT)writefds->fd_array[i] << std::endl;
					FD_SET(writefds->fd_array[i],&wfds);
					++readiness;
				}
			}
			*writefds = wfds;
		}
		if(exceptfds!=NULL) {
			fd_set efds;
			FD_ZERO(&efds);
			for(int i=0; i<es->Length; ++i) {
				if(rs[i]!=NULL) {
					std::wcerr << "except socket #" << std::dec << (UINT)exceptfds->fd_array[i] << std::endl;
					FD_SET(exceptfds->fd_array[i],&efds);
					++readiness;
				}
			}
			*exceptfds = efds;
		}
		return readiness;
	}

	static int WSPAPI WSPSend(
		SOCKET s,
		LPWSABUF lpBuffers,
		DWORD dwBufferCount,
		LPDWORD lpNumberOfBytesSent,
		DWORD dwFlags,
		LPWSAOVERLAPPED lpOverlapped,
		LPWSAOVERLAPPED_COMPLETION_ROUTINE lpCompletionRoutine,
		LPWSATHREADID lpThreadId,
		LPINT lpErrno)
	{
		_PREPARE_SOCKET(s,sd,socket,SOCKET_ERROR,lpErrno);
		if(lpOverlapped!=NULL || lpCompletionRoutine!=NULL) return seterr(SOCKET_ERROR,WSA_OPERATION_ABORTED,lpErrno);
		try {
			int iosize = 0;
			for(int i=0; i<(int)dwBufferCount; ++i) {
				if(lpBuffers[i].len<=0) continue;
				unsigned char buf __gc[] = new unsigned char __gc[lpBuffers[i].len];
				unsigned char __pin* p = &buf[0];
				memcpy(p,lpBuffers[i].buf,lpBuffers[i].len);
				iosize += socket->Send(buf);
			}
			return 0;
		} catch(Exception* ex) {
			Console::Error->WriteLine(ex);
			return seterr(SOCKET_ERROR,WSAEFAULT,lpErrno);
		}
	}

	static int WSPAPI WSPSendDisconnect(
		SOCKET s,
		LPWSABUF lpOutboundDisconnectData,
		LPINT lpErrno)
	{
		//TODO: not implemented
		PREPARE_SOCKET(s,sd,socket,SOCKET_ERROR,lpErrno);
		return seterr(SOCKET_ERROR,WSAEFAULT,lpErrno);
	}

    static int WSPAPI WSPSendTo(
		SOCKET s,
		LPWSABUF lpBuffers,
		DWORD dwBufferCount,
		LPDWORD lpNumberOfBytesSent,
		DWORD dwFlags,
		const struct sockaddr* lpTo,
		int iTolen,
		LPWSAOVERLAPPED lpOverlapped,
		LPWSAOVERLAPPED_COMPLETION_ROUTINE lpCompletionRoutine,
		LPWSATHREADID lpThreadId,
		LPINT lpErrno)
	{
		//TODO: not implemented
		PREPARE_SOCKET(s,sd,socket,SOCKET_ERROR,lpErrno);
		return seterr(SOCKET_ERROR,WSAEFAULT,lpErrno);
	}

	static int WSPAPI WSPSetSockOpt(
		SOCKET s,
		int level,
		int optname,
		const char* optval,
		int optlen,
		LPINT lpErrno)
	{
		//TODO: not implemented
		PREPARE_SOCKET(s,sd,socket,SOCKET_ERROR,lpErrno);
		Console::Error->WriteLine(S"level  :0x{0:x}",__box(level));
		Console::Error->WriteLine(S"optname:0x{0:x}",__box(optname));
		return 0;
		return seterr(SOCKET_ERROR,WSAEFAULT,lpErrno);
	}

	static int WSPAPI WSPShutdown(
		SOCKET s,
		int	how,
		LPINT lpErrno)
	{
		PREPARE_SOCKET(s,sd,socket,SOCKET_ERROR,lpErrno);
		switch(how) {
		case SD_RECEIVE:
			socket->Shutdown(SocketShutdown::Receive);
			return 0;
		case SD_SEND:
			socket->Shutdown(SocketShutdown::Send);
			return 0;
		case SD_BOTH:
			socket->Shutdown(SocketShutdown::Both);
			return 0;
		default:
			return seterr(SOCKET_ERROR,WSAEINVAL,lpErrno);
		}
	}

	static SOCKET WSPAPI WSPSocket(
		int	af,
		int	type,
		int	protocol,
		LPWSAPROTOCOL_INFOW	lpProtocolInfo,
		GROUP g,
		DWORD dwFlags,
		LPINT lpErrno)
	{
		if(g!=0) return seterr(INVALID_SOCKET,WSAEINVAL,lpErrno);
		AddressFamily _af;
		SocketType _st;
		ProtocolType _pt;
		_af = af2(af);
		_st = st2(type);
		_pt = pt2(protocol);
		if(_af==AddressFamily::Unknown) return seterr(INVALID_SOCKET,WSAEAFNOSUPPORT,lpErrno);
		if(_st==SocketType::Unknown) return seterr(INVALID_SOCKET,WSAESOCKTNOSUPPORT,lpErrno);
		if(_pt==ProtocolType::Unknown) return seterr(INVALID_SOCKET,WSAEPROTONOSUPPORT,lpErrno);
		if(dwFlags & WSA_FLAG_OVERLAPPED) {
			Console::Error->WriteLine("Overlapped I/O sockets are not supported.");
			//return seterr(INVALID_SOCKET,WSAESOCKTNOSUPPORT,lpErrno);
		}
		return NewSocket(lpProtocolInfo->dwCatalogEntryId,UpcallTable,_af,_st,_pt,lpErrno);
	}

	static INT WSPAPI WSPStringToAddress(
		LPWSTR AddressString,
		INT	AddressFamily,
		LPWSAPROTOCOL_INFOW	lpProtocolInfo,
		LPSOCKADDR lpAddress,
		LPINT lpAddressLength,
		LPINT lpErrno)
	{
		Manager::peer->Log->WriteLine(S"WSPStringToAddress resolves {0}", new String(AddressString));
		//int buf[16];
		switch(AddressFamily) {
		case AF_INET:
			if(lpAddress!=NULL && *lpAddressLength>=sizeof(sockaddr_in)) {
				sockaddr_in* sa4;
				sa4 = (sockaddr_in*)lpAddress;
				/*
				swscanf(AddressString,L"%d.%d.%d.%d"
					,&buf[0],&buf[1],&buf[2],&buf[3]);
				*/
				IPAddress* address = Manager::host->StringToAddress(new String(AddressString),af2(AddressFamily));
				unsigned char buf __gc[] = address->GetAddressBytes();
				sa4->sin_addr.S_un.S_un_b.s_b1 = buf[0];
				sa4->sin_addr.S_un.S_un_b.s_b2 = buf[1];
				sa4->sin_addr.S_un.S_un_b.s_b3 = buf[2];
				sa4->sin_addr.S_un.S_un_b.s_b4 = buf[3];
				sa4->sin_family = AF_INET;
				sa4->sin_port = 0;
			}
			*lpAddressLength = sizeof(sockaddr_in);
			return 0;
		case AF_INET6:
			if(lpAddress!=NULL && *lpAddressLength>=sizeof(sockaddr_in6)) {
				sockaddr_in6* sa6;
				sa6 = (sockaddr_in6*)lpAddress;
				/*
				swscanf(AddressString,L"%x:%x:%x:%x:%x:%x:%x:%x"
					,&buf[0],&buf[1],&buf[2],&buf[3]
					,&buf[4],&buf[5],&buf[6],&buf[7]);
				*/
				IPAddress* address = Manager::host->StringToAddress(new String(AddressString),af2(AddressFamily));
				unsigned char buf __gc[] = address->GetAddressBytes();
				sa6->sin6_addr.u.Byte[0] = buf[0];
				sa6->sin6_addr.u.Byte[1] = buf[1];
				sa6->sin6_addr.u.Byte[2] = buf[2];
				sa6->sin6_addr.u.Byte[3] = buf[3];
				sa6->sin6_addr.u.Byte[4] = buf[4];
				sa6->sin6_addr.u.Byte[5] = buf[5];
				sa6->sin6_addr.u.Byte[6] = buf[6];
				sa6->sin6_addr.u.Byte[7] = buf[7];
				sa6->sin6_addr.u.Byte[8] = buf[8];
				sa6->sin6_addr.u.Byte[9] = buf[9];
				sa6->sin6_addr.u.Byte[10] = buf[10];
				sa6->sin6_addr.u.Byte[11] = buf[11];
				sa6->sin6_addr.u.Byte[12] = buf[12];
				sa6->sin6_addr.u.Byte[13] = buf[13];
				sa6->sin6_addr.u.Byte[14] = buf[14];
				sa6->sin6_addr.u.Byte[15] = buf[15];
				sa6->sin6_family = AF_INET6;
				sa6->sin6_flowinfo = 0;
				sa6->sin6_port = 0;
				sa6->sin6_scope_id = (u_long)address->ScopeId;
			}
			*lpAddressLength = sizeof(sockaddr_in6);
			return 0;
		default:
			return SOCKET_ERROR;
		}
	}

	static LONG	refcount = 0;

	int	WSPAPI WSPCleanup(LPINT	lpErrno) {
		if(InterlockedDecrement(&refcount)>0) {
			return 0;
		} else {
			std::cout << "shellnet data transport service provider is going to shutdown." << std::endl;
			Manager::peer->Cleanup();
			__crt_dll_terminate();
			return 0;
		}
	}

	int	WSPAPI GetProcedureTable(LPWSAPROTOCOL_INFO lpProtocolInfo, WSPUPCALLTABLE UpcallTable, LPWSPPROC_TABLE lpProcTable) {

		std::wcout << L"procedure set: " << __FUNCTION__ << std::endl;
		ProtocolInfo = *lpProtocolInfo;
		::PROTOCOL::UpcallTable = UpcallTable;

		lpProcTable->lpWSPAccept = WSPAccept;
		lpProcTable->lpWSPAddressToString =	WSPAddressToString;
		lpProcTable->lpWSPAsyncSelect =	WSPAsyncSelect;
		lpProcTable->lpWSPBind = WSPBind;
		lpProcTable->lpWSPCancelBlockingCall = WSPCancelBlockingCall;
		lpProcTable->lpWSPCleanup =	WSPCleanup;
		lpProcTable->lpWSPCloseSocket =	WSPCloseSocket;
		lpProcTable->lpWSPConnect =	WSPConnect;
		lpProcTable->lpWSPDuplicateSocket =	WSPDuplicateSocket;
		lpProcTable->lpWSPEnumNetworkEvents	= WSPEnumNetworkEvents;
		lpProcTable->lpWSPEventSelect =	WSPEventSelect;
		lpProcTable->lpWSPGetOverlappedResult =	WSPGetOverlappedResult;
		lpProcTable->lpWSPGetPeerName =	WSPGetPeerName;
		lpProcTable->lpWSPGetSockName =	WSPGetSockName;
		lpProcTable->lpWSPGetSockOpt = WSPGetSockOpt;
		lpProcTable->lpWSPGetQOSByName = WSPGetQOSByName;
		lpProcTable->lpWSPIoctl	= WSPIoctl;
		lpProcTable->lpWSPJoinLeaf = WSPJoinLeaf;
		lpProcTable->lpWSPListen = WSPListen;
		lpProcTable->lpWSPRecv = WSPRecv;
		lpProcTable->lpWSPRecvDisconnect = WSPRecvDisconnect;
		lpProcTable->lpWSPRecvFrom = WSPRecvFrom;
		lpProcTable->lpWSPSelect = WSPSelect;
		lpProcTable->lpWSPSend = WSPSend;
		lpProcTable->lpWSPSendDisconnect = WSPSendDisconnect;
		lpProcTable->lpWSPSendTo = WSPSendTo;
		lpProcTable->lpWSPSetSockOpt = WSPSetSockOpt;
		lpProcTable->lpWSPShutdown = WSPShutdown;
		lpProcTable->lpWSPSocket = WSPSocket;
		lpProcTable->lpWSPStringToAddress =	WSPStringToAddress;

		return 0;

	}

}
