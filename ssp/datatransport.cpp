#include "stdafx.h"
#include "ssp.h"
#include "manager.h"
#include "datatransport.h"


#undef PROTOCOL
#define PROTOCOL IPV4TCP
#include "datatransport.hpp"

#undef PROTOCOL
#define PROTOCOL IPV4UDP
#include "datatransport.hpp"

#undef PROTOCOL
#define PROTOCOL IPV6TCP
#include "datatransport.hpp"

#undef PROTOCOL
#define PROTOCOL IPV6UDP
#include "datatransport.hpp"


static LONG refcount = 0;

int	WSPAPI WSPStartup(
	WORD wVersionRequested,
	LPWSPDATA lpWSPData,
	LPWSAPROTOCOL_INFO lpProtocolInfo,
	WSPUPCALLTABLE UpcallTable,
	LPWSPPROC_TABLE	lpProcTable)
{

	if(InterlockedIncrement(&refcount)==1) {
		__crt_dll_initialize();
		std::wcout << L"ssp <" << lpProtocolInfo->szProtocol << L"> starts up" << std::endl;
	}

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
	wcscpy(lpWSPData->szDescription, L"Shellnet	Data Transport Service Provider");

	switch(lpProtocolInfo->iAddressFamily) {
	case AF_INET:
		switch(lpProtocolInfo->iSocketType) {
		case SOCK_STREAM:
			IPV4TCP::GetProcedureTable(lpProtocolInfo,UpcallTable,lpProcTable);
			break;
		case SOCK_DGRAM:
			IPV4UDP::GetProcedureTable(lpProtocolInfo,UpcallTable,lpProcTable);
			break;
		}
		break;
	case AF_INET6:
		switch(lpProtocolInfo->iSocketType) {
		case SOCK_STREAM:
			IPV6TCP::GetProcedureTable(lpProtocolInfo,UpcallTable,lpProcTable);
			break;
		case SOCK_DGRAM:
			IPV6UDP::GetProcedureTable(lpProtocolInfo,UpcallTable,lpProcTable);
			break;
		}
	}

	try {
		return Initialize();
	} catch(Exception* ex) {
		Console::Error->WriteLine(ex->ToString());
		return WSASYSNOTREADY;
	} catch(...) {
		std::wcerr << L"EXCEPTION CAUGHT" << std::endl;
		return WSASYSNOTREADY;
	}

}
