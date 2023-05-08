// SPDX-FileCopyrightText: Copyright 2023 Holo Interactive <dev@holoi.com>
// SPDX-FileContributor: Yuchen Zhang <yuchen@holoi.com>
// SPDX-License-Identifier: MIT

#import "MultipeerNearbyServiceBrowser.h"
#import "MultipeerSession.h"

void* HoloInteractiveMC_MultipeerNearbyServiceBrowser_initWithPeer(void *peerIDPtr, void *serviceTypePtr) {
    MCPeerID *peerID = (__bridge MCPeerID *)peerIDPtr;
    NSString *serviceType = (__bridge NSString *)serviceTypePtr;
    
    MultipeerNearbyServiceBrowser *browser = [[MultipeerNearbyServiceBrowser alloc] initWithPeer:peerID serviceType:serviceType];
    return (__bridge_retained void *)browser;
}

void HoloInteractiveMC_MultipeerNearbyServiceBrowser_startBrowsingForPeers(void *self) {
    MultipeerNearbyServiceBrowser *browser = (__bridge MultipeerNearbyServiceBrowser *)self;
    [browser startBrowsingForPeers];
}

void HoloInteractiveMC_MultipeerNearbyServiceBrowser_stopBrowsingForPeers(void *self) {
    MultipeerNearbyServiceBrowser *browser = (__bridge MultipeerNearbyServiceBrowser *)self;
    [browser stopBrowsingForPeers];
}

void HoloInteractiveMC_MultipeerNearbyServiceBrowser_registerCallbacks(void *self,
                                                                       OnFoundPeerCallback onFoundPeerCallback,
                                                                       OnLostPeerCallback onLostPeerCallback,
                                                                       OnDidNotStartBrowsingForPeersCallback onDidNotStartBrowsingForPeersCallback) {
    MultipeerNearbyServiceBrowser *browser = (__bridge MultipeerNearbyServiceBrowser *)self;
    [browser setOnFoundPeerCallback: onFoundPeerCallback];
    [browser setOnLostPeerCallback: onLostPeerCallback];
    [browser setOnDidNotStartBrowsingForPeersCallback: onDidNotStartBrowsingForPeersCallback];
}

void HoloInteractiveMC_MultipeerNearbyServiceBrowser_invitePeer(void *self, void *peerIDPtr, void *sessionPtr, void *contextPtr, double timeout) {
    MultipeerNearbyServiceBrowser *browser = (__bridge MultipeerNearbyServiceBrowser *)self;
    MCPeerID *peerID = (__bridge MCPeerID *)peerIDPtr;
    MultipeerSession *session = (__bridge MultipeerSession *)sessionPtr;
    NSData *context = (__bridge NSData *)contextPtr;

    [browser invitePeer:peerID toSession:session.session withContext:context timeout:timeout];
}
