#import "MultipeerNearbyServiceBrowser.h"

@implementation MultipeerNearbyServiceBrowser

MCNearbyServiceBrowser *m_NearbyServiceBrowser;

- (instancetype)initWithPeer:(MCPeerID *)peerID serviceType:(NSString *)serviceType {
    if (self = [super init]) {
        m_NearbyServiceBrowser = [[MCNearbyServiceBrowser alloc] initWithPeer:peerID serviceType:serviceType];
        m_NearbyServiceBrowser.delegate = self;
    }
    return self;
}

- (void)startBrowsingForPeers {
    [m_NearbyServiceBrowser startBrowsingForPeers];
}

- (void)stopBrowsingForPeers {
    [m_NearbyServiceBrowser stopBrowsingForPeers];
}

- (void)invitePeer:(MCPeerID *)peerID toSession:(MCSession *)session withContext:(NSData *)context timeout:(NSTimeInterval)timeout {
    //[m_NearbyServiceBrowser invitePeer:peerID toSession:session withContext:context timeout:timeout];
    [m_NearbyServiceBrowser invitePeer:peerID toSession:session withContext:nil timeout:7];
}

#pragma mark - MCNearbyServiceBrowser

- (void)browser:(nonnull MCNearbyServiceBrowser *)browser foundPeer:(nonnull MCPeerID *)peerID withDiscoveryInfo:(nullable NSDictionary<NSString *,NSString *> *)info
{
    if (_onFoundPeerCallback) {
        _onFoundPeerCallback((__bridge void *)self, (__bridge_retained void *)peerID, (__bridge_retained void *)info);
    }
}

- (void)browser:(nonnull MCNearbyServiceBrowser *)browser lostPeer:(nonnull MCPeerID *)peerID
{
    if (_onLostPeerCallback) {
        _onLostPeerCallback((__bridge void *)browser, (__bridge void *)peerID);
    }
}

@end
