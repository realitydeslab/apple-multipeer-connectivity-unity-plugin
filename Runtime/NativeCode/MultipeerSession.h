// SPDX-FileCopyrightText: Copyright 2023 Holo Interactive <dev@holoi.com>
// SPDX-FileContributor: Yuchen Zhang <yuchen@holoi.com>
// SPDX-License-Identifier: MIT

#import <MultipeerConnectivity/MultipeerConnectivity.h>

typedef void (*OnPeerDidChangeStateCallback)(void * _Nonnull, void * _Nonnull, long);
typedef void (*OnDidReceiveDataCallback)(void * _Nonnull, void * _Nonnull, void * _Nonnull);

@interface MultipeerSession : NSObject<MCSessionDelegate>

@property (nonatomic, strong) MCSession * _Nullable session;
@property (nonatomic, assign) OnPeerDidChangeStateCallback _Nullable onPeerDidChangeStateCallback;
@property (nonatomic, assign) OnDidReceiveDataCallback _Nullable onDidReceiveDataCallback;

- (nullable instancetype)initWithPeer:(MCPeerID * _Nonnull)peerID;
- (BOOL)sendData:(nonnull NSData *)data toPeer:(nonnull MCPeerID *)peerID withMode:(MCSessionSendDataMode)mode;
- (BOOL)sendDataToAllPeers:(nonnull NSData *)data withMode:(MCSessionSendDataMode)mode;

@end
