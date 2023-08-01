//
//  OverrideiOS.m
//  Unity-iPhone
//
//  Created by tuandigital on 6/17/20.
//

#import "OverrideiOS.h"
#import "ColoringAppDelegate.h"
#import "EPPZSwizzler.h"
#import "UnityAppController.h"

__strong OverrideiOS *_instance;

@implementation OverrideiOS

+(void)load
{
//    NSLog(@"[Override_iOS load]");
    [self swizzle];
    
    _instance = [OverrideiOS new];
}

+(OverrideiOS*)instance
{ return _instance; }
+(void)swizzle
{
//    NSLog(@"[Override_iOS swizzle]");
    
    // The Unity base app controller class (the class name stored in <span class="eppz inlineCode">AppControllerClassName`).
    Class unityAppDelegate = UnityAppController.class;
    Class overrideAppDelegate = ColoringAppDelegate.class;
    
    // See log messages for the sake of this tutorial.
    [EPPZSwizzler setLogging:NO];
    // Add empty placholder to Unity app delegate.
    [EPPZSwizzler addInstanceMethod:@selector(_original_saved_by_Override_application:didFinishLaunchingWithOptions:)
                            toClass:unityAppDelegate
                          fromClass:overrideAppDelegate];
    
    // Save the original Unity app delegate implementation into.
    [EPPZSwizzler swapInstanceMethod:@selector(_original_saved_by_Override_application:didFinishLaunchingWithOptions:)
                  withInstanceMethod:@selector(application:didFinishLaunchingWithOptions:)
                             ofClass:unityAppDelegate];
    
    // Replace Unity app delegate with ours.
    [EPPZSwizzler replaceInstanceMethod:@selector(application:didFinishLaunchingWithOptions:)
                                ofClass:unityAppDelegate
                              fromClass:overrideAppDelegate];
}
@end
