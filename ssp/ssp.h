#pragma once


// �ȉ��� ifdef �u���b�N�� DLL ����̃G�N�X�|�[�g��e�Ղɂ���}�N�����쐬���邽�߂�
// ��ʓI�ȕ��@�ł��B���� DLL ���̂��ׂẴt�@�C���́A�R�}���h ���C���Œ�`���ꂽ SSP_EXPORTS
// �V���{���ŃR���p�C������܂��B���̃V���{���́A���� DLL ���g���v���W�F�N�g�Œ�`���邱�Ƃ͂ł��܂���B
// �\�[�X�t�@�C�������̃t�@�C�����܂�ł���ق��̃v���W�F�N�g�́A
// SSP_API �֐��� DLL ����C���|�[�g���ꂽ�ƌ��Ȃ��̂ɑ΂��A���� DLL �́A���̃}�N���Œ�`���ꂽ
// �V���{�����G�N�X�|�[�g���ꂽ�ƌ��Ȃ��܂��B

#ifdef SSP_EXPORTS
#define SSP_API __declspec(dllexport)
#else
#define SSP_API __declspec(dllimport)
#endif


#define WIDEN2(x) L ## x
#define WIDEN(x) WIDEN2(x)
#define seterr(rv,ec,out) _seterr(rv,ec,out, WIDEN(__FILE__), __LINE__, WIDEN(__FUNCTION__));
#define seterrif(cond,rv,ec,out) _seterrif(cond,rv,ec,out, WIDEN(__FILE__), __LINE__, WIDEN(__FUNCTION__));

inline static int _seterr(int retval, int errcode, int* out, const wchar_t* file, int line, const wchar_t* function) {
	if(out==NULL) {
		WSASetLastError(errcode);
	} else {
		*out = errcode;
	}
	return retval;
}

inline static int _seterrif(int cond, int retval, int errcode, int* out, const wchar_t* file, int line, const wchar_t* function) {
	if(cond) {
		if(out==NULL) {
			WSASetLastError(errcode);
		} else {
			*out = errcode;
		}
	}
	return cond;
}


inline ProtocolFamily pf2(int pf) {
	switch(pf) {
	case PF_UNSPEC:
		return ProtocolFamily::Unspecified;
	case PF_INET:
		return ProtocolFamily::InterNetwork;
	case PF_INET6:
		return ProtocolFamily::InterNetworkV6;
	default:
		return ProtocolFamily::Unknown;
	}
}

inline AddressFamily af2(int af) {
	switch(af) {
	case AF_UNSPEC:
		return AddressFamily::Unspecified;
	case AF_INET:
		return AddressFamily::InterNetwork;
	case AF_INET6:
		return AddressFamily::InterNetworkV6;
	default:
		return AddressFamily::Unknown;
	}
}

inline SocketType st2(int st) {
	switch(st) {
	case SOCK_STREAM:
		return SocketType::Stream;
	case SOCK_DGRAM:
		return SocketType::Dgram;
	default:
		return SocketType::Unknown;
	}
}

inline ProtocolType pt2(int pt) {
	switch(pt) {
	case 0:
		return ProtocolType::Unspecified;
	case IPPROTO_TCP:
		return ProtocolType::Tcp;
	case IPPROTO_UDP:
		return ProtocolType::Udp;
	default:
		return ProtocolType::Unknown;
	}
}


inline Array* array(Object* arr __gc[]) {
	Array* wrap = arr;
	return wrap;
}

extern char* s2a(String* s);
extern wchar_t* s2w(String* s);
extern void s2a(String* s, char* buf, int size);
extern void s2w(String* s, wchar_t* buf, int size);


extern int Initialize();
extern IPEndPoint* getEndPoint(const sockaddr* addr, int addrlen=0);
extern int getSockAddr(IPEndPoint* ep, sockaddr* sa, int* salen);
