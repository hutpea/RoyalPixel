//
//  MediaMuxer.m
//  NumberColoringObjCPlugin
//
//  Created by Tuan Nguyen on 5/20/20.
//  Copyright © 2020 Tuan Nguyen. All rights reserved.
//

#import <Foundation/Foundation.h>
#import <UIKit/UIKit.h>
#import <MessageUI/MessageUI.h>
#import <PhotosUI/PhotosUI.h>
#import "HJImagesToVideo.h"
#import "MediaMuxer.h"

@implementation MediaMuxer {
    int pictureId;
    NSMutableArray *frames;
    CGSize size;
    CGRect fullRect;
    int shareType;
    NSMutableDictionary *videoPaths;
    NSMutableDictionary *videoGalleryIdentifiers;
    NSMutableDictionary *pictureGalleryIdentifiers;
}

int const SHARE_SAVE_GALLERY = 1;
int const SHARE_SAVE_IMESSAGE = 2;
int const SHARE_SAVE_INSTAGRAM = 3;
int const SHARE_SAVE_OTHERS = 4;
NSString *const HASH_TAGS = @"#pixelcolor #paintbynumber";

static MediaMuxer *gInstance;

+(MediaMuxer *) instance{
    static dispatch_once_t onceToken;
    dispatch_once(&onceToken, ^{
        gInstance = [[MediaMuxer alloc] init];
    });
    return gInstance;
}

-(id) init{
    self = [super init];
    videoPaths = [[NSMutableDictionary alloc] init];
    pictureGalleryIdentifiers = [[NSMutableDictionary alloc] init];
    videoGalleryIdentifiers = [[NSMutableDictionary alloc] init];
    return self;
}

-(void) initialize:(int) pictureId width:(int) width height:(int) height with:(int) shareType{
    [self clearTask];
    NSLog(@"initialize");
    self->pictureId = pictureId;
    self->frames = [[NSMutableArray alloc] init];
    self->size = CGSizeMake(width, height);
    self->fullRect = CGRectMake(0, 0, width, height);
    self->shareType = shareType;
}

-(void) clearTask{
    if(frames){
        [frames removeAllObjects];
        frames = nil;
    }
}

-(void) addFrame:(NSData *) data with:(int) count last:(bool) isLast{
    UIImage *image = [UIImage imageWithData:data];
    for (int i = 0; i < count; i++) {
        [frames addObject:image];
    }
}

-(void) saveVideo: (NSString *) videoPath {
    NSString *localIdentifier = [self->videoGalleryIdentifiers objectForKey:[NSNumber numberWithInt:self->pictureId]];
    if(localIdentifier == nil || ![self isIdentifierExistInGallery:localIdentifier]){
        if([PHPhotoLibrary authorizationStatus] != PHAuthorizationStatusAuthorized){
            [PHPhotoLibrary requestAuthorization:^(PHAuthorizationStatus status) {
                if(status == PHAuthorizationStatusAuthorized){
                    [self saveVideoToGallery:videoPath];
                } else {
                    [self showRequestPermissionPopup];
                }
            }];
        } else{
            [self saveVideoToGallery:videoPath];
        }
    }
}

-(void) savePhoto: (NSData *) imageData withLogo:(NSData *)logo withPadding:(int)padding{
    dispatch_async(dispatch_get_main_queue(), ^{
        NSString *localIdentifier = [pictureGalleryIdentifiers objectForKey:[NSNumber numberWithInt:self->pictureId]];
        if(localIdentifier == nil || ![self isIdentifierExistInGallery:localIdentifier]){
            UIImage *originalImage = [UIImage imageWithData:imageData];
            CGImageRef cgImageLogo = nil;
            if(logo != nil){
                cgImageLogo = [[UIImage imageWithData:logo] CGImage];
            }
            CGFloat width = originalImage.size.width;
            CGFloat height = originalImage.size.height;
            CGRect targetRect;
            if(width > height){
                targetRect.size.width = self->size.width - padding * 2;
                targetRect.size.height = targetRect.size.width * height / width;
                targetRect.origin.x = padding;
                targetRect.origin.y = (self->size.height - targetRect.size.height) / 2;
            } else {
                targetRect.size.height = self->size.height - padding * 2;
                targetRect.size.width = targetRect.size.height * width / height;
                targetRect.origin.x = (self->size.width - targetRect.size.width) / 2;
                targetRect.origin.y = padding;
            }

            CGColorSpaceRef colorSpaceRef = CGColorSpaceCreateDeviceRGB();
            UIImage *scaledImage =[self resizeImageUsingCGContext:[originalImage CGImage] withLogo:cgImageLogo withTargetRect:targetRect];
            if([PHPhotoLibrary authorizationStatus] != PHAuthorizationStatusAuthorized){
                [PHPhotoLibrary requestAuthorization:^(PHAuthorizationStatus status) {
                    if(status == PHAuthorizationStatusAuthorized){
                        [self savePhotoToGallery:scaledImage];
                    } else {
                        [self showRequestPermissionPopup];
                    }
                }];
            } else{
                [self savePhotoToGallery:scaledImage];
            }
        }
    });
}

-(void) sendiMessage:(NSString *) videoPath{
    dispatch_async(dispatch_get_main_queue(), ^{
        MFMessageComposeViewController *messageComposer = [[MFMessageComposeViewController alloc] init];
        [messageComposer addAttachmentURL:[NSURL fileURLWithPath:videoPath] withAlternateFilename:nil];
        [messageComposer setBody:HASH_TAGS];
        [messageComposer setMessageComposeDelegate:self];
        [UnityGetGLViewController() presentViewController:messageComposer animated:YES completion:^{
        }];
    });
}

-(void) shareInstagram:(NSString *) videoPath{
    dispatch_async(dispatch_get_main_queue(), ^{
       // do work here to Usually to update the User Interface
        NSURL *instagramURL = [NSURL URLWithString:@"instagram://location?id=1"];
        if([[UIApplication sharedApplication] canOpenURL:instagramURL]){
            NSString *localIdentifier = [self->videoGalleryIdentifiers objectForKey:[NSNumber numberWithInt:self->pictureId]];
            if(localIdentifier == nil || ![self isIdentifierExistInGallery:localIdentifier]){
                if([PHPhotoLibrary authorizationStatus] != PHAuthorizationStatusAuthorized){
                    [PHPhotoLibrary requestAuthorization:^(PHAuthorizationStatus status) {
                        if(status == PHAuthorizationStatusAuthorized){
                            UISaveVideoAtPathToSavedPhotosAlbum(videoPath, self, @selector(shareVideoToInstagram:didFinishSavingWithError:contextInfo:), nil);
                        } else {
                            [self showRequestPermissionPopup];
                        }
                    }];
                } else{
                    UISaveVideoAtPathToSavedPhotosAlbum(videoPath, self, @selector(shareVideoToInstagram:didFinishSavingWithError:contextInfo:), nil);
                }
            } else {
                [self shareVideoToInstagramByLocalIdentifier:localIdentifier];
            }
        } else {
            [self openInstagramOnAppStore];
        }
    });
}

-(void) shareVideoToInstagram: (NSString *) video didFinishSavingWithError:(NSError *) error contextInfo: (void *) info{
    NSLog(@"start share video to instagram");
    PHFetchOptions *ops = [[PHFetchOptions alloc] init];
    ops.predicate = [NSPredicate predicateWithFormat:@"mediaType == %d", PHAssetMediaTypeVideo];
    ops.sortDescriptors = @[[[NSSortDescriptor alloc] initWithKey:@"creationDate" ascending:false]];
    PHAsset *lastVideo = [[PHAsset fetchAssetsWithOptions:ops] firstObject];
    if(lastVideo){
        NSString *localIdentifier = [lastVideo localIdentifier];
        [self->videoGalleryIdentifiers setObject:localIdentifier forKey:[NSNumber numberWithInt:self->pictureId]];
        [self shareVideoToInstagramByLocalIdentifier:localIdentifier];
    }
}

-(void) shareVideoToInstagramByLocalIdentifier:(NSString *) localIdentifier{
    NSURL *instagramUrl = [NSURL URLWithString:[NSString stringWithFormat:@"instagram://library?LocalIdentifier=%@", localIdentifier]];
    [[UIApplication sharedApplication] openURL:instagramUrl options:@{} completionHandler:nil];
}

-(void) shareOthers: (NSString *) videoPath{
    dispatch_async(dispatch_get_main_queue(), ^{
        NSURL *videoUrl = [NSURL fileURLWithPath:videoPath];
        NSArray *items = @[videoUrl, HASH_TAGS];
        UIActivityViewController *ac = [[UIActivityViewController alloc] initWithActivityItems:items applicationActivities:nil];
        ac.modalPresentationStyle = UIModalPresentationPopover;
        UIViewController *parentController = UnityGetGLViewController();
        if(UI_USER_INTERFACE_IDIOM() == UIUserInterfaceIdiomPad){
            ac.modalPresentationStyle = UIModalPresentationPopover;
            UIPopoverPresentationController *popoverController = ac.popoverPresentationController;
            if(popoverController != nil){
                popoverController.sourceView = parentController.view;
                popoverController.sourceRect = CGRectMake(0,200,768,20);
                [parentController presentViewController:ac animated:YES completion:nil];
            }
        } else{
            [parentController presentViewController:ac animated:YES completion:nil];
        }
//        if(UI_USER_INTERFACE_IDIOM() == UIUserInterfaceIdiomPad){
//            UIPopoverController *popup = [[UIPopoverController alloc] initWithContentViewController:ac];
//            [popup presentPopoverFromRect:CGRectMake(parentController.view.frame.size.width / 2, parentController.view.frame.size.height / 2, 0, 0) inView:parentController.view permittedArrowDirections:UIPopoverArrowDirectionAny animated:YES];
//        } else{
//            [parentController presentViewController:ac animated:YES completion:nil];
//        }
    });
}

-(void) openInstagramOnAppStore{
    NSURL *instagramAppStoreUrl = [NSURL URLWithString:@"itms-apps://itunes.apple.com/app/id389801252"];
        [[UIApplication sharedApplication] openURL:instagramAppStoreUrl options:@{} completionHandler:^(BOOL success) {
        }];
}

-(void) produceVideo: (NSData *) originalPic withGray: (NSData *) grayPic withSteps: (NSArray *) steps withLogo: (NSData *) logo andStepsByFrame: (int) stepsByFrame withFPS: (int) fps withPadding:(int)padding{
    NSString *videoPath = [videoPaths objectForKey:[NSNumber numberWithInt:self->pictureId]];
    if(videoPath == nil || ![[NSFileManager defaultManager] fileExistsAtPath:videoPath]){
        NSLog(@"start producing video");
        UIImage *original = [UIImage imageWithData:originalPic];
        UIImage *gray = [UIImage imageWithData:grayPic];
        CGImageRef originalRef = [original CGImage];
        CGImageRef logoRef = nil;
        CGRect logoRect;
        if(logo != nil){
            logoRef = [[UIImage imageWithData:logo] CGImage];
            logoRect.origin.x = self->size.width - CGImageGetWidth(logoRef);
            logoRect.origin.y = 0;
            logoRect.size.width = CGImageGetWidth(logoRef);
            logoRect.size.height = CGImageGetHeight(logoRef);

        }
        uint width = CGImageGetWidth(originalRef);
        uint height = CGImageGetHeight(originalRef);
        CGImageRef grayRef = [gray CGImage];
        uint8_t *paintPixels = (uint8_t *)malloc(width * height * 4);
        uint8_t *originalPixels = (uint8_t *) malloc(width * height * 4);
        CGColorSpaceRef colorSpaceRef = CGColorSpaceCreateDeviceRGB();
        CGContextRef originalContext = CGBitmapContextCreate(originalPixels, width, height, 8, width * 4, colorSpaceRef, kCGImageAlphaNoneSkipLast);
        CGContextDrawImage(originalContext, CGRectMake(0, 0, width, height), originalRef);
        CGContextRef context = CGBitmapContextCreate(paintPixels, width, height, 8, width * 4, colorSpaceRef, kCGImageAlphaPremultipliedLast);
        CGContextSetFillColorWithColor(context, [[UIColor whiteColor] CGColor]);
        CGContextFillRect(context, CGRectMake(0, 0, width, height));
        CGContextSetBlendMode(context, kCGBlendModeMultiply);
        CGContextSetAlpha(context, 0.2f);
        CGContextDrawImage(context, CGRectMake(0, 0, width, height), grayRef);
        CGRect targetRect;
        if(width > height){
            targetRect.size.width = self->size.width - padding * 2;
            targetRect.size.height = targetRect.size.width * height / width;
            targetRect.origin.x = padding;
            targetRect.origin.y = (self->size.height - targetRect.size.height) / 2;
        } else {
            targetRect.size.height = self->size.height - padding * 2;
            targetRect.size.width = targetRect.size.height * width / height;
            targetRect.origin.x = (self->size.width - targetRect.size.width) / 2;
            targetRect.origin.y = padding;
        }
        int count = 0;
        int frameIndex = 0;
//        CGContextRef scaledContext = CGBitmapContextCreate(NULL, self->size.width, self->size.height, 8, self->size.width * 4, colorSpaceRef, kCGImageAlphaPremultipliedLast);
//        CGContextSetInterpolationQuality(scaledContext, kCGInterpolationNone);
//        CGContextSetFillColorWithColor(scaledContext, [[UIColor whiteColor] CGColor]);
//        CGContextFillRect(scaledContext, self->fullRect);

        for(int i = 0; i < steps.count; i++){
            NSInteger step = [[steps objectAtIndex:i] integerValue];
            int col = step >> 8;
            int row = height - 1 - (step & 255);
            int redIndex = row * width * 4 + col * 4;
            int greenIndex = redIndex + 1;
            int blueIndex = greenIndex + 1;
            int alphaIndex = blueIndex + 1;
            paintPixels[redIndex] = originalPixels[redIndex];
            paintPixels[greenIndex] = originalPixels[greenIndex];
            paintPixels[blueIndex] = originalPixels[blueIndex];
            paintPixels[alphaIndex] = 255;
            count++;
            if(count >= stepsByFrame || i == steps.count - 1){
    //            frameIndex++;
    //            NSLog([@"Render frame at index =  " stringByAppendingFormat:@"%d", frameIndex]);
                count = 0;
                CGImageRef paintedImage = CGBitmapContextCreateImage(context);
//                UIImage *newFrame = [self resizeImageUsingCGContext:paintedImage withTargetRect:targetRect];
//                CGContextDrawImage(scaledContext, targetRect, paintedImage);
//                CGImageRef scaledCGImage = CGBitmapContextCreateImage(scaledContext);
//                UIImage *newFrame = [UIImage imageWithCGImage:scaledCGImage];
//                NSLog([@"Frame " stringByAppendingFormat:@"%d", i]);
                [self->frames addObject:[UIImage imageWithCGImage:paintedImage]];
//                CGImageRelease(paintedImage);
//                CGImageRelease(scaledCGImage);
            }
        }
//        UIImage *originalFrame = [self resizeImageUsingCGContext:originalRef withLogo:logoRef withTargetRect:targetRect];
        for(int i = 0; i < 4; i++){
            [self->frames insertObject:original atIndex:0];
        }
    //    free(paintPixels);
    //    free(originalPixels);
        CGColorSpaceRelease(colorSpaceRef);
    //    CGImageRelease(grayRef);
    //    CGImageRelease(originalRef);
//        CGContextRelease(scaledContext);
        CGContextRelease(context);
        CGContextRelease(originalContext);
        free(paintPixels);
        free(originalPixels);
//    NSString *basePath = [NSTemporaryDirectory() stringByAppendingPathComponent:[NSString stringWithFormat:@""]];
//    BOOL isdir;
//    if(![[NSFileManager defaultManager] fileExistsAtPath:basePath isDirectory:&isdir]){
//        [[NSFileManager defaultManager] createDirectoryAtPath:basePath withIntermediateDirectories:true attributes:nil error:nil];
//    }
        NSString *path = [NSTemporaryDirectory() stringByAppendingPathComponent:[NSString stringWithFormat:@"paintbynumber_video_%d.mp4", self->pictureId]];
        [[NSFileManager defaultManager] removeItemAtPath:path error:NULL];
        [HJImagesToVideo setLogo:logoRef withTargetRect:logoRect];
        [HJImagesToVideo videoFromImages:frames toPath:path withSize:self->size withFPS:fps animateTransitions: false withCallbackBlock:^(BOOL success) {
            if(success){
                [self->videoPaths setObject:path forKey:[NSNumber numberWithInt:self->pictureId]];
                [self shareVideoFromPath:path];
                NSLog(@"Create video success");
            } else {
                NSLog(@"Create video failed");
            }
        } withTargetRect:targetRect];
        if(logoRef != nil){
//            CFRelease(logoRef);
            logoRef = nil;
            [HJImagesToVideo setLogo:nil withTargetRect:logoRect];
        }
    } else {
        [self shareVideoFromPath:videoPath];
    }
}

-(BOOL) isIdentifierExistInGallery: (NSString *) localIdentifier{
    return [[PHAsset fetchAssetsWithLocalIdentifiers:[NSArray arrayWithObject:localIdentifier] options:nil] count] > 0;
}

-(void) saveVideoToGallery:(NSString *) videoPath{
    UISaveVideoAtPathToSavedPhotosAlbum(videoPath, self, @selector(saveVideoToGalleryFinished:didFinishSavingWithError:contextInfo:), nil);
}

-(void)saveVideoToGalleryFinished:(NSString *) videoPath
didFinishSavingWithError: (NSError *) error
             contextInfo: (void *) contextInfo {
    if(error == nil){
        PHFetchOptions *ops = [[PHFetchOptions alloc] init];
        ops.predicate = [NSPredicate predicateWithFormat:@"mediaType == %d", PHAssetMediaTypeVideo];
        ops.sortDescriptors = @[[[NSSortDescriptor alloc] initWithKey:@"creationDate" ascending:false]];
        PHAsset *lastVideo = [[PHAsset fetchAssetsWithOptions:ops] firstObject];
        [self->videoGalleryIdentifiers setObject:[lastVideo localIdentifier] forKey:[NSNumber numberWithInt:self->pictureId]];
    }
}

-(void) savePhotoToGallery:(UIImage *) image{
    UIImageWriteToSavedPhotosAlbum(image, self, @selector(savePhotoToGalleryFinished:didFinishSavingWithError:contextInfo:), nil);
}

-(void) savePhotoToGalleryFinished: (UIImage *)image
didFinishSavingWithError:(NSError *)error
             contextInfo:(void *)contextInfo{
    if(error == nil){
        PHFetchOptions *ops = [[PHFetchOptions alloc] init];
        ops.predicate = [NSPredicate predicateWithFormat:@"mediaType == %d", PHAssetMediaTypeImage];
        ops.sortDescriptors = @[[[NSSortDescriptor alloc] initWithKey:@"creationDate" ascending:false]];
        PHAsset *lastImage = [[PHAsset fetchAssetsWithOptions:ops] firstObject];
        NSString *imageIdentifier = [lastImage localIdentifier];
        [self->pictureGalleryIdentifiers setObject:imageIdentifier forKey:[NSNumber numberWithInt:self->pictureId]];
    }
}

-(void) shareVideoFromPath: (NSString *) path{
    switch (self->shareType) {
        case SHARE_SAVE_GALLERY:
            [self saveVideo:path];
            break;
        case SHARE_SAVE_IMESSAGE:
            [self sendiMessage:path];
            break;
        case SHARE_SAVE_INSTAGRAM:
            [self shareInstagram:path];
            break;
        default:
            [self shareOthers:path];
            break;
    }
}

-(UIImage *) resizeImage: (CGImageRef) cgImage withSize:(CGSize) size {
    UIImage *inputImage = [UIImage imageWithCGImage:cgImage];
    CGRect newRect = CGRectMake(0, 0, size.width, size.height);
    UIGraphicsImageRenderer *renderer = [[UIGraphicsImageRenderer alloc] initWithSize:size];
    return [renderer imageWithActions:^(UIGraphicsImageRendererContext * _Nonnull rendererContext) {
        return [inputImage drawInRect:newRect];
    }];
}

-(UIImage *) resizeImageUsingCGContext: (CGImageRef) cgImage withLogo:(CGImageRef) cgImageLogo withTargetRect:(CGRect) targetRect {
//    @autoreleasepool {
//        UIGraphicsImageRenderer *renderer = [[UIGraphicsImageRenderer alloc] initWithSize:targetRect.size];
//        UIImage *ret = [renderer imageWithActions:^(UIGraphicsImageRendererContext * _Nonnull rendererContext) {
//            [[UIColor whiteColor] setStroke];
//            [rendererContext fillRect:targetRect];
//            [[UIImage imageWithCGImage:cgImage] drawInRect:targetRect blendMode:kCGBlendModeNormal alpha:1.0f];
//        }];
//        return ret;
//    }
    
    CGColorSpaceRef colorSpace = CGColorSpaceCreateDeviceRGB();
//    uint8_t *buffer = (uint8_t *)malloc(self->size.height * self->size.width * 8 * 4);
    CGContextRef context = CGBitmapContextCreate(NULL, self->size.width, self->size.height, 8, self->size.width * 4, colorSpace, kCGImageAlphaPremultipliedLast);
    CGContextSetInterpolationQuality(context, kCGInterpolationNone);
    CGContextSetFillColorWithColor(context, [[UIColor whiteColor] CGColor]);
    CGContextFillRect(context, self->fullRect);
    CGContextDrawImage(context, targetRect, cgImage);
    if(cgImageLogo != nil){
        CGRect logoRect;
        logoRect.origin.x = self->size.width - CGImageGetWidth(cgImageLogo);
        logoRect.origin.y = 0;
        logoRect.size.width = CGImageGetWidth(cgImageLogo);
        logoRect.size.height = CGImageGetHeight(cgImageLogo);
        CGContextDrawImage(context, logoRect, cgImageLogo);
    }
//    size_t dataLength = CGBitmapContextGetBytesPerRow(context) * CGBitmapContextGetHeight(context);
//    NSData *pixelData = [NSData dataWithBytesNoCopy:CGBitmapContextGetData(context) length:dataLength freeWhenDone:YES];
    CGImageRef scaledCGImage = CGBitmapContextCreateImage(context);
    UIImage *ret = [UIImage imageWithCGImage:scaledCGImage];
    CGColorSpaceRelease(colorSpace);
    CGContextRelease(context);
//    free(buffer);
    CFRelease(scaledCGImage);
    return ret;
}



-(void)messageComposeViewController:(MFMessageComposeViewController *)controller didFinishWithResult:(MessageComposeResult)result{
    [controller dismissViewControllerAnimated:YES completion:nil];
}

void runOnMainQueueWithoutDeadlocking(void (^block)(void))
{
    if ([NSThread isMainThread])
    {
        block();
    }
    else
    {
        dispatch_sync(dispatch_get_main_queue(), block);
    }
}

-(void)showRequestPermissionPopup{
    NSString *accessDescription = [[NSBundle mainBundle] objectForInfoDictionaryKey:@"NSPhotoLibraryUsageDescription"];
    UIAlertController * alertController = [UIAlertController alertControllerWithTitle:accessDescription message:@"Please Allow Access to Photos in Settings" preferredStyle:UIAlertControllerStyleAlert];
                        
    UIAlertAction *cancelAction = [UIAlertAction actionWithTitle:@"Cancel" style:UIAlertActionStyleCancel handler:nil];
    [alertController addAction:cancelAction];
    
    UIAlertAction *settingsAction = [UIAlertAction actionWithTitle:@"Settings" style:UIAlertActionStyleDefault handler:^(UIAlertAction * _Nonnull action) {
        [[UIApplication sharedApplication] openURL:[NSURL URLWithString:UIApplicationOpenSettingsURLString] options:@{} completionHandler:nil];
    }];
    [alertController addAction:settingsAction];
    runOnMainQueueWithoutDeadlocking(^{
        [UnityGetGLViewController() presentViewController:alertController animated:YES completion:nil];
    });
}

@end

extern "C" {
    void NativeInitialize(int pictureId, int width, int height, int shareType){
        [[MediaMuxer instance] initialize:pictureId width:width height:height with:shareType];
    }

    void NativeAppendFrame(unsigned char *data, int count, bool isLast){
    }

    void NativeGenerateVideo(const unsigned char *originalPic, const int originalLength, const unsigned char *grayPic, const int grayLength, const unsigned short *steps, const int stepLength, const int stepsByFrame, const int fps, const int padding){
        NSLog(@"Create Video");
        NSData *originalData = [NSData dataWithBytes:originalPic length:originalLength];
        NSData *grayData = [NSData dataWithBytes:grayPic length:grayLength];
        NSMutableArray *stepArray = [[NSMutableArray alloc] init];
        for(int i = 0; i < stepLength; i++){
            [stepArray addObject:[NSNumber numberWithInt:steps[i]]];
        }
        [[MediaMuxer instance] produceVideo:originalData withGray:grayData withSteps:[stepArray copy] withLogo: nil andStepsByFrame:stepsByFrame withFPS:fps withPadding:padding];
    }

    void NativeGenerateVideoWithLogo(const unsigned char *originalPic, const int originalLength, const unsigned char *grayPic, const int grayLength, const unsigned short *steps, const int stepLength, const unsigned char *logoPic, const int logolength, const int stepsByFrame, const int fps, const int padding){
        NSLog(@"Create Video");
        NSData *originalData = [NSData dataWithBytes:originalPic length:originalLength];
        NSData *grayData = [NSData dataWithBytes:grayPic length:grayLength];
        NSData *logoData = [NSData dataWithBytes:logoPic length:logolength];
        NSMutableArray *stepArray = [[NSMutableArray alloc] init];
        for(int i = 0; i < stepLength; i++){
            [stepArray addObject:[NSNumber numberWithInt:steps[i]]];
        }
        [[MediaMuxer instance] produceVideo:originalData withGray:grayData withSteps:[stepArray copy] withLogo: logoData andStepsByFrame:stepsByFrame withFPS:fps withPadding:padding];
    }

    void NativeSaveImage(unsigned char *bytes, int dataLength, int padding){
        NSLog(@"Save photo");
        NSData *data = [NSData dataWithBytes:bytes length:dataLength];
        [[MediaMuxer instance] savePhoto:data withLogo:nil withPadding:padding];
    }

    void NativeSaveImageWithLogo(unsigned char *bytes, int dataLength, unsigned char *logoBytes, int logoLength, int padding){
        NSLog(@"Save photo with logo");
        NSData *data = [NSData dataWithBytes:bytes length:dataLength];
        NSData *logoData = [NSData dataWithBytes:logoBytes length:logoLength];
        [[MediaMuxer instance] savePhoto:data withLogo:logoData withPadding:padding];
    }
}
