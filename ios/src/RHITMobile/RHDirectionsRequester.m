//
//  RHDirectionsRequester.m
//  RHITMobile
//
//  Created by Jimmy Theis on 12/11/11.
//  Copyright (c) 2011 Rose-Hulman Institute of Technology. All rights reserved.
//

#import "RHDirectionsRequester.h"
#import "RHWrappedCoordinate.h"
#import "RHWebRequestMaker.h"
#import "RHDirectionLineItem.h"

#define kDirectionsPath @"/directions/testing/directions"
#define kResultsKey @"Result"
#define kPathsKey @"Paths"
#define kDirectionsNameKey @"Dir"
#define kFlaggedKey @"Flagged"
#define kCoordinateKey @"To"
#define kLatKey @"Lat"
#define kLngKey @"Lon"

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
        [self performSelectorInBackground:@selector(requestLocations) withObject:nil];
        return;
    }
    
    NSDictionary *response = [RHWebRequestMaker JSONGetRequestWithPath:kDirectionsPath URLargs:@""];
    
    NSDictionary *results = [response objectForKey:kResultsKey];
    
    NSArray *paths = [results objectForKey:kPathsKey];
    
    NSMutableArray *result = [NSMutableArray arrayWithCapacity:paths.count];
    
    for (NSDictionary *lineItem in paths) {
        RHDirectionLineItem *item = [[[RHDirectionLineItem alloc] init] autorelease];
        item.name = [lineItem objectForKey:kDirectionsNameKey];
        item.flagged = [[lineItem objectForKey:kFlaggedKey] intValue] == 1;
        
        NSDictionary *coordinateDict = [lineItem objectForKey:kCoordinateKey];
        
        NSNumber *lat = [coordinateDict objectForKey:kLatKey];
        NSNumber *lng = [coordinateDict objectForKey:kLngKey];
        
        item.coordinate = CLLocationCoordinate2DMake(lat.doubleValue, lng.doubleValue);
        
        [result addObject:item];
    }
    
    [self.delegate didFinishLoadingDirections:result];
}

@end
