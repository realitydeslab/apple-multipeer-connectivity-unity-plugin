#import <Foundation/Foundation.h>

void HoloInteractiveMC_NativeApi_CFRelease(void* ptr)
{
    if (ptr)
    {
        CFRelease(ptr);
    }
}
