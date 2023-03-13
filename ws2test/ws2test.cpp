#include "stdafx.h"


// {36EC06D0-1FD4-4377-9529-9AC20D611419}
static GUID GUID_SSP = 
{ 0x36ec06d0, 0x1fd4, 0x4377, { 0x95, 0x29, 0x9a, 0xc2, 0xd, 0x61, 0x14, 0x19 } };

extern int wmain(int argc, wchar_t *argv[]) {

	WSADATA wsaData;
	int wsret = WSAStartup(MAKEWORD(2,2),&wsaData);
	if(wsret!=0) {
		std::wcout << wsret << std::endl;
		return -1;
	}

	while(true) {

		WSAPROTOCOL_INFO protocols[120];
		DWORD size = sizeof(protocols);
		int errno;
		int n = WSCEnumProtocols(NULL,protocols,&size,&errno);
		//std::wcout << n << "/" << WSAGetLastError() << std::endl;
		for(int i=0; i<n; ++i) {
			std::wcout << std::dec << protocols[i].dwCatalogEntryId << ":" << protocols[i].szProtocol << std::endl;
			/*
			std::wcout << "sz:" << protocols[i].szProtocol << std::endl;
			std::wcout << "id:" << std::dec << protocols[i].dwCatalogEntryId << std::endl;
			std::wcout << "af:" << std::dec << protocols[i].iAddressFamily << std::endl;
			std::wcout << "st:" << std::dec << protocols[i].iSocketType << std::endl;
			std::wcout << "pt:" << std::dec << protocols[i].iProtocol << std::endl;
			std::wcout << "ms:" << std::hex << protocols[i].dwMessageSize << std::endl;
			std::wcout << "sf:" << std::hex << protocols[i].dwServiceFlags1 << std::endl;
			std::wcout << "pf:" << std::hex << protocols[i].dwProviderFlags << std::endl;
			for(int k=0; k<protocols[i].ProtocolChain.ChainLen; ++k) {
				std::wcout << "ch:" << protocols[i].ProtocolChain.ChainEntries[k] << std::endl;
			}
			*/
		}

		wchar_t line[256];
		std::wcin >> line;
		if(wcslen(line)>1) break;

	}

	WSACleanup();

}
