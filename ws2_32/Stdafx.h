// stdafx.h : 標準のシステム インクルード ファイルのインクルード ファイル、または
// 参照回数が多く、かつあまり変更されない、プロジェクト専用のインクルード ファイル
// を記述します。

#pragma once

#define WINVER			0x510
#define _WIN32_WINNT	0x510


#define WINSOCK_API_LINKAGE

#include <winsock2.h>
#include <ws2tcpip.h>
#include <windows.h>

#include <_vcclrit.h>
#include <stdio.h>
#include <memory.h>


using namespace System;
