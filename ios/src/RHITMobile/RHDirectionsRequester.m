//
//  RHDirectionsRequester.m
//  RHITMobile
//
//  Created by Jimmy Theis on 12/11/11.
//  Copyright (c) 2011 Rose-Hulman Institute of Technology. All rights reserved.
//

#import "RHDirectionsRequester.h"
#import "RHWrappedCoordinate.h"

@implementation RHDirectionsRequester

@synthesize delegate;

- (id)initWithDelegate:(id<RHDirectionsRequesterDelegate>)inDelegate {
    self = [super init];
    if (self) {
        self.delegate = inDelegate;
        [self requestLocations];
    }
    return self;
}

- (void)requestLocations {
    if ([NSThread isMainThread]) {
        NSLog(@"Preparing");
        [self performSelectorInBackground:@selector(requestLocations) withObject:nil];
        return;
    }
    
    [self.delegate didFinishLoadingDirections:[NSArray
            arrayWithObjects:[RHWrappedCoordinate coordinateWithLatitude:39.48409 longitude:-87.32586],
            [RHWrappedCoordinate coordinateWithLatitude:39.48344 longitude:-87.32528], 
            [RHWrappedCoordinate coordinateWithLatitude:39.48377 longitude:-87.32470], 
            [RHWrappedCoordinate coordinateWithLatitude:39.48399 longitude:-87.32401], 
            [RHWrappedCoordinate coordinateWithLatitude:39.48429 longitude:-87.32288],
            [RHWrappedCoordinate coordinateWithLatitude:39.48344 longitude:-87.32258], nil]];
    
}

@end
