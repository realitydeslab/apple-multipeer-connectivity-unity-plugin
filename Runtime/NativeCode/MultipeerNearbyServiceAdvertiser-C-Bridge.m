#import "MultipeerNearbyServiceAdvertiser.h"
#import "MultipeerSession.h"

void* HoloInteractiveMC_MultipeerNearbyServiceAdvertiser_initWithPeer(void *peerIDPtr, void *serviceTypePtr) {
    MCPeerID *peerID = (__bridge MCPeerID *)peerIDPtr;
    NSString *serviceType = (__bridge NSString *)serviceTypePtr;
    
    MultipeerNearbyServiceAdvertiser *advertiser = [[MultipeerNearbyServiceAdvertiser alloc] initWithPeer:peerID discoveryInfo:nil serviceType:serviceType];
    return (__bridge_retained void *)advertiser;
}

void* HoloInteractiveMC_MultipeerNearbyServiceAdvertiser_initWithPeerAndDiscoveryInfo(void *peerIDPtr, void *discoveryInfoPtr, void *serviceTypePtr) {
    MCPeerID *peerID = (__bridge MCPeerID *)peerIDPtr;
    NSDictionary<NSString *, NSString *> *discoveryInfo = (__bridge NSDictionary<NSString *, NSString *> *)discoveryInfoPtr;
    NSString *serviceType = (__bridge NSString *)serviceTypePtr;
    
    MultipeerNearbyServiceAdvertiser *advertiser = [[MultipeerNearbyServiceAdvertiser alloc] initWithPeer:peerID discoveryInfo:discoveryInfo serviceType:serviceType];
    return (__bridge_retained void *)advertiser;
}

void HoloInteractiveMC_MultipeerNearbyServiceAdvertiser_startAdvertisingPeer(void *self) {
    MultipeerNearbyServiceAdvertiser *advertiser = (__bridge MultipeerNearbyServiceAdvertiser *)self;
    [advertiser startAdvertisingPeer];
}

void HoloInteractiveMC_MultipeerNearbyServiceAdvertiser_stopAdvertisingPeer(void *self) {
    MultipeerNearbyServiceAdvertiser *advertiser = (__bridge MultipeerNearbyServiceAdvertiser *)self;
    [advertiser stopAdvertisingPeer];
}

void HoloInteractiveMC_MultipeerNearbyServiceAdvertiser_registerCallbacks(void *self,
                                                                          OnDidReceiveInvitationFromPeerCallback onDidReceiveInvitationFromPeerCallback) {
    MultipeerNearbyServiceAdvertiser *advertiser = (__bridge MultipeerNearbyServiceAdvertiser *)self;
    [advertiser setOnDidReceiveInvitationFromPeerCallback:onDidReceiveInvitationFromPeerCallback];
}

void HoloInteractiveMC_MultipeerNearbyServiceAdvertiser_handleInvitation(void *invitationHandlerPtr, bool accept, void *sessionPtr) {
    InvitationHandler invitationHandler = (__bridge InvitationHandler)invitationHandlerPtr;
    MultipeerSession *session = (__bridge MultipeerSession *)sessionPtr;
    invitationHandler(accept, session.session);
}
