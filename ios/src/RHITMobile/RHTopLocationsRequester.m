//
//  RHTopLocationsRequester.m
//  RHIT Mobile Campus Directory
//
//  Copyright 2011 Rose-Hulman Institute of Technology
// 
//  Licensed under the Apache License, Version 2.0 (the "License");
//  you may not use this file except in compliance with the License.
//  You may obtain a copy of the License at
//
//  http://www.apache.org/licenses/LICENSE-2.0
//
//  Unless required by applicable law or agreed to in writing, software
//  distributed under the License is distributed on an "AS IS" BASIS,
//  WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//  See the License for the specific language governing permissions and
//  limitations under the License.
//

#import "RHTopLocationsRequester.h"
#import "RHInternalLocationsRequester.h"
#import "RHLocation.h"
#import "RHWebRequestMaker.h"

#define kTopLevelServerPath @"/locations/data/top"
#define kLocationsKey @"Locations"


@implementation RHTopLocationsRequester

- (void)updateTopLevelLocations {
    // Only run on a background thread
    if ([NSThread isMainThread]) {
        [self performSelectorInBackground:@selector(updateTopLevelLocations) withObject:nil];
        return;
    }
    
    NSManagedObjectContext *localContext = self.threadSafeManagedObjectContext;
    
    [self deleteAllLocations:localContext];
    
    NSDictionary *response = [RHWebRequestMaker JSONGetRequestWithPath:kTopLevelServerPath
                                                               URLargs:@""];
    
    NSArray *locations = [response objectForKey:kLocationsKey];
    
    for (NSDictionary *locationDict in locations) {
        RHLocation *location = [self locationFromJSONResponse:locationDict
                                       inManagedObjectContext:localContext];
        location.retrievalStatus = RHLocationRetrievalStatusNoChildren;
    }
    
    NSError *saveError;
    [localContext save:&saveError];
    
    if (saveError) {
        NSLog(@"Problem saving top level locations: %@", saveError);
        return;
    }
    
    [self.delegate performSelectorOnMainThread:@selector(didFinishUpdatingTopLevelLocations)
                                    withObject:nil
                                 waitUntilDone:NO];
    
    [[[RHInternalLocationsRequester alloc] initWithDelegate:self.delegate
                                 persistantStoreCoordinator:self.persistantStoreCoordinator] 
     updateInternalLocations];
}

@end
