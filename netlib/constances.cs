using System;

namespace Shellnet {
	public abstract class WSAE {
		public const int WSABASEERR				= 10000;
		public const int INTR					= (WSABASEERR+4);
		public const int BADF					= (WSABASEERR+9);
		public const int ACCES					= (WSABASEERR+13);
		public const int FAULT					= (WSABASEERR+14);
		public const int INVAL					= (WSABASEERR+22);
		public const int MFILE					= (WSABASEERR+24);
		public const int WOULDBLOCK				= (WSABASEERR+35);
		public const int INPROGRESS				= (WSABASEERR+36);
		public const int ALREADY				= (WSABASEERR+37);
		public const int NOTSOCK				= (WSABASEERR+38);
		public const int DESTADDRREQ			= (WSABASEERR+39);
		public const int MSGSIZE				= (WSABASEERR+40);
		public const int PROTOTYPE				= (WSABASEERR+41);
		public const int NOPROTOOPT				= (WSABASEERR+42);
		public const int PROTONOSUPPORT			= (WSABASEERR+43);
		public const int SOCKTNOSUPPORT			= (WSABASEERR+44);
		public const int OPNOTSUPP				= (WSABASEERR+45);
		public const int PFNOSUPPORT			= (WSABASEERR+46);
		public const int AFNOSUPPORT			= (WSABASEERR+47);
		public const int ADDRINUSE				= (WSABASEERR+48);
		public const int ADDRNOTAVAIL			= (WSABASEERR+49);
		public const int NETDOWN				= (WSABASEERR+50);
		public const int NETUNREACH				= (WSABASEERR+51);
		public const int NETRESET				= (WSABASEERR+52);
		public const int CONNABORTED			= (WSABASEERR+53);
		public const int CONNRESET				= (WSABASEERR+54);
		public const int NOBUFS					= (WSABASEERR+55);
		public const int ISCONN					= (WSABASEERR+56);
		public const int NOTCONN				= (WSABASEERR+57);
		public const int SHUTDOWN				= (WSABASEERR+58);
		public const int TOOMANYREFS			= (WSABASEERR+59);
		public const int TIMEDOUT				= (WSABASEERR+60);
		public const int CONNREFUSED			= (WSABASEERR+61);
		public const int LOOP					= (WSABASEERR+62);
		public const int NAMETOOLONG			= (WSABASEERR+63);
		public const int HOSTDOWN				= (WSABASEERR+64);
		public const int HOSTUNREACH			= (WSABASEERR+65);
		public const int NOTEMPTY				= (WSABASEERR+66);
		public const int PROCLIM				= (WSABASEERR+67);
		public const int USERS					= (WSABASEERR+68);
		public const int DQUOT					= (WSABASEERR+69);
		public const int STALE					= (WSABASEERR+70);
		public const int REMOTE					= (WSABASEERR+71);
		public const int WSASYSNOTREADY			= (WSABASEERR+91);
		public const int WSAVERNOTSUPPORTED		= (WSABASEERR+92);
		public const int WSANOTINITIALISED		= (WSABASEERR+93);
		public const int DISCON					= (WSABASEERR+101);
		public const int NOMORE					= (WSABASEERR+102);
		public const int CANCELLED				= (WSABASEERR+103);
		public const int INVALIDPROCTABLE		= (WSABASEERR+104);
		public const int INVALIDPROVIDER		= (WSABASEERR+105);
		public const int PROVIDERFAILEDINIT		= (WSABASEERR+106);
		public const int WSASYSCALLFAILURE		= (WSABASEERR+107);
		public const int WSASERVICE_NOT_FOUND	= (WSABASEERR+108);
		public const int WSATYPE_NOT_FOUND		= (WSABASEERR+109);
		public const int WSA_E_NO_MORE			= (WSABASEERR+110);
		public const int WSA_E_CANCELLED		= (WSABASEERR+111);
		public const int REFUSED				= (WSABASEERR+112);
	}
	public abstract class FD {
		public const int READ						= 1;
		public const int WRITE						= 2;
		public const int OOB						= 4;
		public const int ACCEPT						= 8;
		public const int CONNECT					= 16;
		public const int CLOSE						= 32;
		public const int QOS						= 64;
		public const int GROUP_QOS					= 128;
		public const int ROUTING_INTERFACE_CHANGE	= 256;
		public const int ADDRESS_LIST_CHANGE		= 512;
		public const int ALL_EVENTS					= 0xFFFF;
	}
}
