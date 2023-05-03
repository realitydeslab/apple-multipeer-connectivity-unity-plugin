#import <MultipeerConnectivity/MultipeerConnectivity.h>

typedef void (*OnFoundPeerCallback)(void * _Nonnull, void * _Nonnull, void * _Nullable);
typedef void (*OnLostPeerCallback)(void * _Nonnull, void * _Nonnull);

@interface MultipeerNearbyServiceBrowser: NSObject<MCNearbyServiceBrowserDelegate>

- (nullable instancetype)initWithPeer:(MCPeerID * _Nonnull)peerID serviceType:(NSString * _Nonnull)serviceType;
- (void)startBrowsingForPeers;
- (void)stopBrowsingForPeers;
- (void)invitePeer:(MCPeerID * _Nonnull)peerID toSession:(MCSession * _Nonnull)session withContext:(NSData * _Nullable)context timeout:(NSTimeInterval)timeout;

@property (nonatomic, assign) OnFoundPeerCallback _Nullable onFoundPeerCallback;
@property (nonatomic, assign) OnLostPeerCallback _Nullable onLostPeerCallback;

@end
