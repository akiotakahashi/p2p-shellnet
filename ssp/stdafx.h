// stdafx.h : 標準のシステム インクルード ファイルのインクルード ファイル、または
// 参照回数が多く、かつあまり変更されない、プロジェクト専用のインクルード ファイル
// を記述します。

#pragma once

#define WINVER			0x510
#define _WIN32_WINNT	0x510


#include <winsock2.h>
#include <ws2tcpip.h>
#include <ws2spi.h>
#include <windows.h>

#include <_vcclrit.h>
#include <iostream>
#include <string>
#include <stdio.h>
#include <memory.h>


using namespace System;
using namespace System::IO;
using namespace System::Net;
using namespace System::Net::Sockets;

using namespace System::Threading;
using namespace System::Collections;

using namespace System::Runtime;
using namespace System::Runtime::Remoting;
using namespace System::Runtime::Remoting::Channels;
using namespace System::Runtime::Remoting::Channels::Tcp;
using namespace System::Runtime::Remoting::Activation;

using namespace Shellnet;
