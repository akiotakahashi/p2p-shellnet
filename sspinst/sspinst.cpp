#include <winsock2.h>
#include <ws2tcpip.h>
#include <ws2spi.h>
#include <sporder.h>
#include <windows.h>
#include <shlwapi.h>
#include <iostream>
#include <fstream>
#include <vector>

using namespace std;


// {3DDFC745-3AA6-4699-B0ED-B5EF28182DAC}
static GUID GUID_SSP_LAYEREDPROTOCOL = 
{ 0x3ddfc745, 0x3aa6, 0x4699, { 0xb0, 0xed, 0xb5, 0xef, 0x28, 0x18, 0x2d, 0xac } };
// {36EC06D0-1FD4-4377-9529-9AC20D611419}
static GUID GUID_SSP_PROTOCOLCHAIN = 
{ 0x36ec06d0, 0x1fd4, 0x4377, { 0x95, 0x29, 0x9a, 0xc2, 0xd, 0x61, 0x14, 0x19 } };

extern int wmain(int argc, wchar_t *argv[]) {

	if(argc==1) {
		wcout << L"sspinst /r" << endl;
		wcout << L"sspinst /u" << endl;
		wcout << L"sspinst /free" << endl;
		wcout << L"sspinst /list" << endl;
		wcout << L"sspinst /set [path]" << endl;
		wcout << L"sspinst /unset [path]" << endl;
		return 0;
	}

	if(!wcsicmp(argv[1],L"/r")) {
		int errnum;
		WSAPROTOCOL_INFO protocols[2];
		for(int i=0; i<sizeof(protocols)/sizeof(protocols[0]); ++i) {
			protocols[i].dwCatalogEntryId = 0;
			protocols[i].iVersion = 1;
			protocols[i].dwProviderReserved = 0;
			protocols[i].iNetworkByteOrder = BIGENDIAN;
			protocols[i].iProtocolMaxOffset = 0;
			protocols[i].ProtocolChain.ChainLen = 1;
			protocols[i].dwServiceFlags1 = 0;
			protocols[i].dwServiceFlags2 = 0;
			protocols[i].dwServiceFlags3 = 0;
			protocols[i].dwServiceFlags4 = 0;
			protocols[i].dwProviderFlags = PFL_MATCHES_PROTOCOL_ZERO;
			protocols[i].iSecurityScheme = SECURITY_PROTOCOL_NONE;
			protocols[0].dwServiceFlags1 = 0;//XP1_IFS_HANDLES;
		}

		// IPv4 TCP
		wcscpy(protocols[0].szProtocol, L"Shellnet IPv4 TCP - Base Protocol"); 
		protocols[0].iAddressFamily = AF_INET;
		protocols[0].iSocketType = SOCK_STREAM;
		protocols[0].iProtocol = IPPROTO_TCP;
		protocols[0].dwMessageSize = 0;
		protocols[0].iMinSockAddr = sizeof(sockaddr_in);
		protocols[0].iMaxSockAddr = sizeof(sockaddr_in);
		protocols[0].dwServiceFlags1 |= XP1_GUARANTEED_DELIVERY|XP1_GUARANTEED_ORDER|XP1_GRACEFUL_CLOSE|XP1_EXPEDITED_DATA;
		// IPv4 UDP
		wcscpy(protocols[1].szProtocol, L"Shellnet IPv4 UDP - Base Protocol"); 
		protocols[1].iAddressFamily = AF_INET;
		protocols[1].iSocketType = SOCK_DGRAM;
		protocols[1].iProtocol = IPPROTO_UDP;
		protocols[1].dwMessageSize = 65467;
		protocols[1].iMinSockAddr = sizeof(sockaddr_in);
		protocols[1].iMaxSockAddr = sizeof(sockaddr_in);
		protocols[1].dwServiceFlags1 |= XP1_CONNECTIONLESS|XP1_MESSAGE_ORIENTED|XP1_SUPPORT_BROADCAST|XP1_SUPPORT_MULTIPOINT;
		/*
		// IPv6 TCP
		wcscpy(protocols[2].szProtocol, L"Shellnet IPv6 TCP - Base Protocol");
		protocols[2].iAddressFamily = AF_INET6;
		protocols[2].iSocketType = SOCK_STREAM;
		protocols[2].iProtocol = IPPROTO_TCP;
		protocols[2].dwMessageSize = 0;
		protocols[2].iMinSockAddr = sizeof(sockaddr_in6);
		protocols[2].iMaxSockAddr = sizeof(sockaddr_in6);
		protocols[2].dwServiceFlags1 = protocols[0].dwServiceFlags1;
		// IPv6 UDP
		wcscpy(protocols[3].szProtocol, L"Shellnet IPv6 UDP - Base Protocol"); 
		protocols[3].iAddressFamily = AF_INET6;
		protocols[3].iSocketType = SOCK_DGRAM;
		protocols[3].iProtocol = IPPROTO_UDP;
		protocols[3].dwMessageSize = 65467;
		protocols[3].iMinSockAddr = sizeof(sockaddr_in6);
		protocols[3].iMaxSockAddr = sizeof(sockaddr_in6);
		protocols[3].dwServiceFlags1 = protocols[1].dwServiceFlags1;
		*/

		// Install our provider
		wchar_t szPath[MAX_PATH];
		GetModuleFileName(NULL,szPath,sizeof(szPath));
		wcscpy(wcsrchr(szPath,'\\')+1,L"spsp.dll");
		int ret = WSCInstallProvider(&GUID_SSP_LAYEREDPROTOCOL, szPath, protocols, sizeof(protocols)/sizeof(protocols[0]), &errnum);
		if(ret!=0) {
			std::wcout << ret << std::endl;
			return 1;
		}

		wcout << L"successfully installed layered protocols" << endl;

		DWORD size = 0;
		WSCEnumProtocols(NULL,NULL,&size,&errno);
		WSAPROTOCOL_INFO* buf = new WSAPROTOCOL_INFO[size/sizeof(WSAPROTOCOL_INFO)+4];
		int n = WSCEnumProtocols(NULL,buf,&size,&errnum);

		if(n<4) return 0;
		wcout << L"successfully enumerated installed " << n << L" data transport providers" << endl;

		for(int i=0; i<sizeof(protocols)/sizeof(protocols[0]); ++i) {
			protocols[i].dwServiceFlags4 = 0;
			for(int k=n-1; k>=0; --k) {
				if(protocols[i].iAddressFamily==buf[k].iAddressFamily
				&& protocols[i].iSocketType==buf[k].iSocketType
				&& protocols[i].iProtocol==buf[k].iProtocol)
				{
					if(protocols[i].ProviderId==buf[k].ProviderId) {
						protocols[i].dwCatalogEntryId = buf[k].dwCatalogEntryId;
					} else {
						protocols[i].dwServiceFlags4 = buf[k].dwCatalogEntryId;
					}
				}
			}
		}

		int skipped = 0;
		for(int i=sizeof(protocols)/sizeof(protocols[0])-1; i>=0; --i) {
			protocols[i].ProtocolChain.ChainEntries[0] = protocols[i].dwCatalogEntryId;
			protocols[i].ProtocolChain.ChainEntries[1] = protocols[i].dwServiceFlags4;
			protocols[i].dwServiceFlags4 = 0;
			if(protocols[i].ProtocolChain.ChainEntries[1]==0
///			|| protocols[i].iAddressFamily==AF_INET6
			|| protocols[i].iSocketType==SOCK_DGRAM) {
				memmove(&protocols[i],&protocols[i+1],sizeof(protocols[0])*(sizeof(protocols)/sizeof(protocols[0])-(i+1)));
				++skipped;
			} else {
				wcscpy(wcsstr(protocols[i].szProtocol,L"- "),L"- Protocolchain");
				protocols[i].ProtocolChain.ChainLen = 2;
			}
		}

		ret = WSCInstallProvider(&GUID_SSP_PROTOCOLCHAIN,szPath,protocols,sizeof(protocols)/sizeof(protocols[0])-skipped,&errnum);
		if(ret==SOCKET_ERROR) {
			wcout << L"failed to install: " << dec << errno << endl;
		} else {
			wcout << L"successfully install protocol chains" << endl;
		}

		WSCEnumProtocols(NULL,NULL,&size,&errnum);
		n = WSCEnumProtocols(NULL,buf,&size,&errnum);
		for(int i=0; i<sizeof(protocols)/sizeof(protocols[0]); ++i) {
			for(int k=n-1; k>=i+1; --k) {
				if(buf[k].ProviderId==GUID_SSP_PROTOCOLCHAIN
//				&& buf[k].iAddressFamily==AF_INET
				&& 0!=(buf[k].iSocketType&SOCK_STREAM)) {
					WSAPROTOCOL_INFO tmp = buf[k-1];
					buf[k-1] = buf[k];
					buf[k] = tmp;
				}
			}
		}

		DWORD catids[100];
		for(int i=0; i<n; ++i) {
			catids[i] = buf[i].dwCatalogEntryId;
		}
		int r;
		switch(r=WSCWriteProviderOrder(catids,n)) {
		case ERROR_SUCCESS:
			std::wcout << L"successfully reordered protocols" << std::endl;
			break;
		case ERROR_BUSY:
			std::wcout << L"busy" << std::endl;
			break;
		default:
			std::wcout << L"failed to reorder with error code ";
			std::wcout << std::dec << r << std::endl;
			break;
		}
	} else if(!wcsicmp(argv[1],L"/u")) {
		int errnum;
		int ret = 0;
		ret += WSCDeinstallProvider(&GUID_SSP_LAYEREDPROTOCOL, &errnum);
		ret += WSCDeinstallProvider(&GUID_SSP_PROTOCOLCHAIN, &errnum);
		std::wcout << ret << std::endl;
		return 0;
	} else if(!wcsicmp(argv[1],L"/free")) {
		HANDLE hEvent = OpenEvent(EVENT_MODIFY_STATE,FALSE,L"spsp-release-ssp-signal");
		if(hEvent==NULL) {
			wcout << L"failed to send a releasing signal." << endl;
		} else {
			PulseEvent(hEvent);
			CloseHandle(hEvent);
		}
	} else if(!wcsicmp(argv[1],L"/set") || !wcsicmp(argv[1],L"/unset") || !wcsicmp(argv[1],L"/list")) {
		wchar_t line[256];
		std::vector<std::wstring> targets;
		wifstream fsrc("spsp.dll.ini");
		for(;;) {
			fsrc.getline(line,sizeof(line));
			if(fsrc.fail()) break;
			targets.push_back(std::wstring(line));
		}
		fsrc.close();
		if(!wcsicmp(argv[1],L"/list")) {
			std::vector<std::wstring>::iterator it;
			for(it=targets.begin(); it!=targets.end(); ++it) {
				wcout << it->c_str() << endl;
			}
		} else {
			if(!wcsicmp(argv[1],L"/set")) {
				targets.push_back(std::wstring(argv[2]));
			} else if(!wcsicmp(argv[1],L"/unset")) {
				std::vector<std::wstring>::iterator it;
				for(it=targets.begin(); it!=targets.end(); ++it) {
					if(!wcsicmp(it->c_str(),argv[2])) {
						targets.erase(it);
						break;
					}
				}
			}
			wofstream fdst("spsp.dll.ini");
			std::vector<std::wstring>::iterator it;
			for(it=targets.begin(); it!=targets.end(); ++it) {
				fdst << it->c_str() << endl;
			}
		}
	} else {
		wcerr << L"invalid arguments" << endl;
		return 0;
	}

}
