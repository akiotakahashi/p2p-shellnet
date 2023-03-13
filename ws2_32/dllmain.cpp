#include "stdafx.h"
#include "ws2_32.h"


extern "C" __declspec(dllexport) BOOL WINAPI DllMain(HINSTANCE hinstDLL, DWORD fdwReason, LPVOID lpvReserved) {
	return FALSE;
	switch(fdwReason) {
	case DLL_PROCESS_ATTACH:
		Console::Error->WriteLine(S"attached from process");
		return TRUE;
	case DLL_THREAD_ATTACH:
		return TRUE;
	case DLL_THREAD_DETACH:
		return TRUE;
	case DLL_PROCESS_DETACH:
		Console::Error->WriteLine(S"detached from process");
		return TRUE;
	default:
		return TRUE;
	}
}
