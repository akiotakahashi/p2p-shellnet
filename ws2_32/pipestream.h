#pragma once


enum PACKETTYPE {
	HELLO,
	WELCOME,
};

__gc interface PACKET {
	virtual PACKETTYPE getType() = 0;
};

struct PipeStream {
	HANDLE pipe;
	PipeStream(HANDLE hPipe);
	PipeStream& operator <<(System::String& text);
	System::String& getString() const;
};

__gc struct HELLOPACKET : PACKET {
	PACKETTYPE getType() {return HELLO;}
	System::String* client;
};

__gc struct WELCOMEPACKET : PACKET {
	PACKETTYPE getType() {return WELCOME;}
	System::String* server;
	WELCOMEPACKET(PipeStream& pipe) {
		server = &pipe.getString();
	}
};
