// SPDX-FileCopyrightText: Copyright 2023 Holo Interactive <dev@holoi.com>
// SPDX-FileCopyrightText: Copyright 2020 Unity Technologies ApS
// SPDX-FileContributor: Yuchen Zhang <yuchen@holoi.com>
// SPDX-License-Identifier: MIT AND LicenseRef-Unity-Companion-License

#import <Foundation/Foundation.h>

void HoloInteractiveMC_NativeApi_CFRelease(void* ptr)
{
    if (ptr)
    {
        CFRelease(ptr);
    }
}
