//
//  RHLocationsSearchRequester.m
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

#import "RHLocationsSearchRequester.h"
#import "RHLocation.h"
#import "RHWebRequestMaker.h"

#define kSearchLocationsServerPath @"/locations/names"
#define kNamesKey @"Names"
#define kIDKey @"Id"


@implementation RHLocationsSearchRequester

@synthesize delegate = delegate_;
@synthesize persistantStoreCoordinator = persistantStoreCoordinator_;

- (id)initWithDelegate:(NSObject<RHLocationsSearchRequesterDelegate> *)delegate persistantStoreCoordinator:(NSPersistentStoreCoordinator *)persistantStoreCoordinator {
    self = [super init];
    
    if (self) {
        self.delegate = delegate;
        self.persistantStoreCoordinator = persistantStoreCoordinator;
    }
    
    return self;
}

- (void)searchForLocations:(NSString *)searchTerm {
    // Only run on background thread
    if ([NSThread isMainThread]) {
        [self performSelectorInBackground:@selector(searchForLocations:) withObject:searchTerm];
        return;
    }
    
    NSManagedObjectContext *localContext = [[NSManagedObjectContext alloc] init];
    localContext.persistentStoreCoordinator = self.persistantStoreCoordinator;
    
    NSString *tokenizedTerm = [searchTerm stringByReplacingOccurrencesOfString:@" " withString:@"+"];
    NSString *sanitizedSearchTerm = [tokenizedTerm stringByAddingPercentEscapesUsingEncoding:NSUTF8StringEncoding];
    NSString *urlArgs = [NSString stringWithFormat:@"?s=%@", sanitizedSearchTerm];
    
    NSDictionary *jsonData = [RHWebRequestMaker JSONGetRequestWithPath:kSearchLocationsServerPath URLargs:urlArgs];
    
    NSArray *locations = [jsonData objectForKey:kNamesKey];
    
    NSMutableArray *result = [NSMutableArray arrayWithCapacity:locations.count];    
    
    for (NSDictionary *location in locations) {
        NSFetchRequest *fetchRequest = [NSFetchRequest fetchRequestWithEntityName:kRHLocationEntityName];
        
        NSPredicate *predicate = [NSPredicate predicateWithFormat:
                                  @"serverIdentifier == %d", [[location objectForKey:kIDKey] intValue]];
        fetchRequest.predicate = predicate;
        
        NSArray *results = [localContext executeFetchRequest:fetchRequest error:nil];
        
        if (results.count > 0) {
            RHLocation *matchingLocation = [results objectAtIndex:0];
            [result addObject:matchingLocation.objectID];
        }
    }
    
    [self.delegate performSelectorOnMainThread:@selector(didFindLocationSearchResults:)
                                    withObject:result
                                 waitUntilDone:NO];
}

@end
