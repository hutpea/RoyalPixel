//
//  OverrideAppDelegate.m
//  Unity-iPhone
//
//  Created by tuandigital on 6/17/20.
//

#import "ColoringAppDelegate.h"
#import <UIKit/UIKit.h>
#import <NotificationCenter/NotificationCenter.h>
#import "IOSHelper.h"

@implementation ColoringAppDelegate
-(BOOL)application:(UIApplication*) application didFinishLaunchingWithOptions:(NSDictionary*) launchOptions
{
    UILocalNotification *notification = [launchOptions objectForKey: UIApplicationLaunchOptionsLocalNotificationKey];
    if(notification != nil && notification.userInfo != nil){
        [IOSHelper setNotificationData:[notification.userInfo objectForKey:@"data"]];
    }
    return [self _original_saved_by_Override_application:application didFinishLaunchingWithOptions:launchOptions];
}
-(BOOL)_original_saved_by_Override_application:(UIApplication*) application didFinishLaunchingWithOptions:(NSDictionary*) launchOptions
{
    // Yet empty (original Unity implementation will be copied here).
    return YES;
}
@end
