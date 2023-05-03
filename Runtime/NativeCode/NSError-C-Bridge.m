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
