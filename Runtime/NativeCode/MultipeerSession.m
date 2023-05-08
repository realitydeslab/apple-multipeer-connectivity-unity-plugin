// SPDX-FileCopyrightText: Copyright 2023 Holo Interactive <dev@holoi.com>
// SPDX-FileContributor: Yuchen Zhang <yuchen@holoi.com>
// SPDX-License-Identifier: MIT

#import "MultipeerSession.h"

@implementation MultipeerSession

- (instancetype)initWithPeer:(MCPeerID *)peerID {
    if (self = [super init]) {
        _session = [[MCSession alloc] initWithPeer:peerID securityIdentity:nil
                               encryptionPreference:MCEncryptionRequired];
        _session.delegate = self;
    }
    return self;
}

- (BOOL)sendData:(nonnull NSData *)data toPeer:(nonnull MCPeerID *)peerID withMode:(MCSessionSendDataMode)mode {
    NSArray *peerIDs = @[peerID];
    NSError *error = nil;
    return [_session sendData:data toPeers:peerIDs withMode:mode error:&error];
}

- (BOOL)sendDataToAllPeers:(nonnull NSData *)data withMode:(MCSessionSendDataMode)mode {
    NSError *error = nil;
    return [_session sendData:data toPeers:_session.connectedPeers withMode:mode error:&error];
}

#pragma mark - MCSessionDelegate

- (void)session:(nonnull MCSession *)session peer:(nonnull MCPeerID *)peerID didChangeState:(MCSessionState)state {
    if (_onPeerDidChangeStateCallback) {
        dispatch_async(dispatch_get_main_queue(), ^{
            self.onPeerDidChangeStateCallback((__bridge void *)self, (__bridge_retained void *)peerID, state);
        });
    }
}

- (void)session:(nonnull MCSession *)session didReceiveData:(nonnull NSData *)data fromPeer:(nonnull MCPeerID *)peerID {
    if (_onDidReceiveDataCallback) {
        dispatch_async(dispatch_get_main_queue(), ^{
            self.onDidReceiveDataCallback((__bridge void *)self, (__bridge_retained void *)data, (__bridge void *)peerID);
        });
    }
}

- (void)session:(MCSession *)session didReceiveCertificate:(NSArray *)certificate fromPeer:(MCPeerID *)peerID certificateHandler:(void (^)(BOOL))certificateHandler {
    if (certificateHandler != nil) {
        certificateHandler(YES);
    }
}

- (void)session:(nonnull MCSession *)session didReceiveStream:(nonnull NSInputStream *)stream withName:(nonnull NSString *)streamName fromPeer:(nonnull MCPeerID *)peerID {
    // Not used.
}

- (void)session:(nonnull MCSession *)session didStartReceivingResourceWithName:(nonnull NSString *)resourceName fromPeer:(nonnull MCPeerID *)peerID withProgress:(nonnull NSProgress *)progress {
    // Not used.
}

- (void)session:(nonnull MCSession *)session didFinishReceivingResourceWithName:(nonnull NSString *)resourceName fromPeer:(nonnull MCPeerID *)peerID atURL:(nullable NSURL *)localURL withError:(nullable NSError *)error {
    // Not used.
}

@end
