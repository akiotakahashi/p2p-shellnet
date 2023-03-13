#if !defined(PROTOCOL)
#error Undefined PROTOCOL
#endif


#if !defined(BASEPROTOCOL)
#define BASEPROTOCOL
struct BaseProtocol {
	WSAPROTOCOL_INFO ProtocolInfo;;
	HMODULE ProviderModule;
	LPWSPSTARTUP WSAStartup;
	WSPPROC_TABLE DispatchTable;
	BaseProtocol() {
		ProtocolInfo.dwCatalogEntryId = -1;
		ProviderModule = NULL;
	}
};
#define WIDEN2(x) L ## x
#define WIDEN(x) WIDEN2(#x)
#define DISPFUNC(x) protocol.DispatchTable.lp ## x
#define RETURN_INVALID_STATE(rv) \
	if(protocol.ProviderModule==NULL) {*lpErrno=WSAENETDOWN; return rv;} \
	if(DISPFUNC(__FUNCTION__)==NULL) {*lpErrno=WSAENETDOWN; return rv;}
#endif

namespace PROTOCOL {

	BaseProtocol protocol;
	WSPUPCALLTABLE upcalls;

	/*

	static SOCKET WSPAPI WSPAccept(
		SOCKET s,
		struct sockaddr	FAR	* addr,
		LPINT addrlen,
		LPCONDITIONPROC	lpfnCondition,
		DWORD_PTR dwCallbackData,
		LPINT lpErrno)
	{
		if(protocol.ProviderModule==NULL) {*lpErrno=WSAENETDOWN; return INVALID_SOCKET;}
		if(protocol.DispatchTable.lpWSPAccept==NULL) {*lpErrno=WSAENETDOWN; return INVALID_SOCKET;}
		return protocol.DispatchTable.lpWSPAccept(s,addr,addrlen,lpfnCondition,dwCallbackData,lpErrno);
	}

	static int WSPAPI WSPAsyncSelect(SOCKET s, HWND hWnd, unsigned int wMsg, long lEvent, LPINT lpErrno) {
		if(protocol.ProviderModule==NULL) {*lpErrno=WSAENETDOWN; return SOCKET_ERROR;}
		if(protocol.DispatchTable.lpWSPAsyncSelect==NULL) {*lpErrno=WSAENETDOWN; return SOCKET_ERROR;}
		return protocol.DispatchTable.lpWSPAsyncSelect(s,hWnd,wMsg,lEvent,lpErrno);
	}

	static INT WSPAPI WSPAddressToString(
		LPSOCKADDR lpsaAddress,
		DWORD dwAddressLength,
		LPWSAPROTOCOL_INFOW	lpProtocolInfo,
		LPWSTR lpszAddressString,
		LPDWORD	lpdwAddressStringLength,
		LPINT lpErrno)
	{
		if(protocol.ProviderModule==NULL) {*lpErrno=WSAENETDOWN; return SOCKET_ERROR;}
		if(protocol.DispatchTable.lpWSPAddressToString==NULL) {*lpErrno=WSAENETDOWN; return SOCKET_ERROR;}
		return protocol.DispatchTable.lpWSPAddressToString(lpsaAddress,dwAddressLength,lpProtocolInfo,lpszAddressString,lpdwAddressStringLength,lpErrno);
	}

	static int WSPAPI WSPBind(SOCKET s, const struct sockaddr FAR * name, int namelen, LPINT lpErrno) {
		if(protocol.ProviderModule==NULL) {*lpErrno=WSAENETDOWN; return SOCKET_ERROR;}
		if(protocol.DispatchTable.lpWSPBind==NULL) {*lpErrno=WSAENETDOWN; return SOCKET_ERROR;}
		return protocol.DispatchTable.lpWSPBind(s,name,namelen,lpErrno);
	}

	static int WSPAPI WSPCancelBlockingCall(LPINT lpErrno) {
		if(protocol.ProviderModule==NULL) {*lpErrno=WSAENETDOWN; return SOCKET_ERROR;}
		if(protocol.DispatchTable.lpWSPCancelBlockingCall==NULL) {*lpErrno=WSAENETDOWN; return SOCKET_ERROR;}
		return protocol.DispatchTable.lpWSPCancelBlockingCall(lpErrno);
	}

	static int WSPAPI WSPCleanup(LPINT lpErrno) {
		wcout << L"shellnet data transport service provider is going to shutdown." << endl;
		if(protocol.ProviderModule==NULL) {*lpErrno=WSAENETDOWN; return SOCKET_ERROR;}
		if(protocol.DispatchTable.lpWSPCleanup==NULL) {*lpErrno=WSAENETDOWN; return SOCKET_ERROR;}
		return protocol.DispatchTable.lpWSPCleanup(lpErrno);
	}

	static int WSPAPI WSPCloseSocket(SOCKET s, LPINT lpErrno) {
		if(protocol.ProviderModule==NULL) {*lpErrno=WSAENETDOWN; return SOCKET_ERROR;}
		if(protocol.DispatchTable.lpWSPCloseSocket==NULL) {*lpErrno=WSAENETDOWN; return SOCKET_ERROR;}
		return protocol.DispatchTable.lpWSPCloseSocket(s,lpErrno);
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
		if(protocol.ProviderModule==NULL) {*lpErrno=WSAENETDOWN; return SOCKET_ERROR;}
		if(protocol.DispatchTable.lpWSPConnect==NULL) {*lpErrno=WSAENETDOWN; return SOCKET_ERROR;}
		return protocol.DispatchTable.lpWSPConnect(s,name,namelen,lpCallerData,lpCalleeData,lpSQOS,lpGQOS,lpErrno);
	}

	static int WSPAPI WSPDuplicateSocket(
		SOCKET s,
		DWORD dwProcessId,
		LPWSAPROTOCOL_INFO lpProtocolInfo,
		LPINT lpErrno)
	{
		if(protocol.ProviderModule==NULL) {*lpErrno=WSAENETDOWN; return SOCKET_ERROR;}
		if(protocol.DispatchTable.lpWSPDuplicateSocket==NULL) {*lpErrno=WSAENETDOWN; return SOCKET_ERROR;}
		return protocol.DispatchTable.lpWSPDuplicateSocket(s,dwProcessId,lpProtocolInfo,lpErrno);
	}

	static int WSPAPI WSPEnumNetworkEvents(
		SOCKET s,
		WSAEVENT hEventObject,
		LPWSANETWORKEVENTS lpNetworkEvents,
		LPINT lpErrno)
	{
		if(protocol.ProviderModule==NULL) {*lpErrno=WSAENETDOWN; return SOCKET_ERROR;}
		if(protocol.DispatchTable.lpWSPEnumNetworkEvents==NULL) {*lpErrno=WSAENETDOWN; return SOCKET_ERROR;}
		return protocol.DispatchTable.lpWSPEnumNetworkEvents(s,hEventObject,lpNetworkEvents,lpErrno);
	}

	static int WSPAPI WSPEventSelect(
		SOCKET s,
		WSAEVENT hEventObject,
		long lNetworkEvents,
		LPINT lpErrno)
	{
		if(protocol.ProviderModule==NULL) {*lpErrno=WSAENETDOWN; return SOCKET_ERROR;}
		if(protocol.DispatchTable.lpWSPEventSelect==NULL) {*lpErrno=WSAENETDOWN; return SOCKET_ERROR;}
		return protocol.DispatchTable.lpWSPEventSelect(s,hEventObject,lNetworkEvents,lpErrno);
	}

	static BOOL WSPAPI WSPGetOverlappedResult(
		SOCKET s,
		LPWSAOVERLAPPED lpOverlapped,
		LPDWORD lpcbTransfer,
		BOOL fWait,
		LPDWORD lpdwFlags,
		LPINT lpErrno)
	{
		if(protocol.ProviderModule==NULL) {*lpErrno=WSAENETDOWN; return FALSE;}
		if(protocol.DispatchTable.lpWSPGetOverlappedResult==NULL) {*lpErrno=WSAENETDOWN; return SOCKET_ERROR;}
		return protocol.DispatchTable.lpWSPGetOverlappedResult(s,lpOverlapped,lpcbTransfer,fWait,lpdwFlags,lpErrno);
	}

	static int WSPAPI WSPGetPeerName(
		SOCKET s,
		struct sockaddr	FAR	* name,
		LPINT namelen,
		LPINT lpErrno)
	{
		if(protocol.ProviderModule==NULL) {*lpErrno=WSAENETDOWN; return SOCKET_ERROR;}
		if(protocol.DispatchTable.lpWSPGetPeerName==NULL) {*lpErrno=WSAENETDOWN; return SOCKET_ERROR;}
		return protocol.DispatchTable.lpWSPGetPeerName(s,name,namelen,lpErrno);
	}

	static int WSPAPI WSPGetSockName(
		SOCKET s,
		struct sockaddr	FAR	* name,
		LPINT namelen,
		LPINT lpErrno)
	{
		if(protocol.ProviderModule==NULL) {*lpErrno=WSAENETDOWN; return SOCKET_ERROR;}
		if(protocol.DispatchTable.lpWSPGetSockName==NULL) {*lpErrno=WSAENETDOWN; return SOCKET_ERROR;}
		return protocol.DispatchTable.lpWSPGetSockName(s,name,namelen,lpErrno);
	}

	static int WSPAPI WSPGetSockOpt(
		SOCKET s,
		int level,
		int optname,
		char* optval,
		LPINT optlen,
		LPINT lpErrno)
	{
		if(protocol.ProviderModule==NULL) {*lpErrno=WSAENETDOWN; return SOCKET_ERROR;}
		if(protocol.DispatchTable.lpWSPGetSockOpt==NULL) {*lpErrno=WSAENETDOWN; return SOCKET_ERROR;}
		return protocol.DispatchTable.lpWSPGetSockOpt(s,level,optname,optval,optlen,lpErrno);
	}

	static BOOL WSPAPI WSPGetQOSByName(
		SOCKET s,
		LPWSABUF lpQOSName,
		LPQOS lpQOS,
		LPINT lpErrno)
	{
		if(protocol.ProviderModule==NULL) {*lpErrno=WSAENETDOWN; return SOCKET_ERROR;}
		if(protocol.DispatchTable.lpWSPGetQOSByName==NULL) {*lpErrno=WSAENETDOWN; return SOCKET_ERROR;}
		return protocol.DispatchTable.lpWSPGetQOSByName(s,lpQOSName,lpQOS,lpErrno);
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
		if(protocol.ProviderModule==NULL) {*lpErrno=WSAENETDOWN; return SOCKET_ERROR;}
		if(protocol.DispatchTable.lpWSPIoctl==NULL) {*lpErrno=WSAENETDOWN; return SOCKET_ERROR;}
		return protocol.DispatchTable.lpWSPIoctl(s,dwIoControlCode,lpvInBuffer,cbInBuffer,lpvOutBuffer,cbOutBuffer,lpcbBytesReturned,lpOverlapped,lpCompletionRoutine,lpThreadId,lpErrno);
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
		if(protocol.ProviderModule==NULL) {*lpErrno=WSAENETDOWN; return INVALID_SOCKET;}
		if(protocol.DispatchTable.lpWSPJoinLeaf==NULL) {*lpErrno=WSAENETDOWN; return SOCKET_ERROR;}
		return protocol.DispatchTable.lpWSPJoinLeaf(s,name,namelen,lpCallerData,lpCalleeData,lpSQOS,lpGQOS,dwFlags,lpErrno);
	}

	static int WSPAPI WSPListen(SOCKET s, int backlog, LPINT lpErrno) {
		if(protocol.ProviderModule==NULL) {*lpErrno=WSAENETDOWN; return SOCKET_ERROR;}
		if(protocol.DispatchTable.lpWSPListen==NULL) {*lpErrno=WSAENETDOWN; return SOCKET_ERROR;}
		return protocol.DispatchTable.lpWSPListen(s,backlog,lpErrno);
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
		if(protocol.ProviderModule==NULL) {*lpErrno=WSAENETDOWN; return SOCKET_ERROR;}
		if(protocol.DispatchTable.lpWSPRecv==NULL) {*lpErrno=WSAENETDOWN; return SOCKET_ERROR;}
		return protocol.DispatchTable.lpWSPRecv(s,lpBuffers,dwBufferCount,lpNumberOfBytesRecvd,lpFlags,lpOverlapped,lpCompletionRoutine,lpThreadId,lpErrno);
	}

	static int WSPAPI WSPRecvDisconnect(SOCKET s, LPWSABUF lpInboundDisconnectData, LPINT lpErrno) {
		if(protocol.ProviderModule==NULL) {*lpErrno=WSAENETDOWN; return SOCKET_ERROR;}
		if(protocol.DispatchTable.lpWSPRecvDisconnect==NULL) {*lpErrno=WSAENETDOWN; return SOCKET_ERROR;}
		return protocol.DispatchTable.lpWSPRecvDisconnect(s,lpInboundDisconnectData,lpErrno);
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
		if(protocol.ProviderModule==NULL) {*lpErrno=WSAENETDOWN; return SOCKET_ERROR;}
		if(protocol.DispatchTable.lpWSPRecvFrom==NULL) {*lpErrno=WSAENETDOWN; return SOCKET_ERROR;}
		return protocol.DispatchTable.lpWSPRecvFrom(s,lpBuffers,dwBufferCount,lpNumberOfBytesRecvd,lpFlags,lpFrom,lpFromlen,lpOverlapped,lpCompletionRoutine,lpThreadId,lpErrno);
	}

	static int WSPAPI WSPSelect(
		int nfds,
		fd_set* readfds,
		fd_set* writefds,
		fd_set* exceptfds,
		const struct timeval* timeout,
		LPINT lpErrno)
	{
		if(protocol.ProviderModule==NULL) {*lpErrno=WSAENETDOWN; return SOCKET_ERROR;}
		if(protocol.DispatchTable.lpWSPSelect==NULL) {*lpErrno=WSAENETDOWN; return SOCKET_ERROR;}
		SOCKET s;
		if(readfds->fd_count>0) {
			s = readfds->fd_array[0];
		} else if(writefds->fd_count>0) {
			s = writefds->fd_array[0];
		} else if(exceptfds->fd_count>0) {
			s = exceptfds->fd_array[0];
		} else {
			return SOCKET_ERROR;
		}
		return protocol.DispatchTable.lpWSPSelect(nfds,readfds,writefds,exceptfds,timeout,lpErrno);
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
		if(protocol.ProviderModule==NULL) {*lpErrno=WSAENETDOWN; return SOCKET_ERROR;}
		if(protocol.DispatchTable.lpWSPSend==NULL) {*lpErrno=WSAENETDOWN; return SOCKET_ERROR;}
		return protocol.DispatchTable.lpWSPSend(s,lpBuffers,dwBufferCount,lpNumberOfBytesSent,dwFlags,lpOverlapped,lpCompletionRoutine,lpThreadId,lpErrno);
	}

	static int WSPAPI WSPSendDisconnect(SOCKET s, LPWSABUF lpOutboundDisconnectData, LPINT lpErrno) {
		if(protocol.ProviderModule==NULL) {*lpErrno=WSAENETDOWN; return SOCKET_ERROR;}
		if(protocol.DispatchTable.lpWSPSendDisconnect==NULL) {*lpErrno=WSAENETDOWN; return SOCKET_ERROR;}
		return protocol.DispatchTable.lpWSPSendDisconnect(s,lpOutboundDisconnectData,lpErrno);
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
		if(protocol.ProviderModule==NULL) {*lpErrno=WSAENETDOWN; return SOCKET_ERROR;}
		if(protocol.DispatchTable.lpWSPSendTo==NULL) {*lpErrno=WSAENETDOWN; return SOCKET_ERROR;}
		return protocol.DispatchTable.lpWSPSendTo(s,lpBuffers,dwBufferCount,lpNumberOfBytesSent,dwFlags,lpTo,iTolen,lpOverlapped,lpCompletionRoutine,lpThreadId,lpErrno);
	}

	static int WSPAPI WSPSetSockOpt(
		SOCKET s,
		int level,
		int optname,
		const char* optval,
		int optlen,
		LPINT lpErrno)
	{
		if(protocol.ProviderModule==NULL) {*lpErrno=WSAENETDOWN; return SOCKET_ERROR;}
		if(protocol.DispatchTable.lpWSPSetSockOpt==NULL) {*lpErrno=WSAENETDOWN; return SOCKET_ERROR;}
		return protocol.DispatchTable.lpWSPSetSockOpt(s,level,optname,optval,optlen,lpErrno);
	}

	static int WSPAPI WSPShutdown(
		SOCKET s,
		int	how,
		LPINT lpErrno)
	{
		if(protocol.ProviderModule==NULL) {*lpErrno=WSAENETDOWN; return SOCKET_ERROR;}
		if(protocol.DispatchTable.lpWSPShutdown==NULL) {*lpErrno=WSAENETDOWN; return SOCKET_ERROR;}
		return protocol.DispatchTable.lpWSPShutdown(s,how,lpErrno);
	}

	static SOCKET WSPAPI WSPSocket(
		int	af,
		int	st,
		int	pt,
		LPWSAPROTOCOL_INFOW	lpProtocolInfo,
		GROUP g,
		DWORD dwFlags,
		LPINT lpErrno)
	{
		if(protocol.ProviderModule==NULL) {*lpErrno=WSAENETDOWN; return INVALID_SOCKET;}
		if(protocol.DispatchTable.lpWSPSocket==NULL) {*lpErrno=WSAENETDOWN; return INVALID_SOCKET;}
		return protocol.DispatchTable.lpWSPSocket(af,st,pt,lpProtocolInfo,g,dwFlags,lpErrno);
	}

	static INT WSPAPI WSPStringToAddress(
		LPWSTR AddressString,
		INT	AddressFamily,
		LPWSAPROTOCOL_INFOW	lpProtocolInfo,
		LPSOCKADDR lpAddress,
		LPINT lpAddressLength,
		LPINT lpErrno)
	{
		if(protocol.ProviderModule==NULL) {*lpErrno=WSAENETDOWN; return SOCKET_ERROR;}
		if(protocol.DispatchTable.lpWSPStringToAddress==NULL) {*lpErrno=WSAENETDOWN; return SOCKET_ERROR;}
		return protocol.DispatchTable.lpWSPStringToAddress(AddressString,AddressFamily,lpProtocolInfo,lpAddress,lpAddressLength,lpErrno);
	}

	*/

	static int WSPAPI WSPStartup(
		WORD wVersionRequested,
		LPWSPDATA lpWSPData,
		LPWSAPROTOCOL_INFO lpProtocolInfo,
		WSPUPCALLTABLE UpcallTable,
		LPWSPPROC_TABLE	lpProcTable)
	{

		int	reqMinor = HIBYTE(wVersionRequested);
		int	reqMajor = LOBYTE(wVersionRequested);

		/* Make	sure that the version requested	is >= 2.2.	*/
		/* The low byte	is the major version and the high	*/
		/* byte	is the minor version.						*/
		if(reqMajor>2 || reqMajor==2 &&	reqMinor>2)	{
			return WSAVERNOTSUPPORTED;
		}

		/* Since we	only support 2.2, set both wVersion	and	 */
		/* wHighVersion	to 2.2.								 */
		lpWSPData->wVersion	= MAKEWORD(2,2);
		lpWSPData->wHighVersion	= MAKEWORD(2,2);
		wcscpy(lpWSPData->szDescription, L"Shellnet	Pseudo Service Provider [" WIDEN(PROTOCOL) L"]");

		/*

		ZeroMemory(lpProcTable,	sizeof(*lpProcTable));
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

		*/

		return protocol.WSAStartup(wVersionRequested,lpWSPData,lpProtocolInfo,UpcallTable,lpProcTable);
	}

}
