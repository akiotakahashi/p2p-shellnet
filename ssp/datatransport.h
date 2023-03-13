#pragma once


#define _PREPARE_SOCKET(s,sd,socket,rv,errno) \
	const SOCKETDESC* sd = GetSocketDesc(UpcallTable,s); \
	if((sd)==NULL) return seterr(rv,WSAENOTSOCK,errno); \
	ISocket* socket = GetSocket(sd->peerId,sd->socketId);

#define PREPARE_SOCKET(s,sd,socket,rv,errno) \
	std::cerr << __FUNCTION__ << std::endl; \
	_PREPARE_SOCKET(s,sd,socket,rv,errno);


#define WPUCreateSocketHandle			UpcallTable.lpWPUCreateSocketHandle
#define WPUQuerySocketHandleContext		UpcallTable.lpWPUQuerySocketHandleContext
