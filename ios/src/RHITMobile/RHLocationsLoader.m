//
//  RHLocationsLoader.m
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

#import "RHLocationsLoader.h"
#import "RHLoaderRequestsWrapper.h"


@interface RHLocationsLoader () {
    @private
    BOOL _currentlyUpdating;
}

@end


@implementation RHLocationsLoader

static RHLocationsLoader *_instance;

+ (void)initialize
{
    static BOOL initialized = NO;
    if(!initialized)
    {
        initialized = YES;
        _instance = [[RHLocationsLoader alloc] init];
    }
}

- (id)init
{
    if (self = [super init]) {
        _currentlyUpdating = NO;
    }
    
    return self;
}

+ (id)instance
{
    return _instance;
}

- (BOOL)currentlyUpdating
{
    return _currentlyUpdating;
}

- (void)updateLocations:(double)version
{
    [RHLoaderRequestsWrapper makeTopLocationsRequestWithVersion:version successBlock:^(NSDictionary *jsonDict) {
        
        // TODO
        
        _currentlyUpdating = NO;
        
    } failureBlock:^(NSError *error) {
        
        NSLog(@"Error while updating top level locations: %@", error);
        _currentlyUpdating = NO;
        
    }];
}

- (void)registerCallbackForTopLevelLocations:(void (^)(void))callback
{
    // TODO
}

- (void)registerCallbackForLocationWithId:(NSInteger)locationId
                                 callback:(void (^)(void))callback
{
    // TODO
}

@end
