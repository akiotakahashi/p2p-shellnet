#include "stdafx.h"


static void engage(void* param) {
	SOCKET accepted = reinterpret_cast<SOCKET>(param);
	/* 終わる(Ctrl-Dが押される)まで繰り返す */
	char buf[1024];
	int readlen;
	while((readlen = recv(accepted, buf, sizeof(buf), 0)) > 0){
		buf[readlen] = '\0';
		printf("%s\n",buf);
	}
	if(readlen==SOCKET_ERROR) {
		std::cout << "failed to read: " << WSAGetLastError() << std::endl;
	}
	closesocket(accepted);
	std::cout << "finished reading" << std::endl;
}

extern int main(int argc, char *argv[]) {

	WSADATA wsaData;
	if(0!=WSAStartup(MAKEWORD(2,2),&wsaData)) {
		std::cout << WSAGetLastError() << std::endl;
		WSACleanup();
		return -1;
	}

	SOCKET sd;
	struct sockaddr_in sin;
	struct sockaddr_in client;
	int clientlen;
	SOCKET accepted;

	/* socketの準備 */
	sd = socket(AF_INET, SOCK_STREAM, 0);
	if(sd==INVALID_SOCKET) {
		std::cout << WSAGetLastError() << std::endl;
		WSACleanup();
		return 1;
	}

	WSAPROTOCOL_INFO pi;
	int size;
	getsockopt(sd,SOL_SOCKET,SO_PROTOCOL_INFO,(char*)&pi,&size);
	std::cout << pi.szProtocol << std::endl;

	if(argc==1) {

		/* 自分のアドレスとポートを設定 */
		memset((void *)&sin, 0, sizeof(sin));
		sin.sin_family = AF_INET;
		sin.sin_port = htons(5556);
		sin.sin_addr.S_un.S_addr = htonl(INADDR_ANY);
		bind(sd, (struct sockaddr *)&sin, sizeof(sin));
		/* 準備完了 */
		listen(sd, 5);

		/* 接続されるまで待つ */
		while(1) {
			clientlen = sizeof(client);
			accepted = accept(sd, (struct sockaddr *)&client, &clientlen);
			/* 接続された */
			char host[256];
			getnameinfo((sockaddr*)&client,sizeof(client),host,sizeof(host),NULL,0,NI_NUMERICHOST);
			printf("accepted from %s [%d]\n", host, ntohs(client.sin_port));
			_beginthread(engage,0,reinterpret_cast<void*>(accepted));
		}

	} else {

		SOCKADDR_IN sa;
		::memset(&sa, 0, sizeof(sa));
		sa.sin_family = AF_INET;
		sa.sin_port = htons(5556);
		sa.sin_addr.S_un.S_addr = htonl(INADDR_LOOPBACK);

		connect(sd,(SOCKADDR*)&sa,sizeof(sa));

		char buf[1024];
		while(true) {
			scanf("%[\n\r]",buf);
			scanf("%[^\n\r]",buf);
			send(sd,buf,(int)strlen(buf),0);
		}
	}

	WSACleanup();

}
