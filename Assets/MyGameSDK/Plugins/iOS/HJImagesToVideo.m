//
//  HJImagesToVideo.m
//  HJImagesToVideo
//
//  Created by Harrison Jackson on 8/4/13.
//  Copyright (c) 2013 Harrison Jackson. All rights reserved.
//

#import "HJImagesToVideo.h"

CGSize const DefaultFrameSize                             = (CGSize){480, 320};

NSInteger const DefaultFrameRate                          = 1;
NSInteger const TransitionFrameCount                      = 50;
NSInteger const FramesToWaitBeforeTransition              = 40;

BOOL const DefaultTransitionShouldAnimate = YES;
static CGRect targetRect;
static CGRect logoTargetRect;
static CGImageRef logoRef;
@implementation HJImagesToVideo

+ (void)setLogo:(CGImageRef)logo withTargetRect:(CGRect)rect{
    logoRef = logo;
    logoTargetRect = rect;
}

+ (void)videoFromImages:(NSArray *)images
                 toPath:(NSString *)path
      withCallbackBlock:(SuccessBlock)callbackBlock
         withTargetRect:(CGRect) targetRect;
{
    [HJImagesToVideo videoFromImages:images
                              toPath:path
                            withSize:DefaultFrameSize
                             withFPS:DefaultFrameRate
                  animateTransitions:DefaultTransitionShouldAnimate
                   withCallbackBlock:callbackBlock
                      withTargetRect:targetRect];
}

+ (void)videoFromImages:(NSArray *)images
                 toPath:(NSString *)path
     animateTransitions:(BOOL)animate
      withCallbackBlock:(SuccessBlock)callbackBlock
         withTargetRect:(CGRect)targetRect
{
    [HJImagesToVideo videoFromImages:images
                              toPath:path
                            withSize:DefaultFrameSize
                             withFPS:DefaultFrameRate
                  animateTransitions:animate
                   withCallbackBlock:callbackBlock
                      withTargetRect:targetRect];
}

+ (void)videoFromImages:(NSArray *)images
                 toPath:(NSString *)path
                withFPS:(int)fps
     animateTransitions:(BOOL)animate
      withCallbackBlock:(SuccessBlock)callbackBlock
         withTargetRect:(CGRect)targetRect
{
    [HJImagesToVideo videoFromImages:images
                              toPath:path
                            withSize:DefaultFrameSize
                             withFPS:fps
                  animateTransitions:animate
                   withCallbackBlock:callbackBlock
                      withTargetRect:targetRect];
}

+ (void)videoFromImages:(NSArray *)images
                 toPath:(NSString *)path
               withSize:(CGSize)size
     animateTransitions:(BOOL)animate
      withCallbackBlock:(SuccessBlock)callbackBlock
         withTargetRect:(CGRect)targetRect
{
    [HJImagesToVideo videoFromImages:images
                              toPath:path
                            withSize:size
                             withFPS:DefaultFrameRate
                  animateTransitions:animate
                   withCallbackBlock:callbackBlock
                      withTargetRect:targetRect];
}

+ (void)videoFromImages:(NSArray *)images
                 toPath:(NSString *)path
               withSize:(CGSize)size
                withFPS:(int)fps
     animateTransitions:(BOOL)animate
      withCallbackBlock:(SuccessBlock)callbackBlock
         withTargetRect:(CGRect)targetRect
{
    [HJImagesToVideo writeImageAsMovie:images
                                toPath:path
                                  size:size
                                   fps:fps
                    animateTransitions:animate
                     withCallbackBlock:callbackBlock
     withTargetRect:targetRect
     ];
}

+ (void)saveVideoToPhotosWithImages:(NSArray *)images
                  withCallbackBlock:(SuccessBlock)callbackBlock
{
    [HJImagesToVideo saveVideoToPhotosWithImages:images
                                        withSize:DefaultFrameSize
                              animateTransitions:DefaultTransitionShouldAnimate
                               withCallbackBlock:callbackBlock];
}

+ (void)saveVideoToPhotosWithImages:(NSArray *)images
                 animateTransitions:(BOOL)animate
                  withCallbackBlock:(SuccessBlock)callbackBlock
{
    [HJImagesToVideo saveVideoToPhotosWithImages:images
                                        withSize:DefaultFrameSize
                              animateTransitions:animate
                               withCallbackBlock:callbackBlock];
}

+ (void)saveVideoToPhotosWithImages:(NSArray *)images
                           withSize:(CGSize)size
                 animateTransitions:(BOOL)animate
                  withCallbackBlock:(SuccessBlock)callbackBlock
{
    [HJImagesToVideo saveVideoToPhotosWithImages:images
                                        withSize:size
                                         withFPS:DefaultFrameRate
                              animateTransitions:animate
                               withCallbackBlock:callbackBlock];
}

+ (void)saveVideoToPhotosWithImages:(NSArray *)images
                            withFPS:(int)fps
                 animateTransitions:(BOOL)animate
                  withCallbackBlock:(SuccessBlock)callbackBlock
{
    [HJImagesToVideo saveVideoToPhotosWithImages:images
                                        withSize:DefaultFrameSize
                                         withFPS:fps
                              animateTransitions:animate
                               withCallbackBlock:callbackBlock];
}

+ (void)saveVideoToPhotosWithImages:(NSArray *)images
                           withSize:(CGSize)size
                            withFPS:(int)fps
                 animateTransitions:(BOOL)animate
                  withCallbackBlock:(SuccessBlock)callbackBlock
{
    NSString *tempPath = [NSTemporaryDirectory() stringByAppendingPathComponent:
                          [NSString stringWithFormat:@"temp.mp4"]];
    [[NSFileManager defaultManager] removeItemAtPath:tempPath error:NULL];
    
    [HJImagesToVideo videoFromImages:images
                              toPath:tempPath
                            withSize:size
                             withFPS:fps
                  animateTransitions:animate
                   withCallbackBlock:^(BOOL success) {
                       
                       if (success) {
                           UISaveVideoAtPathToSavedPhotosAlbum(tempPath, self, nil, nil);
                       }
                       
                       if (callbackBlock) {
                           callbackBlock(success);
                       }
                   }
                      withTargetRect:targetRect];
}

+ (void)writeImageAsMovie:(NSArray *)array
                   toPath:(NSString*)path
                     size:(CGSize)size
                      fps:(int)fps
       animateTransitions:(BOOL)shouldAnimateTransitions
        withCallbackBlock:(SuccessBlock)callbackBlock
           withTargetRect:(CGRect) targetRect
{
    NSLog(@"%@", path);
    NSError *error = nil;
    AVAssetWriter *videoWriter = [[AVAssetWriter alloc] initWithURL:[NSURL fileURLWithPath:path]
                                                           fileType:AVFileTypeMPEG4
                                                              error:&error];
    if (error) {
        if (callbackBlock) {
            callbackBlock(NO);
        }
        return;
    }
    NSParameterAssert(videoWriter);
    NSDictionary *videoSettings;
    if (@available(iOS 11.0, *)) {
        videoSettings = @{AVVideoCodecKey: AVVideoCodecTypeH264,
                                        AVVideoWidthKey: [NSNumber numberWithInt:size.width],
                                        AVVideoHeightKey: [NSNumber numberWithInt:size.height]};
    } else {
        videoSettings = @{AVVideoCodecKey: AVVideoCodecH264,
                                        AVVideoWidthKey: [NSNumber numberWithInt:size.width],
                                        AVVideoHeightKey: [NSNumber numberWithInt:size.height]};
    }
    
    AVAssetWriterInput* writerInput = [AVAssetWriterInput assetWriterInputWithMediaType:AVMediaTypeVideo
                                                                         outputSettings:videoSettings];

    AVAssetWriterInputPixelBufferAdaptor *adaptor = [AVAssetWriterInputPixelBufferAdaptor assetWriterInputPixelBufferAdaptorWithAssetWriterInput:writerInput
                                                                                                                     sourcePixelBufferAttributes:nil];
    NSParameterAssert(writerInput);
    NSParameterAssert([videoWriter canAddInput:writerInput]);
    [videoWriter addInput:writerInput];
    
    //Start a session:
    [videoWriter startWriting];
    [videoWriter startSessionAtSourceTime:kCMTimeZero];
    
    CVPixelBufferRef buffer;
    CVPixelBufferPoolCreatePixelBuffer(NULL, adaptor.pixelBufferPool, &buffer);
    
    CMTime presentTime = CMTimeMake(0, fps);
    CGSize targetSize = CGSizeMake(640, 640);
    int i = 0;
    while (1)
    {
        
    if(writerInput.readyForMoreMediaData){
            
      presentTime = CMTimeMake(i, fps);
            
      if (i >= [array count]) {
        buffer = NULL;
      } else {
        buffer = [HJImagesToVideo pixelBufferFromCGImage:[array[i] CGImage] size:targetSize withTargetRect:targetRect];
      }
      
      if (buffer) {
                //append buffer
                
                BOOL appendSuccess = [HJImagesToVideo appendToAdapter:adaptor
                                                          pixelBuffer:buffer
                                                               atTime:presentTime
                                                            withInput:writerInput];
                NSAssert(appendSuccess, @"Failed to append");
                
                if (shouldAnimateTransitions && i + 1 < array.count) {

                    //Create time each fade frame is displayed
                    CMTime fadeTime = CMTimeMake(1, fps*TransitionFrameCount);
            
                    //Add a delay, causing the base image to have more show time before fade begins.
                    for (int b = 0; b < FramesToWaitBeforeTransition; b++) {
                        presentTime = CMTimeAdd(presentTime, fadeTime);
                    }
                    
                    //Adjust fadeFrameCount so that the number and curve of the fade frames and their alpha stay consistant
                    NSInteger framesToFadeCount = TransitionFrameCount - FramesToWaitBeforeTransition;
                    
                    //Apply fade frames
                    for (double j = 1; j < framesToFadeCount; j++) {
                        
                        buffer = [HJImagesToVideo crossFadeImage:[array[i] CGImage]
                                                         toImage:[array[i + 1] CGImage]
                                                          atSize:CGSizeMake(640, 640)
                                                       withAlpha:j/framesToFadeCount
                                                  withTargetRect:targetRect];
                        
                        BOOL appendSuccess = [HJImagesToVideo appendToAdapter:adaptor
                                                                  pixelBuffer:buffer
                                                                       atTime:presentTime
                                                                    withInput:writerInput];
                        presentTime = CMTimeAdd(presentTime, fadeTime);
                        
                        NSAssert(appendSuccess, @"Failed to append");
                    }
                }
                
                i++;
      } else {
        
        //Finish the session:
        [writerInput markAsFinished];
                
        [videoWriter finishWritingWithCompletionHandler:^{
                    NSLog(@"Successfully closed video writer");
                    if (videoWriter.status == AVAssetWriterStatusCompleted) {
                        if (callbackBlock) {
                            callbackBlock(YES);
                        }
                    } else {
                        if (callbackBlock) {
                            callbackBlock(NO);
                        }
                    }
                }];
        
        CVPixelBufferPoolRelease(adaptor.pixelBufferPool);
        
        NSLog (@"Done");
                break;
            }
        }
    }
}

+ (CVPixelBufferRef)pixelBufferFromCGImage:(CGImageRef)image
                                      size:(CGSize)imageSize
                            withTargetRect:(CGRect) targetRect
{
    NSDictionary *options = @{(id)kCVPixelBufferCGImageCompatibilityKey: @YES,
                              (id)kCVPixelBufferCGBitmapContextCompatibilityKey: @YES};
    CVPixelBufferRef pxbuffer = NULL;
    CVReturn status = CVPixelBufferCreate(kCFAllocatorDefault, imageSize.width,
                                          imageSize.height, kCVPixelFormatType_32ARGB, (__bridge CFDictionaryRef) options,
                                          &pxbuffer);
    NSParameterAssert(status == kCVReturnSuccess && pxbuffer != NULL);
    
    CVPixelBufferLockBaseAddress(pxbuffer, 0);
    void *pxdata = CVPixelBufferGetBaseAddress(pxbuffer);
    NSParameterAssert(pxdata != NULL);
    
    CGColorSpaceRef rgbColorSpace = CGColorSpaceCreateDeviceRGB();
    CGContextRef context = CGBitmapContextCreate(pxdata, imageSize.width,
                                                 imageSize.height, 8, 4*imageSize.width, rgbColorSpace,
                                                 kCGImageAlphaNoneSkipFirst);
    CGContextSetInterpolationQuality(context, kCGInterpolationNone);
    NSParameterAssert(context);
    CGContextSetFillColorWithColor(context, [[UIColor whiteColor] CGColor]);
    CGContextFillRect(context, CGRectMake(0, 0, imageSize.width, imageSize.height));

    CGContextDrawImage(context, targetRect, image);
    if(logoRef != nil){
        CGContextDrawImage(context, logoTargetRect, logoRef);
    }
    
    CGColorSpaceRelease(rgbColorSpace);
    CGContextRelease(context);
    
    CVPixelBufferUnlockBaseAddress(pxbuffer, 0);
//    CGImageRelease(image);
    
    return pxbuffer;
}

+ (CVPixelBufferRef)crossFadeImage:(CGImageRef)baseImage
                           toImage:(CGImageRef)fadeInImage
                            atSize:(CGSize)imageSize
                         withAlpha:(CGFloat)alpha
                    withTargetRect:(CGRect)targetRect
{
    NSDictionary *options = @{(id)kCVPixelBufferCGImageCompatibilityKey: @YES,
                              (id)kCVPixelBufferCGBitmapContextCompatibilityKey: @YES};
    CVPixelBufferRef pxbuffer = NULL;
    CVReturn status = CVPixelBufferCreate(kCFAllocatorDefault, imageSize.width,
                                          imageSize.height, kCVPixelFormatType_32ARGB, (__bridge CFDictionaryRef) options,
                                          &pxbuffer);
    NSParameterAssert(status == kCVReturnSuccess && pxbuffer != NULL);
    
    CVPixelBufferLockBaseAddress(pxbuffer, 0);
    void *pxdata = CVPixelBufferGetBaseAddress(pxbuffer);
    NSParameterAssert(pxdata != NULL);
    
    CGColorSpaceRef rgbColorSpace = CGColorSpaceCreateDeviceRGB();
    CGContextRef context = CGBitmapContextCreate(pxdata, imageSize.width,
                                                 imageSize.height, 8, 4*imageSize.width, rgbColorSpace,
                                                 kCGImageAlphaNoneSkipFirst);
    NSParameterAssert(context);
//    CGRect drawRect = CGRectMake(0 + (imageSize.width-CGImageGetWidth(baseImage))/2,
//                                 (imageSize.height-CGImageGetHeight(baseImage))/2,
//                                 CGImageGetWidth(baseImage),
//                                 CGImageGetHeight(baseImage));
    CGContextSetFillColorWithColor(context, [[UIColor whiteColor] CGColor]);
    CGContextFillRect(context, CGRectMake(0, 0, imageSize.width, imageSize.height));

    CGContextDrawImage(context, targetRect, baseImage);
    
    CGContextBeginTransparencyLayer(context, nil);
    CGContextSetAlpha( context, alpha );
    CGContextDrawImage(context, targetRect, fadeInImage);
    CGContextEndTransparencyLayer(context);
    
    CGColorSpaceRelease(rgbColorSpace);
    CGContextRelease(context);
    
    CVPixelBufferUnlockBaseAddress(pxbuffer, 0);
//    CGImageRelease(baseImage);
//    CGImageRelease(fadeInImage);
    
    return pxbuffer;
}

+ (BOOL)appendToAdapter:(AVAssetWriterInputPixelBufferAdaptor*)adaptor
            pixelBuffer:(CVPixelBufferRef)buffer
                 atTime:(CMTime)presentTime
              withInput:(AVAssetWriterInput*)writerInput
{
    while (!writerInput.readyForMoreMediaData) {
        usleep(1);
    }
    BOOL isAppend = [adaptor appendPixelBuffer:buffer withPresentationTime:presentTime];
    CVPixelBufferRelease(buffer);
    return isAppend;
}




@end
