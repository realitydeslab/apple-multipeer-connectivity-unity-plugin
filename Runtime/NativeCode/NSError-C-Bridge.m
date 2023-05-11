// SPDX-FileCopyrightText: Copyright 2023 Holo Interactive <dev@holoi.com>
// SPDX-FileCopyrightText: Copyright 2020 Unity Technologies ApS
// SPDX-FileContributor: Yuchen Zhang <yuchen@holoi.com>
// SPDX-License-Identifier: MIT AND LicenseRef-Unity-Companion-License

#import <Foundation/Foundation.h>

long HoloInteractiveMC_NSError_code(void* ptr)
{
    return ((__bridge NSError*)ptr).code;
}

void* HoloInteractiveMC_NSError_localizedDescription(void* ptr)
{
    NSString* desc = ((__bridge NSError*)ptr).localizedDescription;
    return (__bridge_retained void*)desc;
}
