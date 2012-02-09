//
//  RHTourRequester.m
//  Rose-Hulman Mobile
//
//  Copyright 2012 Rose-Hulman Institute of Technology
// 
//  Licensed under the Apache License, Version 2.0 (the "License");
//  you may not use this file except in compliance with the License.
//  You may obtain a copy of the License at
//
//      http://www.apache.org/licenses/LICENSE-2.0
//
//  Unless required by applicable law or agreed to in writing, software
//  distributed under the License is distributed on an "AS IS" BASIS,
//  WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//  See the License for the specific language governing permissions and
//  limitations under the License.
//

#import "RHTourRequester.h"
#import "RHWrappedCoordinate.h"
#import "RHWebRequestMaker.h"
#import "RHPathStep.h"

#define kDirectionsPath @"/directions/testing/tour"
#define kResultsKey @"Result"
#define kPathsKey @"Paths"
#define kDirectionsNameKey @"Dir"
#define kFlaggedKey @"Flag"
#define kCoordinateKey @"To"
#define kLatKey @"Lat"
#define kLngKey @"Lon"

@implementation RHTourRequester

@synthesize delegate;

- (id)initWithDelegate:(id<RHTourRequesterDelegate>)inDelegate {
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
        RHPathStep *item = [[RHPathStep alloc] init];
        item.name = [lineItem objectForKey:kDirectionsNameKey];
        item.flagged = [[lineItem objectForKey:kFlaggedKey] intValue] == 1;
        
        NSDictionary *coordinateDict = [lineItem objectForKey:kCoordinateKey];
        
        NSNumber *lat = [coordinateDict objectForKey:kLatKey];
        NSNumber *lng = [coordinateDict objectForKey:kLngKey];
        
        item.coordinate = CLLocationCoordinate2DMake(lat.doubleValue, lng.doubleValue);
        
        [result addObject:item];
    }
    
    [self.delegate didFinishLoadingTour:result];
}

@end

