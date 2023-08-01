//
//  OverrideAppDelegate.h
//  Unity-iPhone
//
//  Created by tuandigital on 6/17/20.
//

#import <Foundation/Foundation.h>
#import <UIKit/UIKit.h>

NS_ASSUME_NONNULL_BEGIN

@interface ColoringAppDelegate : NSObject

-(BOOL)application:(UIApplication*) application didFinishLaunchingWithOptions:(NSDictionary*) launchOptions;
-(BOOL)_original_saved_by_Override_application:(UIApplication*) application didFinishLaunchingWithOptions:(NSDictionary*) launchOptions;

@end

NS_ASSUME_NONNULL_END
