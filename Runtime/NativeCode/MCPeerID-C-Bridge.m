// SPDX-FileCopyrightText: Copyright 2023 Holo Interactive <dev@holoi.com>
// SPDX-FileContributor: Yuchen Zhang <yuchen@holoi.com>
// SPDX-License-Identifier: MIT

#import <MultipeerConnectivity/MultipeerConnectivity.h>

void* HoloInteractiveMC_MCPeerID_initWithDisplayName(void *displayNamePtr) {
    NSString *displayName = (__bridge NSString *)displayNamePtr;
    MCPeerID *peerID = [[MCPeerID alloc] initWithDisplayName: displayName];
    return (__bridge_retained void *)peerID;
}

void* HoloInteractiveMC_MCPeerID_getDisplayName(void *self) {
    MCPeerID *peerID = (__bridge MCPeerID*)self;
    return (__bridge_retained void*)peerID.displayName;
}
