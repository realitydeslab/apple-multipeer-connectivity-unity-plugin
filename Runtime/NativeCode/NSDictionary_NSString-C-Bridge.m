// SPDX-FileCopyrightText: Copyright 2023 Holo Interactive <dev@holoi.com>
// SPDX-FileContributor: Yuchen Zhang <yuchen@holoi.com>
// SPDX-License-Identifier: MIT

#import <Foundation/Foundation.h>

void* HoloInteractiveMC_NSDictionaryNSString_createWithArray(void** arrPtr, int length) {
    NSMutableDictionary<NSString *, NSString *> *dict = [[NSMutableDictionary alloc] init];
    for (int i = 0; i < length; i++) {
        NSString *key = (__bridge NSString*)arrPtr[2 * i];
        NSString *value = (__bridge NSString*)arrPtr[2 * i + 1];
        
        NSString *newKey = [key copy];
        NSString *newValue = [value copy];
        [dict setObject:newValue forKey:newKey];
    }
    return (__bridge_retained void *)dict;
}

int HoloInteractiveMC_NSDictionaryNSString_getDictCount(void* dictPtr) {
    NSDictionary<NSString *, NSString *> *dict = (__bridge NSDictionary<NSString *, NSString *> *)dictPtr;
    return (int)[dict count];
}

void HoloInteractiveMC_NSDictionaryNSString_getDictKeys(void *dictPtr, void **keys) {
    NSDictionary<NSString *, NSString *> *dict = (__bridge NSDictionary<NSString *, NSString *> *)dictPtr;
    NSArray<NSString *> *keyArray = [dict allKeys];
    for (NSUInteger i = 0; i < [keyArray count]; i++) {
        keys[i] = (__bridge_retained void *)keyArray[i];
    }
}

void HoloInteractiveMC_NSDictionaryNSString_getDictValues(void *dictPtr, void **values) {
    NSDictionary<NSString *, NSString *> *dict = (__bridge NSDictionary<NSString *, NSString *> *)dictPtr;
    NSArray<NSString *> *valueArray = [dict allValues];
    for (NSUInteger i = 0; i < [valueArray count]; i++) {
        values[i] = (__bridge_retained void *)valueArray[i];
    }
}
