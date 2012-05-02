//
//  RHLocationsLoader.h
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

#import <Foundation/Foundation.h>
#import <CoreData/CoreData.h>

#import "RHLoader.h"


@protocol RHLocationsLoaderSpecificLocationDelegate <NSObject>

- (void)loaderDidFinishUpdatingLocationWithID:(NSManagedObjectID *)locationID;

- (void)loaderDidFailToUpdateLocation:(NSError *)error;

@end


@interface RHLocationsLoader : RHLoader

+ (id)instance;

@property (nonatomic, readonly) BOOL currentlyUpdating;

- (void)updateLocations:(NSNumber *)version;

- (void)addDelegateForTopLevelLocations:(NSObject<RHLoaderDelegate> *)delegate;

- (void)removeDelegateForTopLevelLocations:(NSObject<RHLoaderDelegate> *)delegate;

- (void)addDelegateForAllInternalLocations:(NSObject<RHLoaderDelegate> *)delegate;

- (void)removeDelegateForAllInternalLocations:(NSObject<RHLoaderDelegate> *)delegate;

- (void)addDelegate:(NSObject<RHLocationsLoaderSpecificLocationDelegate> *)delegate forLocationWithServerID:(NSNumber *)serverID;

- (void)removeDelegate:(NSObject<RHLocationsLoaderSpecificLocationDelegate> *)delegate forLocationWithServerID:(NSNumber *)serverID;

@end
