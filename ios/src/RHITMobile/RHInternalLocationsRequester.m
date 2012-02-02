//
//  RHInternalLocationsRequester.m
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

#import "RHInternalLocationsRequester.h"
#import "RHDataVersionManager.h"
#import "RHLocation.h"
#import "RHWebRequestMaker.h"

#define kInternalLocationServerPath @"/locations/data/within/%d"
#define kLocationsKey @"Locations"


@implementation RHInternalLocationsRequester

- (void)updateInternalLocations {
    // Only run on a background thread
    if ([NSThread isMainThread]) {
        [self performSelectorInBackground:@selector(updateInternalLocations) withObject:nil];
        return;
    }
    
    [[UIApplication sharedApplication] setNetworkActivityIndicatorVisible:YES];
    
    NSManagedObjectContext *localContext = self.threadSafeManagedObjectContext;
    
    NSFetchRequest *parentsFetchRequest = [NSFetchRequest fetchRequestWithEntityName:kRHLocationEntityName];
    
    NSPredicate *predicate = [NSPredicate predicateWithFormat:@"retrievalStatusNumber == %d", RHLocationRetrievalStatusNoChildren];
    
    parentsFetchRequest.predicate = predicate;
    
    NSError *parentsError;
    NSArray *parents = [localContext executeFetchRequest:parentsFetchRequest error:&parentsError];
    
    if (parentsError) {
        NSLog(@"Problem finding locations to be populated: %@", parentsError);
        return;
    }
    
    for (RHLocation *parent in parents) {
        NSString *serverPath = [NSString stringWithFormat:kInternalLocationServerPath,
                                parent.serverIdentifier.intValue];
        
        NSDictionary *response = [RHWebRequestMaker JSONGetRequestWithPath:serverPath
                                                                   URLargs:@""];
        
        NSArray *locations = [response objectForKey:kLocationsKey];

        for (NSDictionary *locationDict in locations) {
            RHLocation *location = [self locationFromJSONResponse:locationDict inManagedObjectContext:localContext];
            location.retrievalStatus = RHLocationRetrievalStatusFull;
        }
        
        parent.retrievalStatus = RHLocationRetrievalStatusFull;
    }
    
    NSError *saveError;
    [localContext save:&saveError];
    
    if (saveError) {
        NSLog(@"Problem saving internal locations: %@", saveError);
        return;
    }
    
    RHDataVersionManager *dataVersionManager = [RHDataVersionManager instance];
    [dataVersionManager upgradeLocationsVersion];
    
    [[UIApplication sharedApplication] setNetworkActivityIndicatorVisible:NO];
    
    [self.delegate performSelectorOnMainThread:@selector(didFinishUpdatingInternalLocations)
                                    withObject:nil
                                 waitUntilDone:NO];
}

@end
