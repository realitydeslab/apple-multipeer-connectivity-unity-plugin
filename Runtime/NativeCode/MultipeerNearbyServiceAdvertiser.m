// SPDX-FileCopyrightText: Copyright 2023 Holo Interactive <dev@holoi.com>
// SPDX-FileContributor: Yuchen Zhang <yuchen@holoi.com>
// SPDX-License-Identifier: MIT

#import "MultipeerNearbyServiceAdvertiser.h"

@implementation MultipeerNearbyServiceAdvertiser

MCNearbyServiceAdvertiser *m_NearbyServiceAdvertiser;

- (instancetype)initWithPeer:(MCPeerID *)peerID discoveryInfo:(NSDictionary<NSString *, NSString *> *)discoveryInfo serviceType:(NSString *)serviceType {
    if (self = [super init]) {
        m_NearbyServiceAdvertiser = [[MCNearbyServiceAdvertiser alloc] initWithPeer:peerID discoveryInfo:discoveryInfo serviceType:serviceType];
        m_NearbyServiceAdvertiser.delegate = self;
    }
    return self;
}

- (void)startAdvertisingPeer {
    [m_NearbyServiceAdvertiser startAdvertisingPeer];
}

- (void)stopAdvertisingPeer {
    [m_NearbyServiceAdvertiser stopAdvertisingPeer];
}

#pragma mark - MCNearbyServiceAdvertiserDelegate

- (void)advertiser:(nonnull MCNearbyServiceAdvertiser *)advertiser didReceiveInvitationFromPeer:(nonnull MCPeerID *)peerID withContext:(nullable NSData *)context invitationHandler:(nonnull void (^)(BOOL, MCSession * _Nullable))invitationHandler
{
    if (_onDidReceiveInvitationFromPeerCallback) {
        dispatch_async(dispatch_get_main_queue(), ^{
            self.onDidReceiveInvitationFromPeerCallback((__bridge void *)self, (__bridge_retained void *)peerID, (__bridge_retained void *)context, (__bridge_retained void *)invitationHandler);
        });
    }
}

@end
