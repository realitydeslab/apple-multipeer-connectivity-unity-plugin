#import <MultipeerConnectivity/MultipeerConnectivity.h>

typedef void (^InvitationHandler)(BOOL, MCSession * _Nullable);
typedef void (*OnDidReceiveInvitationFromPeerCallback)(void * _Nonnull, void * _Nonnull, void * _Nullable, void * _Nonnull);

@interface MultipeerNearbyServiceAdvertiser: NSObject<MCNearbyServiceAdvertiserDelegate>

- (nullable instancetype)initWithPeer:(MCPeerID * _Nonnull)peerID discoveryInfo:(NSDictionary<NSString *, NSString *> * _Nullable)discoveryInfo serviceType:(NSString * _Nonnull)serviceType;
- (void)startAdvertisingPeer;
- (void)stopAdvertisingPeer;

@property (nonatomic, assign) OnDidReceiveInvitationFromPeerCallback _Nullable onDidReceiveInvitationFromPeerCallback;

@end
