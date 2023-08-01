//
//  MediaMuxer.h
//  NumberColoringObjCPlugin
//
//  Created by Tuan Nguyen on 5/28/20.
//  Copyright © 2020 Tuan Nguyen. All rights reserved.
//

#ifndef MediaMuxer_h
#define MediaMuxer_h

@interface MediaMuxer : NSObject<MFMessageComposeViewControllerDelegate>

+(MediaMuxer *) instance;
-(void) initialize:(int) pictureId width:(int) width height:(int) height with:(int) shareType;
-(void) addFrame:(NSData *) data with:(int) count last:(bool) isLast;
-(void) produceVideo: (NSData *) originalPic withGray: (NSData *) grayPic withSteps: (NSArray *) steps withLogo: (NSData *) logo andStepsByFrame: (int) stepsByFrame withFPS: (int) fps withPadding:(int)padding;
-(void) savePhoto: (NSData *) imageData withLogo:(NSData *)logo withPadding:(int)padding;
-(void) showRequestPermissionPopup;

@end


#endif /* MediaMuxer_h */
