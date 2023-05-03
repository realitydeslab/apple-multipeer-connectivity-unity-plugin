#import "MultipeerSession.h"

void* HoloInteractiveMC_MultipeerSession_initWithPeer(void *peerIDPtr) {
    MCPeerID *peerID = (__bridge MCPeerID *)peerIDPtr;
    MultipeerSession *session = [[MultipeerSession alloc] initWithPeer:peerID];
    return (__bridge_retained void *)session;
}

void HoloInteractiveMC_MultipeerSession_registerCallbacks(void *self,
                                                          OnPeerDidChangeStateCallback onPeerDidChangeStateCallback,
                                                          OnDidReceiveDataCallback onDidReceiveDataCallback) {
    MultipeerSession *session = (__bridge MultipeerSession *)self;
    [session setOnPeerDidChangeStateCallback: onPeerDidChangeStateCallback];
    [session setOnDidReceiveDataCallback: onDidReceiveDataCallback];
}

bool HoloInteractiveMC_MultipeerSession_sendData(void *self, void *dataPtr, void *peerIDPtr, long mode) {
    MultipeerSession *session = (__bridge MultipeerSession *)self;
    NSData *data = (__bridge NSData *)dataPtr;
    MCPeerID *peerID = (__bridge MCPeerID *)peerIDPtr;
    return [session sendData:data toPeer:peerID withMode:mode];
}

bool HoloInteractiveMC_MultipeerSession_sendDataToAllPeers(void *self, void *dataPtr, long mode) {
    MultipeerSession *session = (__bridge MultipeerSession *)self;
    NSData *data = (__bridge NSData *)dataPtr;
    return [session sendDataToAllPeers:data withMode:mode];
}

void HoloInteractiveMC_MultipeerSession_disconnect(void *self) {
    MultipeerSession *session = (__bridge MultipeerSession *)self;
    [session.session disconnect];
}
