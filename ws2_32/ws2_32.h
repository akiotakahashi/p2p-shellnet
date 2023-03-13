#pragma once


using System::Console;

typedef unsigned char byte;
typedef unsigned short ushort;
typedef unsigned long ulong;
typedef unsigned int uint;


/*
    ordinal hint RVA      name

         25    0 00003470 FreeAddrInfoW
         26    1 000036A0 GetAddrInfoW
         27    2 0000C1B0 GetNameInfoW
        500    3 00013880 WEP
         28    4 00013250 WPUCompleteOverlappedRequest
         29    5 00012110 WSAAccept
         30    6 00010C50 WSAAddressToStringA
         31    7 00007B20 WSAAddressToStringW
        102    8 0000EE90 WSAAsyncGetHostByAddr
        103    9 0000EDE0 WSAAsyncGetHostByName
        105    A 0000EF50 WSAAsyncGetProtoByName
        104    B 0000F000 WSAAsyncGetProtoByNumber
        107    C 0000EC80 WSAAsyncGetServByName
        106    D 0000ED40 WSAAsyncGetServByPort
        101    E 000118D0 WSAAsyncSelect
        108    F 0000F090 WSACancelAsyncRequest
        113   10 0000D170 WSACancelBlockingCall
        116   11 00008170 WSACleanup
         32   12 00006C00 WSACloseEvent
         33   13 00011FC0 WSAConnect
         34   14 00006C40 WSACreateEvent
         35   15 0000DBE0 WSADuplicateSocketA
         36   16 0000DB40 WSADuplicateSocketW
         37   17 000102A0 WSAEnumNameSpaceProvidersA
         38   18 00010310 WSAEnumNameSpaceProvidersW
         39   19 00007E20 WSAEnumNetworkEvents
         40   1A 0000DF90 WSAEnumProtocolsA
         41   1B 000069D0 WSAEnumProtocolsW
         42   1C 00007BF0 WSAEventSelect
        111   1D 00007A00 WSAGetLastError
         43   1E 00012070 WSAGetOverlappedResult
         44   1F 0000FBC0 WSAGetQOSByName
         45   20 00011280 WSAGetServiceClassInfoA
         46   21 00010AF0 WSAGetServiceClassInfoW
         47   22 000106C0 WSAGetServiceClassNameByClassIdA
         48   23 000108E0 WSAGetServiceClassNameByClassIdW
         49   24 0000B8B0 WSAHtonl
         50   25 0000B9D0 WSAHtons
         58   26 000111F0 WSAInstallServiceClassA
         59   27 000104C0 WSAInstallServiceClassW
         60   28 00006E00 WSAIoctl
        114   29 0000D1D0 WSAIsBlocking
         61   2A 00012300 WSAJoinLeaf
         62   2B 00003230 WSALookupServiceBeginA
         63   2C 00003300 WSALookupServiceBeginW
         64   2D 000023C0 WSALookupServiceEnd
         65   2E 00001EA0 WSALookupServiceNextA
         66   2F 00001E00 WSALookupServiceNextW
         67   30 00007A50 WSANSPIoctl
         68   31 0000B8B0 WSANtohl
         69   32 0000B9D0 WSANtohs
         70   33 00006B60 WSAProviderConfigChange
         71   34 000012F0 WSARecv
         72   35 0000FE50 WSARecvDisconnect
         73   36 0000FEF0 WSARecvFrom
         74   37 000105C0 WSARemoveServiceClass
         75   38 0000E140 WSAResetEvent
         76   39 00011970 WSASend
         77   3A 00011A30 WSASendDisconnect
         78   3B 00011AD0 WSASendTo
        109   3C 0000D230 WSASetBlockingHook
         79   3D 00006C60 WSASetEvent
        112   3E 00003620 WSASetLastError
         80   3F 00011320 WSASetServiceA
         81   40 00011080 WSASetServiceW
         82   41 000123F0 WSASocketA
         83   42 00002DC0 WSASocketW
        115   43 00007F90 WSAStartup
         84   44 00010EC0 WSAStringToAddressA
         85   45 00003940 WSAStringToAddressW
        110   46 0000D2A0 WSAUnhookBlockingHook
         86   47 00006C80 WSAWaitForMultipleEvents
         24   48 00013520 WSApSetPostRoutine
         87   49 00012EB0 WSCDeinstallProvider
         88   4A 0000F4D0 WSCEnableNSProvider
         89   4B 00006A80 WSCEnumProtocols
         90   4C 0000DF70 WSCGetProviderPath
         91   4D 0000F840 WSCInstallNameSpace
         92   4E 00012B20 WSCInstallProvider
         93   4F 0000FA20 WSCUnInstallNameSpace
         94   50 00012720 WSCUpdateProvider
         95   51 0000F6F0 WSCWriteNameSpaceOrder
         96   52 000129D0 WSCWriteProviderOrder
        151   53 00011870 __WSAFDIsSet
          1   54 00012470 accept
          2   55 00007C70 bind
          3   56 00003B70 closesocket
          4   57 00002150 connect
         97   58 00003470 freeaddrinfo
         98   59 00003570 getaddrinfo
         51   5A 0000E880 gethostbyaddr
         52   5B 00002010 gethostbyname
         57   5C 00006CB0 gethostname
         99   5D 0000C300 getnameinfo
          5   5E 00011BA0 getpeername
         53   5F 0000E5F0 getprotobyname
         54   60 0000E520 getprotobynumber
         55   61 0000EB10 getservbyname
         56   62 0000E9C0 getservbyport
          6   63 00007CF0 getsockname
          7   64 00011C40 getsockopt
          8   65 00001200 htonl
          9   66 000011D0 htons
         11   67 000017B0 inet_addr
         12   68 00003DE0 inet_ntoa
         10   69 0000F0E0 ioctlsocket
         13   6A 00007D70 listen
         14   6B 00001200 ntohl
         15   6C 000011D0 ntohs
         16   6D 0000FD70 recv
         17   6E 00003A60 recvfrom
         18   6F 00001630 select
         19   70 000013B0 send
         20   71 000039C0 sendto
         21   72 00001450 setsockopt
         22   73 00011F20 shutdown
         23   74 00002EA0 socket
*/


typedef int socklen_t;

#undef MessageBox
#undef GetObject


using namespace System::Net;
using namespace System::Net::Sockets;

using namespace System::Runtime;
using namespace System::Runtime::Remoting;
using namespace System::Runtime::Remoting::Channels;
using namespace System::Runtime::Remoting::Channels::Tcp;
using namespace System::Runtime::Remoting::Activation;

using namespace System::Threading;
using namespace System::Collections;

using namespace Shellnet;
