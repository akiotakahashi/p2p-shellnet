#include "stdafx.h"
#include "pipestream.h"


PipeStream::PipeStream(HANDLE hPipe) {
	pipe = hPipe;
}

PipeStream& PipeStream::operator <<(System::String& text) {
	int len = text.Length;
	DWORD io;
	WriteFile(pipe,&len,sizeof len,&io,NULL);
	wchar_t buf __gc[] = text.ToCharArray();
	wchar_t __pin* p = &buf[0];
	WriteFile(pipe,p,len*sizeof(wchar_t),&io,NULL);
	return *this;
}

System::String& PipeStream::getString() const {
	int len;
	DWORD io;
	ReadFile(pipe,&len,sizeof len,&io,NULL);
	wchar_t buf __gc[] = __gc new wchar_t __gc[len];
	wchar_t __pin* p = &buf[0];
	ReadFile(pipe,p,len*sizeof(wchar_t),&io,NULL);
	return * __gc new System::String(buf);
}
