#include "stdafx.h"
#include <string>
#include <vector>
#include <fstream>
#include <iostream>
#include <algorithm>
#include <process.h>
#include <shlwapi.h>

using namespace std;


#undef PROTOCOL
#define PROTOCOL IPv4TCP
#include "spsp.hpp"

#undef PROTOCOL
#define PROTOCOL IPv4UDP
#include "spsp.hpp"

#undef PROTOCOL
#define PROTOCOL IPv6TCP
#include "spsp.hpp"

#undef PROTOCOL
#define PROTOCOL IPv6UDP
#include "spsp.hpp"


static wchar_t szModulePath[MAX_PATH];
static std::vector<std::wstring> Targets;


BOOL APIENTRY DllMain(HANDLE hModule, DWORD  ul_reason_for_call, LPVOID lpReserved) {
	switch (ul_reason_for_call) {
	case DLL_PROCESS_ATTACH:
		wcout << L"shellnet pseudo service provider is attached to process" << endl;
		GetModuleFileName((HMODULE)hModule,szModulePath,sizeof(szModulePath));
		char szConfigPath[MAX_PATH];
		GetModuleFileNameA((HMODULE)hModule,szConfigPath,sizeof(szConfigPath));
		strcat(szConfigPath,".ini");
		if(PathFileExistsA(szConfigPath)) {
			std::wifstream f(szConfigPath);
			wchar_t line[MAX_PATH];
			while(true) {
				f.getline(line,sizeof(line));
				if(f.fail()) break;
				wcout << L"   " << line << endl;
				wcslwr(line);
				Targets.push_back(std::wstring(line));
			}
		}
		return TRUE;
	case DLL_THREAD_ATTACH:
		return TRUE;
	case DLL_THREAD_DETACH:
		return TRUE;
	case DLL_PROCESS_DETACH:
		wcout << L"spsp is detached from process" << endl;
		if(IPv4TCP::protocol.ProviderModule!=NULL) FreeLibrary(IPv4TCP::protocol.ProviderModule);
		if(IPv4UDP::protocol.ProviderModule!=NULL) FreeLibrary(IPv4UDP::protocol.ProviderModule);
		if(IPv6TCP::protocol.ProviderModule!=NULL) FreeLibrary(IPv6TCP::protocol.ProviderModule);
		if(IPv6UDP::protocol.ProviderModule!=NULL) FreeLibrary(IPv6UDP::protocol.ProviderModule);
		return TRUE;
	default:
		return FALSE;
	}
}


extern int WSPAPI WSPStartup(
	WORD wVersionRequested,
	LPWSPDATA lpWSPData,
	LPWSAPROTOCOL_INFO lpProtocolInfo,
	WSPUPCALLTABLE UpcallTable,
	LPWSPPROC_TABLE	lpProcTable)
{

	wcout << L"spsp <" << lpProtocolInfo->szProtocol << "> starts up" << endl;

	BaseProtocol* baseprotocol;
	LPWSPSTARTUP wspstartup;
	switch(lpProtocolInfo->iAddressFamily) {
	case AF_INET:
		if(lpProtocolInfo->iSocketType==SOCK_STREAM || lpProtocolInfo->iProtocol==IPPROTO_TCP) {
			baseprotocol = &IPv4TCP::protocol;
			wspstartup = IPv4TCP::WSPStartup;
			IPv4TCP::upcalls = UpcallTable;
		} else if(lpProtocolInfo->iSocketType==SOCK_DGRAM || lpProtocolInfo->iProtocol==IPPROTO_UDP) {
			baseprotocol = &IPv4UDP::protocol;
			wspstartup = IPv4UDP::WSPStartup;
			IPv4UDP::upcalls = UpcallTable;
		} else {
			return -1;
		}
		break;
	case AF_INET6:
		if(lpProtocolInfo->iSocketType==SOCK_STREAM || lpProtocolInfo->iProtocol==IPPROTO_TCP) {
			baseprotocol = &IPv6TCP::protocol;
			wspstartup = IPv6TCP::WSPStartup;
			IPv6TCP::upcalls = UpcallTable;
		} else if(lpProtocolInfo->iSocketType==SOCK_DGRAM || lpProtocolInfo->iProtocol==IPPROTO_UDP) {
			baseprotocol = &IPv6UDP::protocol;
			wspstartup = IPv6UDP::WSPStartup;
			IPv6UDP::upcalls = UpcallTable;
		} else {
			return -1;
		}
		break;
	default:
		return -1;
	}

	baseprotocol->ProtocolInfo = *lpProtocolInfo;

	if(baseprotocol->ProviderModule==NULL) {
		wchar_t szProviderPath[MAX_PATH];
		GetModuleFileName(NULL,szProviderPath,sizeof(szProviderPath));
		wcslwr(szProviderPath);
		std::wcerr << L"szProviderPath: " << szProviderPath << std::endl;
		std::vector<std::wstring>::iterator it;
		it = std::find(Targets.begin(),Targets.end(),std::wstring(szProviderPath));
		if(it!=Targets.end()) {
			wcscpy(szProviderPath,szModulePath);
			wcscpy(wcsrchr(szProviderPath,'\\')+1,L"ssp.dll");
		} else if(lpProtocolInfo->ProtocolChain.ChainLen==1) {
			// Base Protocol but it's not the target.
			std::wcerr << L"this protocol is not chained." << std::endl;
			return -1;
		} else {
			// Layered Protocol
			WSAPROTOCOL_INFO* protocols;
			DWORD size;
			int errno;
			WSCEnumProtocols(NULL,NULL,&size,&errno);
			protocols = new WSAPROTOCOL_INFO[size/sizeof(WSAPROTOCOL_INFO)];
			int n = WSCEnumProtocols(NULL,protocols,&size,&errno);
			wcout << L"spsp is searching for protocol#" << lpProtocolInfo->ProtocolChain.ChainEntries[1] << L"." << endl;
			wcout << L"spsp enumerated " << n << L" protocols." << endl;
			for(int i=0; i<n; ++i) {
				if(protocols[i].dwCatalogEntryId==lpProtocolInfo->ProtocolChain.ChainEntries[1]) {
					baseprotocol->ProtocolInfo = protocols[i];
					wchar_t path[MAX_PATH];
					INT size = sizeof(path);
					WSCGetProviderPath(&baseprotocol->ProtocolInfo.ProviderId,path,&size,&errno);
					::ExpandEnvironmentStrings(path,szProviderPath,sizeof(szProviderPath));
				}
			}
			delete [] protocols;
		}

		wcout << L"spsp try to load " << szProviderPath << endl;
		baseprotocol->ProviderModule = LoadLibrary(szProviderPath);
	}

	if(baseprotocol->ProviderModule==NULL) {
		wcout << L"spsp failed to load the base protocol provider." << endl;
		return -1;
	} else {
		baseprotocol->WSAStartup = (LPWSPSTARTUP)GetProcAddress(baseprotocol->ProviderModule,"WSPStartup");
		if(baseprotocol->WSAStartup==NULL) {
			wcout << L"spsp failed to get the base protocol WSPStartup." << endl;
			FreeLibrary(baseprotocol->ProviderModule);
			baseprotocol->ProviderModule = NULL;
			return -1;
		} else {
			return wspstartup(wVersionRequested,lpWSPData,lpProtocolInfo,UpcallTable,lpProcTable);
		}
	}

}
