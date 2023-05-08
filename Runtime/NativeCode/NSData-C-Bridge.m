// SPDX-FileCopyrightText: Copyright 2023 Holo Interactive <dev@holoi.com>
// SPDX-FileContributor: Yuchen Zhang <yuchen@holoi.com>
// SPDX-License-Identifier: MIT

#import <Foundation/Foundation.h>

int HoloInteractiveMC_NSData_getLength(void* self)
{
    NSData* data = (__bridge NSData*)self;
    return (int)data.length;
}

void* HoloInteractiveMC_NSData_createWithBytes(void* bytes, int length)
{
    NSData* data = [[NSData alloc] initWithBytes:bytes
                                          length:length];

    return (__bridge_retained void*)data;
}

void* HoloInteracitveMC_NSData_createWithBytesNoCopy(void* bytes, int length, bool freeWhenDone)
{
    NSData* data = [[NSData alloc] initWithBytesNoCopy:bytes
                                                length:length
                                          freeWhenDone:freeWhenDone];

    return (__bridge_retained void*)data;
}

const void* HoloInteractiveMC_NSData_getBytes(void* self)
{
    NSData* data = (__bridge NSData*)self;
    return data.bytes;
}
